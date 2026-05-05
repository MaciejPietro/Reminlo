using Mapster;
using Reminlo.Application.Dto.Workspace;
using Reminlo.Domain.Entities.Workspace;

namespace Reminlo.Application.Config;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Workspace, WorkspaceListDto>
            .NewConfig()
            .Ignore(dto => dto.Role)
            .Ignore(dto => dto.MembersCount);
    }
}
