using VoxelEngine.Core;
using VoxelEngine.Core.Sessions;
using VoxelEngine.Core.UGC;

namespace VoxelEngine.RuntimeContexts.Standalone;

public sealed class GameState : IEngineState
{
    public static string Name => "Standalone-Game";

    private readonly GameBase game;

    public GameState(GameBase game)
    {
        this.game = game;
    }

    public void OnInitialize()
    {
        SessionsManager.StartSinglePlayer(game);
    }

}