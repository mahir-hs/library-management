using LibraryManagement.Application.DTOs.Borrow;
using LibraryManagement.Domain.Common;
using MediatR;

namespace LibraryManagement.Application.Features.Borrows.Queries;

public record GetBorrowByIdQuery(Guid Id) : IRequest<BorrowDto?>;

public record GetBorrowsByMemberQuery(Guid MemberId, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<BorrowDto>>;

public record GetBorrowsByBookCopyQuery(Guid BookCopyId, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<BorrowDto>>;

public record GetMyBorrowsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<MyBorrowsResponse>>;

public record GetOverdueBorrowsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<BorrowListResponse>>;