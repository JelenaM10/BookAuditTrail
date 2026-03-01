
namespace BookAuditTrail;

public class AuditLogRepository(BookAuditTrailDbContext context) : IAuditLogRepository
{
    private readonly BookAuditTrailDbContext _context = context;

    public void AddRange(IEnumerable<BookAuditLog> logs)
    {
        _context.BookAuditLogs.AddRange(logs);
    }

    public void StageRange(IEnumerable<BookAuditLog> logs)
    {
        _context.BookAuditLogs.AddRange(logs);
    }

    public IQueryable<BookAuditLog> GetQueryable()
    {
        return _context.BookAuditLogs.AsQueryable();
    }
}
