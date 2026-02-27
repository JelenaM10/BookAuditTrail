using Microsoft.EntityFrameworkCore;

namespace BookAuditTrail;

public class BookRepository(BookAuditTrailDbContext context) : IBookRepository
{
    private readonly BookAuditTrailDbContext _context = context;

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await _context.Books
            .AsNoTracking()
            .Include(b => b.Authors)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<(List<Book> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = _context.Books
            .AsNoTracking()
            .Include(b => b.Authors)
            .OrderByDescending(b => b.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Book> AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task UpdateAsync(Book book)
    {
        book.UpdatedAt = DateTime.UtcNow;
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Author>> GetAuthorsByNamesAsync(IEnumerable<string> names)
    {
        return await _context.Authors
            .Where(a => names.Contains(a.Name))
            .ToListAsync();
    }

    public async Task<Author?> GetAuthorByNameAsync(string name)
    {
        return await _context.Authors
            .FirstOrDefaultAsync(a => a.Name == name);
    }

    public async Task<Author> AddAuthorAsync(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }

    public async Task AddAuthorsAsync(IEnumerable<Author> authors)
    {
        await _context.Authors.AddRangeAsync(authors);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
