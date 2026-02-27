using Microsoft.AspNetCore.Mvc;

namespace BookAuditTrail.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController(IAuditLogService auditLogService) : ControllerBase
{
    private readonly IAuditLogService _auditLogService = auditLogService;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<AuditLogResponse>>> GetAuditLogs([FromQuery] AuditLogQueryParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.GroupBy))
        {
            var result = await _auditLogService.GetAuditLogsAsync(parameters);
            return Ok(result);
        }

        var grouped = await _auditLogService.GetGroupedAuditLogsAsync(parameters);
        return Ok(grouped);
    }

    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<PagedResponse<AuditLogResponse>>> GetAuditLogsByBook(int bookId, [FromQuery] AuditLogQueryParameters parameters)
    {
        parameters.BookId = bookId;

        if (string.IsNullOrWhiteSpace(parameters.GroupBy))
        {
            var result = await _auditLogService.GetAuditLogsAsync(parameters);
            return Ok(result);
        }

        var grouped = await _auditLogService.GetGroupedAuditLogsAsync(parameters);
        return Ok(grouped);
    }
}
