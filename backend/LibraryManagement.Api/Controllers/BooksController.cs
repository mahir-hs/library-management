namespace LibraryManagement.API.Controllers;

using Application.DTOs.Book;
using Application.Features.Books.Commands;
using Application.Features.Books.Queries;
using LibraryManagement.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Book management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class BooksController : BaseController
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    /// <remarks>Requires Admin or Librarian role</remarks>
    [HttpPost]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBook(
        [FromBody] CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBookCommand(
            request.Title, request.ISBN, request.Description,
            request.Publisher, request.PublishedYear, request.Language,
            request.ImageUrl, request.AuthorId, request.CategoryId
        );
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBookById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Add copies of a book to a specific branch
    /// </summary>
    /// <remarks>Requires Admin or Librarian role</remarks>
    [HttpPost("{id:guid}/copies")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddBookCopies(
        Guid id,
        [FromBody] AddBookCopiesRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddBookCopiesCommand(id, request.Quantity, request.BranchId, request.ShelfLocation);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBookById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a book by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookById(
        Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBookByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get all books with pagination
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResult<BookDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBooks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBooksQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search books by title, author, ISBN, or category
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResult<BookSearchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchBooks(
        [FromQuery] string? title,
        [FromQuery] string? author,
        [FromQuery] string? isbn,
        [FromQuery] Guid? categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchBooksQuery(title, author, isbn, categoryId, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get available books (with at least one available copy)
    /// </summary>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResult<BookSearchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableBooks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAvailableBooksQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update a book
    /// </summary>
    /// <remarks>Requires Admin or Librarian role</remarks>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBook(
        Guid id,
        [FromBody] UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBookCommand(id, request.Title, request.Description,
            request.Publisher, request.PublishedYear, request.Language,
            request.ImageUrl, request.CategoryId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete a book
    /// </summary>
    /// <remarks>Requires Admin role</remarks>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteBook(
        Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteBookCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}