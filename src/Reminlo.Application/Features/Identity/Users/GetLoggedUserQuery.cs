using System.ComponentModel.DataAnnotations;
using Reminlo.Application.Services.Identity;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;
using Mapster;

namespace Reminlo.Application.Features.Identity.Users;

public sealed record GetLoggedUserQuery() : IRequest<ErrorOr<UserDto>>;

public class UserDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}

internal sealed class GetLoggedUserQueryHandler(
    IUserService userService,
    UserManager<ApplicationUser> userManager
) : IRequestHandler<GetLoggedUserQuery, ErrorOr<UserDto>>
{
    public async Task<ErrorOr<UserDto>> Handle(GetLoggedUserQuery request, CancellationToken cancellationToken)
    {
        var userResult = await userService.GetCurrentUserAsync();

        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var user = userResult.Value;
        var roles = await userManager.GetRolesAsync(user);

        var userDto = user.Adapt<UserDto>();
        userDto.Roles = roles.ToList();

        return userDto;
    }
}