using Arch.Core;
using VoxelEngine.Common;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace TestingGame;

internal record struct C_Template();

internal sealed class ECS_Template : EntityProcessor, IUpdatable
{
    private QueryDescription query;

    public ECS_Template()
    {
        query = new QueryDescription().WithAll<C_Testing>();
    }

    public void OnUpdate()
    {
        world.Query(query, (ref C_Testing testing) =>
        {
            // your logic here
        });
    }

}