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
        var result = await _auditLogService.GetAuditLogsAsync(parameters);
        return Ok(result);
    }

    [HttpGet("grouped")]
    public async Task<ActionResult<PagedResponse<GroupedAuditLogResponse>>> GetGroupedAuditLogs([FromQuery] GroupedAuditLogQueryParameters parameters)
    {
        var result = await _auditLogService.GetGroupedAuditLogsAsync(parameters);
        return Ok(result);
    }

    [HttpGet("books/{bookId}")]
    public async Task<ActionResult<PagedResponse<AuditLogResponse>>> GetAuditLogsByBook(int bookId, [FromQuery] AuditLogQueryParameters parameters)
    {
        parameters.BookId = bookId;

        var result = await _auditLogService.GetAuditLogsAsync(parameters);
        return Ok(result);
    }

    [HttpGet("books/{bookId}/grouped")]
    public async Task<ActionResult<PagedResponse<GroupedAuditLogResponse>>> GetGroupedAuditLogsByBook(int bookId, [FromQuery] GroupedAuditLogQueryParameters parameters)
    {
        parameters.BookId = bookId;

        var result = await _auditLogService.GetGroupedAuditLogsAsync(parameters);
        return Ok(result);
    }
}
