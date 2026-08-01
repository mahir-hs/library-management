namespace LibraryManagement.API.Controllers;

using Application.DTOs.Member;
using Application.Features.Members.Commands;
using Application.Features.Members.Queries;
using LibraryManagement.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Member management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class MembersController : BaseController
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new member
    /// </summary>
    /// <remarks>Requires Admin or Librarian role</remarks>
    [HttpPost]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(MemberDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMember(
        [FromBody] CreateMemberRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateMemberCommand(
            request.UserId,
            request.MembershipNumber,
            request.Address,
            request.PhoneNumber
        );
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMemberById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get member details by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MemberDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMemberById(
        Guid id, CancellationToken cancellationToken)
    {
        var query = new GetMemberByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get member details by user ID
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(MemberDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMemberByUserId(
        Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetMemberByUserIdQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get current member profile
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(MemberDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var query = new GetMemberByUserIdQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get all members with pagination
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(PaginatedResult<MemberDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMembers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllMembersQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update a member
    /// </summary>
    /// <remarks>Admin can update any member, Member can update own profile</remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMember(
        Guid id,
        [FromBody] UpdateMemberRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateMemberCommand(id, request.MembershipNumber,
            request.Address, request.PhoneNumber);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete a member
    /// </summary>
    /// <remarks>Requires Admin or Librarian role</remarks>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMember(
        Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteMemberCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}