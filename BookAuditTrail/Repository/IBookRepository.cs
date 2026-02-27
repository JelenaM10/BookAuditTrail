
namespace BookAuditTrail;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id);
    Task<List<Book>> GetAllAsync();
    Task<Book> AddAsync(Book book);
    Task<Author?> GetAuthorByNameAsync(string name);
    Task<Author> AddAuthorAsync(Author author);
}
