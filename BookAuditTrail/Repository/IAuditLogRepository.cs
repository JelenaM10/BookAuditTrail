
namespace BookAuditTrail;

public interface IAuditLogRepository
{
    void StageRange(IEnumerable<BookAuditLog> logs);
    IQueryable<BookAuditLog> GetQueryable();
    void AddRange(IEnumerable<BookAuditLog> logs);
}
