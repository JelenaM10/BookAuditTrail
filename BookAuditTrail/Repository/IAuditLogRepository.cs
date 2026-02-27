
namespace BookAuditTrail;

public interface IAuditLogRepository
{
    Task AddRangeAsync(IEnumerable<BookAuditLog> logs);
    void StageRange(IEnumerable<BookAuditLog> logs);
    IQueryable<BookAuditLog> GetQueryable();
}
