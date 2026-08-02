using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Borrow;
using LibraryManagement.Application.Features.Borrows.Commands;
using LibraryManagement.Application.Features.Borrows.Queries;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using MediatR;

namespace LibraryManagement.Application.Features.Borrows.Handlers;

public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public BorrowBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BorrowDto> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(request.MemberId, cancellationToken);
        if (member is null)
        {
            throw new NotFoundException("Member", request.MemberId);
        }

        var user = await _unitOfWork.Users.GetByIdAsync(member.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedException("Member account is not active");
        }

        var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(request.BookCopyId, cancellationToken);
        if (bookCopy is null)
        {
            throw new NotFoundException("BookCopy", request.BookCopyId);
        }

        if (bookCopy.Status != BookCopyStatus.Available)
        {
            throw new ConflictException("Book copy is not available for borrowing");
        }

        var book = await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId, cancellationToken);
        if (book is null)
        {
            throw new NotFoundException("Book", bookCopy.BookId);
        }

        // Check if member has overdue borrows
        var overdueSpec = new OverdueBorrowsByMemberSpecification(request.MemberId);
        var overdueCount = await _unitOfWork.BorrowRecords.CountAsync(overdueSpec, cancellationToken);
        if (overdueCount > 0)
        {
            throw new ConflictException("Member has overdue borrows");
        }

        // Check if member has reached the maximum number of active borrows (5)
        var activeBorrowsSpec = new ActiveBorrowsByMemberSpecification(request.MemberId);
        var activeBorrowsCount = await _unitOfWork.BorrowRecords.CountAsync(activeBorrowsSpec, cancellationToken);
        if (activeBorrowsCount >= 5)
        {
            throw new ConflictException("Member has reached the maximum number of active borrows (5)");
        }

        var borrowRecord = new BorrowRecord
        {
            MemberId = request.MemberId,
            BookCopyId = request.BookCopyId,
            BorrowedAt = DateTimeOffset.UtcNow,
            DueDate = DateTimeOffset.UtcNow.AddDays(14),
            Status = BorrowStatus.Borrowed,
            FineAmount = 0
        };

        bookCopy.Status = BookCopyStatus.Borrowed;

        await _unitOfWork.BorrowRecords.AddAsync(borrowRecord, cancellationToken);
        await _unitOfWork.BookCopies.UpdateAsync(bookCopy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BorrowDto
        {
            Id = borrowRecord.Id,
            MemberId = borrowRecord.MemberId,
            MemberName = user.FullName,
            BookCopyId = borrowRecord.BookCopyId,
            BookTitle = book.Title,
            BookISBN = book.ISBN,
            BorrowedAt = borrowRecord.BorrowedAt.DateTime,
            DueDate = borrowRecord.DueDate.DateTime,
            ReturnedAt = null,
            Status = BorrowStatus.Borrowed,
            FineAmount = 0,
            DaysOverdue = 0
        };
    }
}

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, BorrowDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReturnBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BorrowDto> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        var borrowRecord = await _unitOfWork.BorrowRecords.GetByIdAsync(request.Id, cancellationToken);
        if (borrowRecord is null)
        {
            throw new NotFoundException("BorrowRecord", request.Id);
        }

        if (borrowRecord.Status == BorrowStatus.Returned)
        {
            throw new ConflictException("Borrow record already returned");
        }

        borrowRecord.ReturnedAt = DateTimeOffset.UtcNow;
        borrowRecord.Status = BorrowStatus.Returned;
        borrowRecord.FineAmount = request.FineAmount ?? 0;

        var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(borrowRecord.BookCopyId, cancellationToken);
        if (bookCopy is not null)
        {
            bookCopy.Status = BookCopyStatus.Available;
            await _unitOfWork.BookCopies.UpdateAsync(bookCopy, cancellationToken);
        }

        await _unitOfWork.BorrowRecords.UpdateAsync(borrowRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var book = await _unitOfWork.Books.GetByIdAsync(bookCopy!.BookId, cancellationToken);
        var member = await _unitOfWork.Members.GetByIdAsync(borrowRecord.MemberId, cancellationToken);
        var user = member is not null ? await _unitOfWork.Users.GetByIdAsync(member.UserId, cancellationToken) : null;

        return new BorrowDto
        {
            Id = borrowRecord.Id,
            MemberId = borrowRecord.MemberId,
            MemberName = user?.FullName ?? string.Empty,
            BookCopyId = borrowRecord.BookCopyId,
            BookTitle = book?.Title ?? string.Empty,
            BookISBN = book?.ISBN ?? string.Empty,
            BorrowedAt = borrowRecord.BorrowedAt.DateTime,
            DueDate = borrowRecord.DueDate.DateTime,
            ReturnedAt = borrowRecord.ReturnedAt?.DateTime,
            Status = BorrowStatus.Returned,
            FineAmount = borrowRecord.FineAmount,
            DaysOverdue = borrowRecord.ReturnedAt.HasValue
                ? Math.Max(0, (borrowRecord.ReturnedAt.Value - borrowRecord.DueDate).Days)
                : 0
        };
    }
}