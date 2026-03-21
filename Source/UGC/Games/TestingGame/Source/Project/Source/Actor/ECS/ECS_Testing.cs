using Arch.Core;
using VoxelEngine.Common;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace TestingGame;

internal record struct C_Testing();

internal sealed class EP_Testing : EntityProcessor, IUpdatable
{

    private QueryDescription query;

    public EP_Testing()
    {
        query = new QueryDescription().WithAll<C_Testing>();
    }

    public void OnUpdate()
    {
        world.Query(query, (ref C_Testing testing) =>
        {
            // Logger.Debug("Testing");
        });
    }

}