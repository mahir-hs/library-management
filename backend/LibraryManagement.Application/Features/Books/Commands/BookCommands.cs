using LibraryManagement.Application.DTOs.Book;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands;

public record CreateBookCommand(
    string Title,
    string ISBN,
    string? Description,
    string? Publisher,
    int? PublishedYear,
    string? Language,
    string? ImageUrl,
    Guid AuthorId,
    Guid CategoryId
) : IRequest<BookDto>;

public record UpdateBookCommand(
    Guid Id,
    string Title,
    string? Description,
    string? Publisher,
    int? PublishedYear,
    string? Language,
    string? ImageUrl,
    Guid? CategoryId
) : IRequest<BookDto>;

public record DeleteBookCommand(Guid Id) : IRequest<bool>;

public record AddBookCopiesCommand(
    Guid BookId,
    int Quantity,
    Guid BranchId,
    string ShelfLocation
) : IRequest<BookDto>;