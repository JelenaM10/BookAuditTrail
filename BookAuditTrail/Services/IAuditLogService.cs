
namespace BookAuditTrail;

public interface IAuditLogService
{
    Task<PagedResponse<AuditLogResponse>> GetAuditLogsAsync(AuditLogQueryParameters parameters);
    Task<PagedResponse<GroupedAuditLogResponse>> GetGroupedAuditLogsAsync(AuditLogQueryParameters parameters);
}
