using VoxelEngine.Core;
using VoxelEngine.Core.UGC;

namespace VoxelEngine.RuntimeContexts.Standalone;

public class StandaloneRuntimeContext : IRuntimeContext
{
    public string Name => "Standalone";
    private IGame game;

    public StandaloneRuntimeContext(IGame game)
    {
        this.game = game;
    }


    public IEngineState CreateFirstState()
    {
        return new GameState(game);
    }
}
