using Microsoft.AspNetCore.Mvc;

namespace BookAuditTrail;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IBookService bookService) : ControllerBase
{
    private readonly IBookService _bookService = bookService;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<BookResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var books = await _bookService.GetAllBooksAsync(pageNumber, pageSize);
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponse>> GetById(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create([FromBody] CreateBookRequest request)
    {
        var book = await _bookService.CreateBookAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookResponse>> Update(int id, [FromBody] UpdateBookRequest request)
    {
        try
        {
            var book = await _bookService.UpdateBookAsync(id, request);
            return Ok(book);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
