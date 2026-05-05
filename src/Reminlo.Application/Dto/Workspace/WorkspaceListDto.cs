namespace Reminlo.Application.Dto.Workspace;

public class WorkspaceListDto
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string Role { get; set; }
    public int MembersCount { get; set; }
}