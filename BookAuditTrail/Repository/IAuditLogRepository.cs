
namespace BookAuditTrail;

public interface IAuditLogRepository
{
    IQueryable<BookAuditLog> GetQueryable();
    void AddRange(IEnumerable<BookAuditLog> logs);
}
