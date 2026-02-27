namespace BookAuditTrail;

public class GroupedAuditLogResponse
{
    public string GroupKey { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<AuditLogResponse> Items { get; set; } = [];
}
