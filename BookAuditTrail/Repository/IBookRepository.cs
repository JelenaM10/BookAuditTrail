
namespace BookAuditTrail;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id);
    Task<List<Book>> GetAllAsync();
    Task<Book> AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task<Author?> GetAuthorByNameAsync(string name);
    Task<Author> AddAuthorAsync(Author author);
    Task<List<Author>> GetAuthorsByNamesAsync(IEnumerable<string> names);
    Task AddAuthorsAsync(IEnumerable<Author> authors);
}
