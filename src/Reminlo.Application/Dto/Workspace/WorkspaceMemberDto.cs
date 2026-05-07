namespace Reminlo.Application.Dto.Workspace;

public class WorkspaceMemberDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string Status { get; set; }
}
