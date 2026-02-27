using BookAuditTrail.Models;

namespace BookAuditTrail;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public ICollection<BookAuditLog> AuditLogs { get; set; } = new List<BookAuditLog>();
}
