using Microsoft.EntityFrameworkCore;

namespace BookAuditTrail;

public class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;

    public async Task<PagedResponse<AuditLogResponse>> GetAuditLogsAsync(AuditLogQueryParameters parameters)
    {
        var query = _auditLogRepository.GetQueryable().Include(a => a.Book).AsQueryable();

        query = ApplyFilters(query, parameters);
        query = ApplyOrdering(query, parameters);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(a => MapToResponse(a))
            .ToListAsync();

        return new PagedResponse<AuditLogResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task<PagedResponse<GroupedAuditLogResponse>> GetGroupedAuditLogsAsync(AuditLogQueryParameters parameters)
    {
        var query = _auditLogRepository.GetQueryable().Include(a => a.Book).AsQueryable();

        query = ApplyFilters(query, parameters);
        query = ApplyOrdering(query, parameters);

        var groupByField = (parameters.GroupBy ?? "BookId").ToLowerInvariant();

        var allItems = await query.ToListAsync();

        var grouped = groupByField switch
        {
            "bookid" => allItems.GroupBy(a => a.BookId.ToString())
                .Select(g => new GroupedAuditLogResponse
                {
                    GroupKey = g.First().Book?.Title ?? g.Key,
                    Count = g.Count(),
                    Items = g.Select(a => MapToResponse(a)).ToList()
                }),
            "date" => allItems.GroupBy(a => a.ChangedAt.Date.ToString("yyyy-MM-dd"))
                .Select(g => new GroupedAuditLogResponse
                {
                    GroupKey = g.Key,
                    Count = g.Count(),
                    Items = g.Select(a => MapToResponse(a)).ToList()
                }),
            _ => allItems.GroupBy(a => a.BookId.ToString())
                .Select(g => new GroupedAuditLogResponse
                {
                    GroupKey = g.First().Book?.Title ?? g.Key,
                    Count = g.Count(),
                    Items = g.Select(a => MapToResponse(a)).ToList()
                })
        };

        var groupedList = grouped.ToList();
        var totalCount = groupedList.Count;

        var pagedGroups = groupedList
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToList();

        return new PagedResponse<GroupedAuditLogResponse>
        {
            Items = pagedGroups,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<BookAuditLog> ApplyFilters(IQueryable<BookAuditLog> query, AuditLogQueryParameters parameters)
    {
        if (parameters.BookId.HasValue)
            query = query.Where(a => a.BookId == parameters.BookId.Value);

        if (parameters.FromDate.HasValue)
            query = query.Where(a => a.ChangedAt >= parameters.FromDate.Value);

        if (parameters.ToDate.HasValue)
            query = query.Where(a => a.ChangedAt <= parameters.ToDate.Value);

        return query;
    }

    private static IQueryable<BookAuditLog> ApplyOrdering(IQueryable<BookAuditLog> query, AuditLogQueryParameters parameters)
    {
        var orderBy = (parameters.OrderBy ?? "ChangedAt").ToLowerInvariant();

        query = orderBy switch
        {
            "changedat" => parameters.Descending
                ? query.OrderByDescending(a => a.ChangedAt)
                : query.OrderBy(a => a.ChangedAt),
            "bookid" => parameters.Descending
                ? query.OrderByDescending(a => a.BookId)
                : query.OrderBy(a => a.BookId),
            _ => parameters.Descending
                ? query.OrderByDescending(a => a.ChangedAt)
                : query.OrderBy(a => a.ChangedAt)
        };

        return query;
    }

    private static AuditLogResponse MapToResponse(BookAuditLog log)
    {
        return new AuditLogResponse
        {
            Id = log.Id,
            BookId = log.BookId,
            BookTitle = log.Book?.Title ?? string.Empty,
            ChangeType = log.ChangeType,
            FieldName = log.FieldName,
            OldValue = log.OldValue,
            NewValue = log.NewValue,
            Description = log.Description,
            ChangedAt = log.ChangedAt
        };
    }
}
