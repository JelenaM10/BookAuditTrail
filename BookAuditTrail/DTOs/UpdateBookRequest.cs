namespace BookAuditTrail;

public class UpdateBookRequest
{
    public string? Title { get; set; }
    public string? ShortDescription { get; set; }
    public DateTime? PublishDate { get; set; }
    public List<string>? Authors { get; set; }
}
