namespace BookAuditTrail;

public class AuditLogResponse
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
}
