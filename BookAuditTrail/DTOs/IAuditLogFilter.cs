namespace BookAuditTrail
{
    public interface IAuditLogFilter
    {
        int? BookId { get; }
        DateTime? FromDate { get; }
        DateTime? ToDate { get; }
    }
}
