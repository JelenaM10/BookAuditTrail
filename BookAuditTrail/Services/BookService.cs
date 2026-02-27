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

    public async Task<BookResponse> UpdateBookAsync(int id, UpdateBookRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(id)
         ?? throw new KeyNotFoundException($"Book with id {id} not found");

        book.Title = request.Title ?? book.Title;
        book.ShortDescription = request.ShortDescription ?? book.ShortDescription;
        book.PublishDate = request.PublishDate ?? book.PublishDate;

        if (request.Authors is not null)
        {
            var current = book.Authors.Select(a => a.Name).ToHashSet();
            var incoming = request.Authors.ToHashSet();

            var toAdd = incoming.Except(current).ToList();
            var toRemove = current.Except(incoming).ToList();

            foreach (var author in book.Authors.Where(a => toRemove.Contains(a.Name)).ToList())
                book.Authors.Remove(author);

            if (toAdd.Any())
            {
                var existingAuthors = await _bookRepository.GetAuthorsByNamesAsync(toAdd);
                var existingNames = existingAuthors.Select(a => a.Name).ToHashSet();

                foreach (var author in existingAuthors)
                    book.Authors.Add(author);

                var missingNames = toAdd.Except(existingNames).ToList();
                if (missingNames.Any())
                {
                    var newAuthors = missingNames.Select(name => new Author { Name = name }).ToList();
                    await _bookRepository.AddAuthorsAsync(newAuthors);
                    ((List<Author>)book.Authors).AddRange(newAuthors);
                }
            }
        }

        await _bookRepository.UpdateAsync(book);

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
