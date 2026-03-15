using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Diagnostics;

namespace TestingGame;

public static class GameSetUp
{

    public static void SetUp(Scene scene)
    {
        Actor actor = scene.CreateActor();
        C_Mesh mesh = new C_Mesh();
        actor.AddComponent(mesh);

        Logger.Debug($"Game setup is running for scene: {scene}");
    }

}
