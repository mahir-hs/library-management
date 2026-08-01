using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Borrow;
using LibraryManagement.Application.Features.Borrows.Queries;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using MediatR;

namespace LibraryManagement.Application.Features.Borrows.Handlers;

public class GetBorrowByIdQueryHandler : IRequestHandler<GetBorrowByIdQuery, BorrowDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBorrowByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BorrowDto?> Handle(GetBorrowByIdQuery request, CancellationToken cancellationToken)
    {
        var borrowRecord = await _unitOfWork.BorrowRecords.GetByIdAsync(request.Id, cancellationToken);
        if (borrowRecord is null)
        {
            return null;
        }

        return BorrowRecordMappers.MapToDto(borrowRecord);
    }
}

public class GetBorrowsByMemberQueryHandler : IRequestHandler<GetBorrowsByMemberQuery, PaginatedResult<BorrowDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBorrowsByMemberQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<BorrowDto>> Handle(GetBorrowsByMemberQuery request, CancellationToken cancellationToken)
    {
        var spec = new BorrowRecordsByMemberSpecification(request.MemberId, request.PageNumber, request.PageSize);
        var records = await _unitOfWork.BorrowRecords.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.BorrowRecords.CountAsync(
            new BorrowRecordsByMemberSpecification(request.MemberId), cancellationToken);

        var dtos = new List<BorrowDto>();
        foreach (var record in records)
        {
            dtos.Add(BorrowRecordMappers.MapToDto(record));
        }

        return new PaginatedResult<BorrowDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetBorrowsByBookCopyQueryHandler : IRequestHandler<GetBorrowsByBookCopyQuery, PaginatedResult<BorrowDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBorrowsByBookCopyQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<BorrowDto>> Handle(GetBorrowsByBookCopyQuery request, CancellationToken cancellationToken)
    {
        var spec = new BorrowRecordsByBookCopySpecification(request.BookCopyId, request.PageNumber, request.PageSize);
        var records = await _unitOfWork.BorrowRecords.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.BorrowRecords.CountAsync(
            new BorrowRecordsByBookCopySpecification(request.BookCopyId), cancellationToken);

        var dtos = new List<BorrowDto>();
        foreach (var record in records)
        {
            dtos.Add(BorrowRecordMappers.MapToDto(record));
        }

        return new PaginatedResult<BorrowDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetMyBorrowsQueryHandler : IRequestHandler<GetMyBorrowsQuery, PaginatedResult<MyBorrowsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyBorrowsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResult<MyBorrowsResponse>> Handle(GetMyBorrowsQuery request, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Members.GetFirstAsync(
            new MemberByUserIdSpecification(_currentUserService.UserId), cancellationToken);

        if (member is null)
        {
            return new PaginatedResult<MyBorrowsResponse>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var spec = new BorrowRecordsByMemberSpecification(member.Id, request.PageNumber, request.PageSize);
        var records = await _unitOfWork.BorrowRecords.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.BorrowRecords.CountAsync(
            new BorrowRecordsByMemberSpecification(member.Id), cancellationToken);

        var dtos = new List<MyBorrowsResponse>();
        foreach (var record in records)
        {
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(record.BookCopyId, cancellationToken);
            var book = bookCopy is not null ? await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId, cancellationToken) : null;
            var author = book is not null ? await _unitOfWork.Authors.GetByIdAsync(book.AuthorId, cancellationToken) : null;

            dtos.Add(new MyBorrowsResponse
            {
                Id = record.Id,
                BookTitle = book?.Title ?? string.Empty,
                BookISBN = book?.ISBN ?? string.Empty,
                AuthorName = author?.Name ?? string.Empty,
                BorrowedAt = record.BorrowedAt.DateTime,
                DueDate = record.DueDate.DateTime,
                ReturnedAt = record.ReturnedAt?.DateTime,
                Status = record.Status
            });
        }

        return new PaginatedResult<MyBorrowsResponse>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetOverdueBorrowsQueryHandler : IRequestHandler<GetOverdueBorrowsQuery, PaginatedResult<BorrowListResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOverdueBorrowsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<BorrowListResponse>> Handle(GetOverdueBorrowsQuery request, CancellationToken cancellationToken)
    {
        var spec = new OverdueBorrowsSpecification(request.PageNumber, request.PageSize);
        var records = await _unitOfWork.BorrowRecords.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.BorrowRecords.CountAsync(
            new OverdueBorrowsSpecification(), cancellationToken);

        var dtos = new List<BorrowListResponse>();
        foreach (var record in records)
        {
            dtos.Add(BorrowRecordMappers.MapToListResponse(record));
        }

        return new PaginatedResult<BorrowListResponse>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public static class BorrowRecordMappers
{
    public static BorrowDto MapToDto(BorrowRecord record)
    {
        // Navigation properties must be loaded: Member.User, BookCopy.Book
        return new BorrowDto
        {
            Id = record.Id,
            MemberId = record.MemberId,
            MemberName = record.Member.User.FullName,
            BookCopyId = record.BookCopyId,
            BookTitle = record.BookCopy.Book.Title,
            BookISBN = record.BookCopy.Book.ISBN,
            BorrowedAt = record.BorrowedAt.DateTime,
            DueDate = record.DueDate.DateTime,
            ReturnedAt = record.ReturnedAt?.DateTime,
            Status = record.Status,
            FineAmount = record.FineAmount,
            DaysOverdue = record.ReturnedAt.HasValue
                ? Math.Max(0, (record.ReturnedAt.Value - record.DueDate).Days)
                : (int)(DateTimeOffset.UtcNow - record.DueDate).TotalDays
        };
    }

    public static BorrowListResponse MapToListResponse(BorrowRecord record)
    {
        // Navigation properties must be loaded: Member.User, BookCopy.Book
        return new BorrowListResponse
        {
            Id = record.Id,
            MemberName = record.Member.User.FullName,
            BookTitle = record.BookCopy.Book.Title,
            BookISBN = record.BookCopy.Book.ISBN,
            BorrowedAt = record.BorrowedAt.DateTime,
            DueDate = record.DueDate.DateTime,
            Status = record.Status,
            DaysOverdue = record.ReturnedAt.HasValue
                ? 0
                : Math.Max(0, (int)(DateTimeOffset.UtcNow - record.DueDate).TotalDays)
        };
    }
}