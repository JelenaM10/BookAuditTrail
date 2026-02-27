namespace BookAuditTrail;

public class BookService(IBookRepository bookRepository) : IBookService
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<BookResponse> CreateBookAsync(CreateBookRequest request)
    {
        var book = new Book
        {
            Title = request.Title,
            ShortDescription = request.ShortDescription,
            PublishDate = request.PublishDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var authorName in request.Authors)
        {
            var author = await _bookRepository.GetAuthorByNameAsync(authorName);
            if (author == null)
            {
                author = new Author { Name = authorName };
                author = await _bookRepository.AddAuthorAsync(author);
            }
            book.Authors.Add(author);
        }

        book = await _bookRepository.AddAsync(book);

        return MapToResponse(book);
    }

    public async Task<BookResponse?> GetBookByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book == null ? null : MapToResponse(book);
    }

    public async Task<List<BookResponse>> GetAllBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(MapToResponse).ToList();
    }

    private static BookResponse MapToResponse(Book book)
    {
        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            ShortDescription = book.ShortDescription,
            PublishDate = book.PublishDate,
            Authors = book.Authors.Select(a => a.Name).ToList(),
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt
        };
    }
}
