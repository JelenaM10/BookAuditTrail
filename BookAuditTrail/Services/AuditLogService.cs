using Microsoft.EntityFrameworkCore;

namespace BookAuditTrail;

public class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;

    public async Task<PagedResponse<AuditLogResponse>> GetAuditLogsAsync(AuditLogQueryParameters parameters)
    {
        var query = _auditLogRepository.GetQueryable().AsNoTracking().Include(a => a.Book).AsQueryable();

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

    public async Task<PagedResponse<GroupedAuditLogResponse>> GetGroupedAuditLogsAsync(GroupedAuditLogQueryParameters parameters)
    {
        var query = _auditLogRepository.GetQueryable().AsNoTracking().AsQueryable();

        query = ApplyFilters(query, parameters);

        var groupByField = parameters.GroupBy ?? GroupByField.BookId;

        if (groupByField == GroupByField.Date)
            return await GetGroupedByDateAsync(query, parameters);

        return await GetGroupedByBookIdAsync(query, parameters);
    }

    private static async Task<PagedResponse<GroupedAuditLogResponse>> GetGroupedByBookIdAsync(IQueryable<BookAuditLog> query, GroupedAuditLogQueryParameters parameters)
    {
        var groupQuery = query
        .GroupBy(a => a.BookId)
        .Select(g => new
        {
            BookId = g.Key,
            Count = g.Count(),
            Items = g.OrderByDescending(a => a.ChangedAt)
                     .Select(a => new
                     {
                         a.Id,
                         a.BookId,
                         BookTitle = a.Book.Title,
                         a.ChangeType,
                         a.FieldName,
                         a.OldValue,
                         a.NewValue,
                         a.Description,
                         a.ChangedAt
                     })
        });

        var totalCount = await groupQuery.CountAsync();

        var pagedGroups = await groupQuery
            .OrderByDescending(g => g.Count)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        var result = pagedGroups.Select(g => new GroupedAuditLogResponse
        {
            GroupKey = g.Items.FirstOrDefault()?.BookTitle ?? g.BookId.ToString(),
            Count = g.Count,
            Items = g.Items.Select(a => new AuditLogResponse
            {
                Id = a.Id,
                BookId = a.BookId,
                BookTitle = a.BookTitle,
                ChangeType = a.ChangeType,
                FieldName = a.FieldName,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                Description = a.Description,
                ChangedAt = a.ChangedAt
            }).ToList()
        }).ToList();

        return new PagedResponse<GroupedAuditLogResponse>
        {
            Items = result,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static async Task<PagedResponse<GroupedAuditLogResponse>> GetGroupedByDateAsync(IQueryable<BookAuditLog> query, GroupedAuditLogQueryParameters parameters)
    {
        var groupQuery = query
        .GroupBy(a => a.ChangedAt.Date)
        .Select(g => new
        {
            Date = g.Key,
            Count = g.Count(),
            Items = g.OrderByDescending(a => a.ChangedAt)
                     .Select(a => new
                     {
                         a.Id,
                         a.BookId,
                         BookTitle = a.Book.Title,
                         a.ChangeType,
                         a.FieldName,
                         a.OldValue,
                         a.NewValue,
                         a.Description,
                         a.ChangedAt
                     })
        });

        var totalCount = await groupQuery.CountAsync();

        var pagedGroups = await groupQuery
            .OrderByDescending(g => g.Date)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        var result = pagedGroups.Select(g => new GroupedAuditLogResponse
        {
            GroupKey = g.Date.ToString("yyyy-MM-dd"),
            Count = g.Count,
            Items = g.Items.Select(a => new AuditLogResponse
            {
                Id = a.Id,
                BookId = a.BookId,
                BookTitle = a.BookTitle,
                ChangeType = a.ChangeType,
                FieldName = a.FieldName,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                Description = a.Description,
                ChangedAt = a.ChangedAt
            }).ToList()
        }).ToList();

        return new PagedResponse<GroupedAuditLogResponse>
        {
            Items = result,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<BookAuditLog> ApplyFilters(IQueryable<BookAuditLog> query, IAuditLogFilter parameters)
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
        query = parameters.OrderBy switch
        {
            OrderByField.ChangedAt => parameters.Descending
                ? query.OrderByDescending(a => a.ChangedAt)
                : query.OrderBy(a => a.ChangedAt),
            OrderByField.BookId => parameters.Descending
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
