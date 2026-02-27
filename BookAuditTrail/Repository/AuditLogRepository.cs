
namespace BookAuditTrail;

public class AuditLogRepository(BookAuditTrailDbContext context) : IAuditLogRepository
{
    private readonly BookAuditTrailDbContext _context = context;

    public async Task AddRangeAsync(IEnumerable<BookAuditLog> logs)
    {
        _context.BookAuditLogs.AddRange(logs);
        await _context.SaveChangesAsync();
    }

    public IQueryable<BookAuditLog> GetQueryable()
    {
        return _context.BookAuditLogs.AsQueryable();
    }
}
