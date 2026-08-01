using LibraryManagement.Application.DTOs.Report;
using LibraryManagement.Domain.Common;
using MediatR;

namespace LibraryManagement.Application.Features.Reports.Queries;

public record GetReportSummaryQuery : IRequest<ReportSummaryDto>;

public record GetBorrowHistoryQuery(
    int PageNumber = 1,
    int PageSize = 10,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<PaginatedResult<BorrowHistoryDto>>;

public record GetOverdueBooksQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<OverdueBookDto>>;

public record GetMemberActivityQuery(Guid MemberId) : IRequest<MemberActivityDto>;