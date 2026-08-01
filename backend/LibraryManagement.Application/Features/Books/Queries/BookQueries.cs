using LibraryManagement.Application.DTOs.Book;
using LibraryManagement.Domain.Common;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries;

public record GetBookByIdQuery(Guid Id) : IRequest<BookDto?>;

public record GetAllBooksQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<BookDto>>;

public record SearchBooksQuery(
    string? Title = null,
    string? Author = null,
    string? ISBN = null,
    Guid? CategoryId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedResult<BookSearchResponse>>;

public record GetAvailableBooksQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<BookSearchResponse>>;