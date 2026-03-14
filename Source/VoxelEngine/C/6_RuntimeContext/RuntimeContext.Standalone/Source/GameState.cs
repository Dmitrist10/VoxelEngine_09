using VoxelEngine.Core.UGC;
using VoxelEngine.Kernel;
using VoxelEngine.Kernel.UGC;

namespace VoxelEngine.RuntimeContexts.Standalone;

public sealed class GameState : IEngineState
{
    private readonly IGame game;

    public GameState(IGame game)
    {
        this.game = game;
    }

    public string Name => "Standalone-Game";
}