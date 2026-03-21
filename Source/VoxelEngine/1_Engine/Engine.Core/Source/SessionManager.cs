using VoxelEngine.Core.UGC;

namespace VoxelEngine.Core.Sessions;

public static class SessionsManager
{

    public static void StartSinglePlayer(GameBase game)
    {
        game.StartSession();
    }
    
}