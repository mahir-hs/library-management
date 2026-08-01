using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Book;
using LibraryManagement.Application.Features.Books.Commands;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Handlers;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // Check if ISBN already exists
        var existingBookSpec = new BookByISBNSpecification(request.ISBN);
        var existingBook = await _unitOfWork.Books.GetFirstAsync(existingBookSpec, cancellationToken);

        if (existingBook is not null)
        {
            throw new ConflictException($"Book with ISBN '{request.ISBN}' already exists");
        }

        // Verify author exists
        var author = await _unitOfWork.Authors.GetByIdAsync(request.AuthorId, cancellationToken);
        if (author is null)
        {
            throw new NotFoundException("Author", request.AuthorId);
        }

        // Verify category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Category", request.CategoryId);
        }

        var book = new Book
        {
            Title = request.Title,
            ISBN = request.ISBN,
            Description = request.Description,
            Publisher = request.Publisher,
            PublishedYear = request.PublishedYear,
            Language = request.Language,
            ImageUrl = request.ImageUrl,
            AuthorId = request.AuthorId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _unitOfWork.Books.AddAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
            AuthorName = author.Name,
            CategoryName = category.Name,
            TotalCopies = 0,
            AvailableCopies = 0,
            CreatedAt = book.CreatedAt.DateTime
        };
    }
}

public class AddBookCopiesCommandHandler : IRequestHandler<AddBookCopiesCommand, BookDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddBookCopiesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BookDto> Handle(AddBookCopiesCommand request, CancellationToken cancellationToken)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            throw new NotFoundException("Book", request.BookId);
        }

        if (request.Quantity <= 0)
        {
            throw new ValidationException("Quantity must be greater than zero");
        }

        var branch = await _unitOfWork.Branchs.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch", request.BranchId);
        }

        // Find the max existing barcode number to generate unique barcodes
        var allCopies = await _unitOfWork.BookCopies.GetAsync(new BookCopiesByBookSpecification(request.BookId), cancellationToken);
        int maxBarcodeNumber = 0;
        foreach (var copy in allCopies)
        {
            if (copy.Barcode.StartsWith("BK-CP-") && copy.Barcode.Length > 6)
            {
                if (int.TryParse(copy.Barcode.Substring(6), out int num) && num > maxBarcodeNumber)
                {
                    maxBarcodeNumber = num;
                }
            }
        }

        var newCopies = new List<BookCopy>();
        for (int i = 1; i <= request.Quantity; i++)
        {
            maxBarcodeNumber++;
            newCopies.Add(new BookCopy
            {
                BookId = request.BookId,
                Barcode = $"BK-CP-{maxBarcodeNumber:D5}",
                Status = BookCopyStatus.Available,
                ShelfLocation = request.ShelfLocation,
                AcquiredDate = DateTimeOffset.UtcNow,
                BranchId = request.BranchId,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        await _unitOfWork.BookCopies.AddRangeAsync(newCopies, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload copies to get the full count including newly added ones
        var updatedCopies = await _unitOfWork.BookCopies.GetAsync(new BookCopiesByBookSpecification(request.BookId), cancellationToken);
        var availableCopies = updatedCopies.Count(c => c.Status == BookCopyStatus.Available);

        // Load author and category for response
        var author = await _unitOfWork.Authors.GetByIdAsync(book.AuthorId, cancellationToken);
        var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId, cancellationToken);

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
            TotalCopies = updatedCopies.Count,
            AvailableCopies = availableCopies,
            CreatedAt = book.CreatedAt.DateTime
        };
    }
}

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
        {
            throw new NotFoundException("Book", request.Id);
        }

        // Verify category exists if provided
        if (request.CategoryId.HasValue)
        {
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (existingCategory is null)
            {
                throw new NotFoundException("Category", request.CategoryId.Value);
            }
            book.CategoryId = request.CategoryId.Value;
        }

        book.Title = request.Title;
        book.Description = request.Description;
        book.Publisher = request.Publisher;
        book.PublishedYear = request.PublishedYear;
        book.Language = request.Language;
        book.ImageUrl = request.ImageUrl;
        book.UpdatedAt = DateTimeOffset.UtcNow;

        await _unitOfWork.Books.UpdateAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load author and category for response
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

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
        {
            throw new NotFoundException("Book", request.Id);
        }

        // Check if book has copies
        var copiesSpec = new BookCopiesByBookSpecification(request.Id);
        var copies = await _unitOfWork.BookCopies.GetAsync(copiesSpec, cancellationToken);
        if (copies.Count > 0)
        {
            throw new ConflictException("Cannot delete book with existing copies. Remove copies first.");
        }

        await _unitOfWork.Books.DeleteAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}