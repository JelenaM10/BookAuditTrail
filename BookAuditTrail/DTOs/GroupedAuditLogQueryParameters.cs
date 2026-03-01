namespace BookAuditTrail;

public class GroupedAuditLogQueryParameters : IAuditLogFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Filtering
    public int? BookId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    // Ordering
    public OrderByField OrderBy { get; set; } = OrderByField.ChangedAt;
    public bool Descending { get; set; } = true;

    // Grouping
    public GroupByField? GroupBy { get; set; }
}
