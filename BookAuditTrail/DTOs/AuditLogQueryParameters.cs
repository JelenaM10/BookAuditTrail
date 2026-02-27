namespace BookAuditTrail;

public class AuditLogQueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

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
