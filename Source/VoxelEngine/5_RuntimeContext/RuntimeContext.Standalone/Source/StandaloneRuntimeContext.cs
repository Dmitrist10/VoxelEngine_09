using VoxelEngine.Core;
using VoxelEngine.Core.UGC;

namespace VoxelEngine.RuntimeContexts.Standalone;

public class StandaloneRuntimeContext : IRuntimeContext
{
    public string Name => "Standalone";
    private GameBase game;

    public StandaloneRuntimeContext(GameBase game)
    {
        this.game = game;
    }


    public IEngineState CreateFirstState()
    {
        return new GameState(game);
    }
}
