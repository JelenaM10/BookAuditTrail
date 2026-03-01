
namespace BookAuditTrail;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id);
    Task<List<Book>> GetAllAsync();
    Task<(List<Book> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<List<Author>> GetAuthorsByNamesAsync(IEnumerable<string> names);
    Task AddAuthorsAsync(IEnumerable<Author> authors);
    Task SaveChangesAsync();
    void Add(Book book);
    void AddAuthors(IEnumerable<Author> authors);
}
