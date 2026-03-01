namespace BookAuditTrail;

public class BookService(IBookRepository bookRepository, IAuditLogRepository auditLogRepository) : IBookService
{
    private readonly IBookRepository _bookRepository = bookRepository;
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;

    public async Task<BookResponse> CreateBookAsync(CreateBookRequest request)
    {
        // TODO: Consider wrapping adding authors and the book in the same transaction / SaveChangesAsync

        var now = DateTime.UtcNow;

        var book = new Book
        {
            Title = request.Title,
            ShortDescription = request.ShortDescription,
            PublishDate = request.PublishDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        var existingAuthors = await _bookRepository.GetAuthorsByNamesAsync(request.Authors);
        var existingNames = existingAuthors.Select(a => a.Name).ToHashSet();

        var newAuthors = request.Authors
            .Where(name => !existingNames.Contains(name))
            .Select(name => new Author { Name = name })
            .ToList();

        if (newAuthors.Any())
            await _bookRepository.AddAuthorsAsync(newAuthors);

        foreach (var author in existingAuthors.Concat(newAuthors))
            book.Authors.Add(author);

        book = await _bookRepository.AddAsync(book);

        var auditLogs = new List<BookAuditLog>
        {
            new()
            {
                BookId = book.Id,
                ChangeType = "Created",
                FieldName = "Book",
                NewValue = book.Title,
                Description = $"Book \"{book.Title}\" was created",
                ChangedAt = now
            }
        };

        if (book.Authors.Any())
        {
            var authorNames = string.Join(", ", book.Authors.Select(a => a.Name));
            auditLogs.Add(new BookAuditLog
            {
                BookId = book.Id,
                ChangeType = "Created",
                FieldName = "Authors",
                NewValue = authorNames,
                Description = $"Authors set to: {authorNames}",
                ChangedAt = now
            });
        }

        await _auditLogRepository.AddRangeAsync(auditLogs);

        return MapToResponse(book);
    }

    public async Task<BookResponse> UpdateBookAsync(int id, UpdateBookRequest request)
    {
        // TODO: Consider wrapping book updates, author changes, and audit logs in the same transaction / SaveChangesAsync

        var book = await _bookRepository.GetByIdAsync(id)
         ?? throw new KeyNotFoundException($"Book with id {id} not found");

        var auditLogs = new List<BookAuditLog>();
        var now = DateTime.UtcNow;

        if (request.Title != null && request.Title != book.Title)
        {
            auditLogs.Add(new BookAuditLog
            {
                BookId = book.Id,
                ChangeType = "Updated",
                FieldName = "Title",
                OldValue = book.Title,
                NewValue = request.Title,
                Description = $"Title was changed to \"{request.Title}\"",
                ChangedAt = now
            });
            book.Title = request.Title;
        }

        if (request.ShortDescription != null && request.ShortDescription != book.ShortDescription)
        {
            auditLogs.Add(new BookAuditLog
            {
                BookId = book.Id,
                ChangeType = "Updated",
                FieldName = "ShortDescription",
                OldValue = book.ShortDescription,
                NewValue = request.ShortDescription,
                Description = $"Short description was changed to \"{request.ShortDescription}\"",
                ChangedAt = now
            });
            book.ShortDescription = request.ShortDescription;
        }

        if (request.PublishDate.HasValue && request.PublishDate.Value != book.PublishDate)
        {
            auditLogs.Add(new BookAuditLog
            {
                BookId = book.Id,
                ChangeType = "Updated",
                FieldName = "PublishDate",
                OldValue = book.PublishDate.ToString("yyyy-MM-dd"),
                NewValue = request.PublishDate.Value.ToString("yyyy-MM-dd"),
                Description = $"Publish date was changed to \"{request.PublishDate.Value:yyyy-MM-dd}\"",
                ChangedAt = now
            });
            book.PublishDate = request.PublishDate.Value;
        }

        if (request.Authors is not null)
        {
            var authorsAudit = await UpdateAuthorsAsync(book, request.Authors, now);
            auditLogs.AddRange(authorsAudit);
        }

        if (auditLogs.Any())
        {
            book.UpdatedAt = now;
            _auditLogRepository.StageRange(auditLogs);
            await _bookRepository.SaveChangesAsync();
        }

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

    public async Task<PagedResponse<BookResponse>> GetAllBooksAsync(int pageNumber, int pageSize)
    {
        var (books, totalCount) = await _bookRepository.GetAllAsync(pageNumber, pageSize);
        return new PagedResponse<BookResponse>
        {
            Items = books.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
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

    private async Task<List<BookAuditLog>> UpdateAuthorsAsync(Book book, IEnumerable<string> newAuthorNames, DateTime now)
    {
        var logs = new List<BookAuditLog>();
        var currentNames = book.Authors.Select(a => a.Name).ToHashSet();
        var incoming = newAuthorNames.ToHashSet();

        var toAdd = incoming.Except(currentNames).ToList();
        var toRemove = currentNames.Except(incoming).ToList();

        foreach (var author in book.Authors.Where(a => toRemove.Contains(a.Name)).ToList())
        {
            book.Authors.Remove(author);
            logs.Add(new BookAuditLog
            {
                BookId = book.Id,
                ChangeType = "AuthorRemoved",
                FieldName = "Authors",
                OldValue = author.Name,
                Description = $"Author \"{author.Name}\" was removed",
                ChangedAt = now
            });
        }

        var existingAuthors = await _bookRepository.GetAuthorsByNamesAsync(toAdd);
        var existingNames = existingAuthors.Select(a => a.Name).ToHashSet();

        foreach (var author in existingAuthors)
        {
            book.Authors.Add(author);

            logs.Add(new BookAuditLog
            {
                BookId = book.Id,
                ChangeType = "AuthorAdded",
                FieldName = "Authors",
                NewValue = author.Name,
                Description = $"Author \"{author.Name}\" was added",
                ChangedAt = now
            });
        }

        var missingNames = toAdd.Except(existingNames).ToList();
        if (missingNames.Any())
        {
            var newAuthors = missingNames.Select(name => new Author { Name = name }).ToList();
            await _bookRepository.AddAuthorsAsync(newAuthors);

            foreach (var author in newAuthors)
            {
                book.Authors.Add(author);

                logs.Add(new BookAuditLog
                {
                    BookId = book.Id,
                    ChangeType = "AuthorAdded",
                    FieldName = "Authors",
                    NewValue = author.Name,
                    Description = $"Author \"{author.Name}\" was added",
                    ChangedAt = now
                });
            }
        }

        return logs;
    }
}
