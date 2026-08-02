using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Auth;
using LibraryManagement.Application.Features.Auth.Commands;
using LibraryManagement.Domain.Entities;
using MediatR;

namespace LibraryManagement.Application.Features.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IAuthService authService, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user with email already exists
        var existingUserByEmailSpec = new UserByEmailSpecification(request.Email);
        var existingUserByEmail = await _unitOfWork.Users.GetFirstAsync(existingUserByEmailSpec, cancellationToken);

        if (existingUserByEmail is not null)
        {
            throw new ConflictException($"User with email '{request.Email}' already exists");
        }

        // Check if user with username already exists
        var existingUserByUsernameSpec = new UserByUsernameSpecification(request.Username);
        var existingUserByUsername = await _unitOfWork.Users.GetFirstAsync(existingUserByUsernameSpec, cancellationToken);

        if (existingUserByUsername is not null)
        {
            throw new ConflictException($"User with username '{request.Username}' already exists");
        }

        // Register the user
        var userId = await _authService.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            request.FullName,
            request.PhoneNumber,
            request.BranchId,
            request.Role,
            cancellationToken
        );

        // Get the created user details
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt?.DateTime
        };
    }
}

// Specification for finding user by username
public class UserByUsernameSpecification : SpecificationBase<User>
{
    public UserByUsernameSpecification(string username)
    {
        Criteria = u => u.Username.ToLower() == username.ToLower();
    }
}