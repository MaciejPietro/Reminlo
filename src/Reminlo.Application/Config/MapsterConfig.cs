using Mapster;

namespace Reminlo.Application.Config;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Domain.Entities.Workspace.Workspace, Features.Workspace.WorkspaceDto>
            .NewConfig()
            .Ignore(dto => dto.Role)
            .Ignore(dto => dto.MembersCount);
    }
}
