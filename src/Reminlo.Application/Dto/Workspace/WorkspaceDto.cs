namespace Reminlo.Application.Dto.Workspace;

public class WorkspaceDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ICollection<WorkspaceMemberDto> Members { get; set; }
}