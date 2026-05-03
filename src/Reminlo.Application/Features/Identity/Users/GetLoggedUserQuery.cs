using Reminlo.Application.Services.Identity;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ResultKit;
using Mapster;

namespace Reminlo.Application.Features.Identity.Users;

public sealed record GetLoggedUserQuery() : IRequest<Result<UserDto>>;

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
) : IRequestHandler<GetLoggedUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetLoggedUserQuery request, CancellationToken cancellationToken)
    {
        var userResult = await userService.GetCurrentUserAsync();

        if (!userResult.IsSuccess)
        {
            return Result<UserDto>.Failure(userResult.Error);
        }

        var user = userResult.Value;
        var roles = await userManager.GetRolesAsync(user);

        var userDto = user.Adapt<UserDto>();
        userDto.Roles = roles.ToList();

        return Result<UserDto>.Success(userDto);
    }
}