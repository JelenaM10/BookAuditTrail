
namespace BookAuditTrail;

public interface IBookService
{
    Task<BookResponse> CreateBookAsync(CreateBookRequest request);
    Task<BookResponse> UpdateBookAsync(int id, UpdateBookRequest request);
    Task<BookResponse?> GetBookByIdAsync(int id);
    Task<List<BookResponse>> GetAllBooksAsync();
}
