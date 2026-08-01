using LibraryManagement.Application.DTOs.Borrow;
using MediatR;

namespace LibraryManagement.Application.Features.Borrows.Commands;

public record BorrowBookCommand(Guid MemberId, Guid BookCopyId) : IRequest<BorrowDto>;

public record ReturnBookCommand(Guid Id, decimal? FineAmount) : IRequest<BorrowDto>;