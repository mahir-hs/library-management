using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Report;
using LibraryManagement.Application.Features.Reports.Queries;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;
using MediatR;

namespace LibraryManagement.Application.Features.Reports.Handlers;

public class GetReportSummaryQueryHandler : IRequestHandler<GetReportSummaryQuery, ReportSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReportSummaryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReportSummaryDto> Handle(GetReportSummaryQuery request, CancellationToken cancellationToken)
    {
        var allBooksSpec = new GetAllSpecification<Book>();
        var totalBooks = await _unitOfWork.Books.CountAsync(allBooksSpec, cancellationToken);

        var availableSpec = new BookCopiesByStatusSpecification(BookCopyStatus.Available);
        var borrowedSpec = new BookCopiesByStatusSpecification(BookCopyStatus.Borrowed);
        int totalAvailable = await _unitOfWork.BookCopies.CountAsync(availableSpec, cancellationToken);
        int totalBorrowed = await _unitOfWork.BookCopies.CountAsync(borrowedSpec, cancellationToken);

        var allMembersSpec = new GetAllSpecification<Member>();
        var totalMembers = await _unitOfWork.Members.CountAsync(allMembersSpec, cancellationToken);

        var borrowSpec = new BorrowRecordsByStatusSpecification(BorrowStatus.Borrowed);
        int activeBorrows = await _unitOfWork.BorrowRecords.CountAsync(borrowSpec, cancellationToken);

        var overdueSpec = new OverdueBorrowsSpecification();
        int overdueBorrows = await _unitOfWork.BorrowRecords.CountAsync(overdueSpec, cancellationToken);

        var pendingReservationSpec = new ReservationsByStatusSpecification(ReservationStatus.Pending);
        int pendingReservations = await _unitOfWork.Reservations.CountAsync(pendingReservationSpec, cancellationToken);

        var overdueRecordsSpec = new OverdueBorrowsSpecification();
        var overdueRecords = await _unitOfWork.BorrowRecords.GetAsync(overdueRecordsSpec, cancellationToken);
        decimal totalOutstandingFines = overdueRecords.Sum(r => r.FineAmount);

        return new ReportSummaryDto
        {
            TotalBooks = totalBooks,
            TotalAvailableCopies = totalAvailable,
            TotalBorrowedCopies = totalBorrowed,
            TotalMembers = totalMembers,
            ActiveBorrows = activeBorrows,
            OverdueBorrows = overdueBorrows,
            PendingReservations = pendingReservations,
            TotalOutstandingFines = totalOutstandingFines
        };
    }
}

public class GetBorrowHistoryQueryHandler : IRequestHandler<GetBorrowHistoryQuery, PaginatedResult<BorrowHistoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBorrowHistoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<BorrowHistoryDto>> Handle(GetBorrowHistoryQuery request, CancellationToken cancellationToken)
    {
        var spec = new BorrowHistorySpecification(request.FromDate, request.ToDate, request.PageNumber, request.PageSize);
        var records = await _unitOfWork.BorrowRecords.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.BorrowRecords.CountAsync(
            new BorrowHistorySpecification(request.FromDate, request.ToDate), cancellationToken);

        var dtos = new List<BorrowHistoryDto>();
        foreach (var record in records)
        {
            dtos.Add(await ReportMappers.MapToHistoryDtoAsync(record, _unitOfWork, cancellationToken));
        }

        return new PaginatedResult<BorrowHistoryDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetOverdueBooksQueryHandler : IRequestHandler<GetOverdueBooksQuery, PaginatedResult<OverdueBookDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOverdueBooksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<OverdueBookDto>> Handle(GetOverdueBooksQuery request, CancellationToken cancellationToken)
    {
        var spec = new OverdueBorrowsSpecification(request.PageNumber, request.PageSize);
        var records = await _unitOfWork.BorrowRecords.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.BorrowRecords.CountAsync(
            new OverdueBorrowsSpecification(), cancellationToken);

        var dtos = new List<OverdueBookDto>();
        foreach (var record in records)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(record.MemberId, cancellationToken);
            var user = member is not null ? await _unitOfWork.Users.GetByIdAsync(member.UserId, cancellationToken) : null;
            var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(record.BookCopyId, cancellationToken);
            var book = bookCopy is not null ? await _unitOfWork.Books.GetByIdAsync(bookCopy.BookId, cancellationToken) : null;

            dtos.Add(new OverdueBookDto
            {
                BorrowId = record.Id,
                MemberName = user?.FullName ?? string.Empty,
                BookTitle = book?.Title ?? string.Empty,
                ISBN = book?.ISBN ?? string.Empty,
                DueDate = record.DueDate.DateTime,
                DaysOverdue = (int)(DateTimeOffset.UtcNow - record.DueDate).TotalDays,
                EstimatedFine = (decimal)(int)(DateTimeOffset.UtcNow - record.DueDate).TotalDays * 0.50m
            });
        }

        return new PaginatedResult<OverdueBookDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetMemberActivityQueryHandler : IRequestHandler<GetMemberActivityQuery, MemberActivityDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMemberActivityQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberActivityDto> Handle(GetMemberActivityQuery request, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(request.MemberId, cancellationToken);
        if (member is null)
        {
            throw new LibraryManagement.Application.Common.Exceptions.NotFoundException("Member", request.MemberId);
        }

        var user = await _unitOfWork.Users.GetByIdAsync(member.UserId, cancellationToken);

        var borrowSpec = new BorrowRecordsByMemberSpecification(request.MemberId);
        var borrowRecords = await _unitOfWork.BorrowRecords.GetAsync(borrowSpec, cancellationToken);

        var reservationSpec = new ReservationsByMemberSpecification(request.MemberId);
        var reservations = await _unitOfWork.Reservations.GetAsync(reservationSpec, cancellationToken);

        int totalBorrows = borrowRecords.Count;
        int activeBorrows = borrowRecords.Count(br => br.Status == BorrowStatus.Borrowed);
        int overdueBorrows = borrowRecords.Count(br => br.Status == BorrowStatus.Borrowed && br.DueDate < DateTimeOffset.UtcNow);
        int pendingReservations = reservations.Count(r => r.Status == ReservationStatus.Pending);
        decimal totalFines = borrowRecords.Where(br => br.ReturnedAt.HasValue).Sum(br => br.FineAmount);

        return new MemberActivityDto
        {
            MemberId = member.Id,
            MemberName = user?.FullName ?? string.Empty,
            MembershipNumber = member.MembershipNumber,
            TotalBorrows = totalBorrows,
            ActiveBorrows = activeBorrows,
            OverdueBorrows = overdueBorrows,
            PendingReservations = pendingReservations,
            TotalFines = totalFines,
            JoinedDate = member.JoinedDate.DateTime
        };
    }
}

public static class ReportMappers
{
    public static async Task<BorrowHistoryDto> MapToHistoryDtoAsync(BorrowRecord record, IUnitOfWork uow, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(record.Member.UserId, ct);
        var book = await uow.Books.GetByIdAsync(record.BookCopy.BookId, ct);
        var author = await uow.Authors.GetByIdAsync(book.AuthorId, ct);

        return new BorrowHistoryDto
        {
            Id = record.Id,
            MemberName = user?.FullName ?? string.Empty,
            BookTitle = book.Title,
            ISBN = book.ISBN,
            AuthorName = author?.Name ?? string.Empty,
            BorrowedAt = record.BorrowedAt.DateTime,
            DueDate = record.DueDate.DateTime,
            ReturnedAt = record.ReturnedAt?.DateTime,
            DaysKept = record.ReturnedAt.HasValue
                ? (record.ReturnedAt.Value - record.BorrowedAt).Days
                : (DateTimeOffset.UtcNow - record.BorrowedAt).Days,
            FineAmount = record.FineAmount
        };
    }
}