using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Book;
using LibraryManagement.Application.Features.Books.Queries;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Handlers;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBookByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
        {
            return null;
        }

        var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId, cancellationToken);
        var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId, cancellationToken);

        var copies = await _unitOfWork.BookCopies.GetAsync(new BookCopiesByBookSpecification(book.Id), cancellationToken);
        var availableCopies = copies.Count(c => c.Status == BookCopyStatus.Available);

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Description = book.Description,
            Publisher = book.Publisher,
            PublishedYear = book.PublishedYear,
            Language = book.Language,
            ImageUrl = book.ImageUrl,
            AuthorName = author?.Name ?? string.Empty,
            CategoryName = category?.Name ?? string.Empty,
            TotalCopies = copies.Count,
            AvailableCopies = availableCopies,
            CreatedAt = book.CreatedAt.DateTime
        };
    }
}

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, PaginatedResult<BookDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllBooksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAllBooksSpecification(request.PageNumber, request.PageSize);
        var books = await _unitOfWork.Books.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.Books.CountAsync(new GetAllBooksSpecification(), cancellationToken);

        var bookDtos = new List<BookDto>();
        foreach (var book in books)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId, cancellationToken);
            var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId, cancellationToken);

            var copies = await _unitOfWork.BookCopies.GetAsync(new BookCopiesByBookSpecification(book.Id), cancellationToken);
            var availableCopies = copies.Count(c => c.Status == BookCopyStatus.Available);

            bookDtos.Add(new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                Publisher = book.Publisher,
                PublishedYear = book.PublishedYear,
                Language = book.Language,
                ImageUrl = book.ImageUrl,
                AuthorName = author?.Name ?? string.Empty,
                CategoryName = category?.Name ?? string.Empty,
                TotalCopies = copies.Count,
                AvailableCopies = availableCopies,
                CreatedAt = book.CreatedAt.DateTime
            });
        }

        return new PaginatedResult<BookDto>
        {
            Items = bookDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, PaginatedResult<BookSearchResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchBooksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<BookSearchResponse>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        var spec = new SearchBooksSpecification(
            request.Title,
            request.Author,
            request.ISBN,
            request.CategoryId,
            request.PageNumber,
            request.PageSize
        );

        var books = await _unitOfWork.Books.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.Books.CountAsync(
            new SearchBooksSpecification(request.Title, request.Author, request.ISBN, request.CategoryId),
            cancellationToken
        );

        var results = new List<BookSearchResponse>();
        foreach (var book in books)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId, cancellationToken);
            var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId, cancellationToken);

            var copies = await _unitOfWork.BookCopies.GetAsync(new BookCopiesByBookSpecification(book.Id), cancellationToken);
            var availableCopies = copies.Count(c => c.Status == BookCopyStatus.Available);

            results.Add(new BookSearchResponse
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                AuthorName = author?.Name ?? string.Empty,
                CategoryName = category?.Name ?? string.Empty,
                AvailableCopies = availableCopies
            });
        }

        return new PaginatedResult<BookSearchResponse>
        {
            Items = results,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetAvailableBooksQueryHandler : IRequestHandler<GetAvailableBooksQuery, PaginatedResult<BookSearchResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAvailableBooksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<BookSearchResponse>> Handle(GetAvailableBooksQuery request, CancellationToken cancellationToken)
    {
        var spec = new AvailableBooksSpecification(request.PageNumber, request.PageSize);
        var books = await _unitOfWork.Books.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.Books.CountAsync(new AvailableBooksSpecification(), cancellationToken);

        var results = new List<BookSearchResponse>();
        foreach (var book in books)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId, cancellationToken);
            var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId, cancellationToken);

            var copies = await _unitOfWork.BookCopies.GetAsync(new BookCopiesByBookSpecification(book.Id), cancellationToken);
            var availableCopies = copies.Count(c => c.Status == BookCopyStatus.Available);

            results.Add(new BookSearchResponse
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                AuthorName = author?.Name ?? string.Empty,
                CategoryName = category?.Name ?? string.Empty,
                AvailableCopies = availableCopies
            });
        }

        return new PaginatedResult<BookSearchResponse>
        {
            Items = results,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}