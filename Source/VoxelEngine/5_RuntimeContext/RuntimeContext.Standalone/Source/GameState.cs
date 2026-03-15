using VoxelEngine.Core;
using VoxelEngine.Core.UGC;
using VoxelEngine.Core.Sessions;

namespace VoxelEngine.RuntimeContexts.Standalone;

public sealed class GameState : IEngineState
{
    public static string Name => "Standalone-Game";

    private readonly IGame game;

    public GameState(IGame game)
    {
        this.game = game;
    }

    public void OnInitialize()
    {
        var universeManager = EngineContext.Get<UniverseManager>();

        game.SetUniverseManager(universeManager);
        game.OnInitialize();
        SessionsManager.StartSinglePlayer(game);
    }

}