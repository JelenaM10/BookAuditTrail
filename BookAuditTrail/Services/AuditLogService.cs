using Microsoft.EntityFrameworkCore;

namespace BookAuditTrail;

public class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;

    public async Task<PagedResponse<AuditLogResponse>> GetAuditLogsAsync(AuditLogQueryParameters parameters)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<GroupedAuditLogResponse>> GetGroupedAuditLogsAsync(AuditLogQueryParameters parameters)
    {
        throw new NotImplementedException();
    }
}
