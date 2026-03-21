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

        IAssetsManager assetsManager = EngineContext.Get<IAssetsManager>();

        Actor camera = scene.CreateActor();
        C_Camera cameraComponent = new C_Camera(CameraProjectionType.Perspective);
        camera.AddComponent(cameraComponent);
        camera.Position = new Vector3(0, 0, -5);

        PBRMaterial material = assetsManager.LoadPBRMaterial(@"resources://Shaders/PBRShader.glsl");

        Actor actor = scene.CreateActor();
        C_Mesh mesh = new C_Mesh();
        mesh.Mesh = assetsManager.LoadCube();
        mesh.Material = material;
        actor.AddComponent(mesh);

        Actor tree = scene.CreateActor();
        C_Mesh treeMesh = new C_Mesh();
        treeMesh.Mesh = assetsManager.LoadMesh("resources://Assets/Models/Tree/Tree.obj");
        treeMesh.Material = material;
        tree.AddComponent(treeMesh);
        tree.Position = new Vector3(0, 1, 2);

        Actor actor2 = scene.CreateActor();
        actor2.AddComponent(new C_Testing());

        scene.AddProcessor<EP_Testing>();
    }

}
