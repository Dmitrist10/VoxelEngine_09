using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Diagnostics;
using VoxelEngine.Assets;
using System.Numerics;

namespace TestingGame;

public static class GameSetUp
{

    public static void SetUp(Scene scene)
    {
        Logger.Debug($"Game setup is running for scene: {scene}");

        Actor actor = scene.CreateActor();
        C_Mesh mesh = new C_Mesh();
        mesh.Mesh = EngineContext.Get<IAssetsManager>().LoadCube();
        mesh.Material = EngineContext.Get<IAssetsManager>().LoadMaterial();
        actor.AddComponent(mesh);


        Actor camera = scene.CreateActor();
        C_Camera cameraComponent = new C_Camera(CameraProjectionType.Perspective);
        camera.AddComponent(cameraComponent);
        camera.Position = new Vector3(0, 0, -5);
    }

}
