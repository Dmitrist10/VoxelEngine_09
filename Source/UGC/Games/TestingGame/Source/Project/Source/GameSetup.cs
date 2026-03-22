using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Diagnostics;
using VoxelEngine.Assets;
using System.Numerics;
using VoxelEngine.Common;

namespace TestingGame;

public static class GameSetUp
{

    public static void SetUp(Scene scene)
    {
        Logger.Debug($"Game setup is running for scene: {scene}");

        IAssetsManager assetsManager = EngineContext.Get<IAssetsManager>();

        Actor camera = scene.CreateActor();
        camera.Name = "Main Camera";
        C_Camera cameraComponent = new C_Camera(CameraProjectionType.Perspective, isMainCamera: true);
        camera.AddComponent(cameraComponent);
        camera.AddComponent(new C_EditorCamera());
        camera.Position = new Vector3(0, 10, -5);
        camera.Rotation = new Vector3(-30, 0, 0).ToQuaternion();
        camera.AddBehavior(new B_CameraController());

        // Cube — shiny red plastic
        PBRMaterial cubeMaterial = assetsManager.LoadPBRMaterial(@"resources://Shaders/PBRShader.glsl");
        cubeMaterial.Properties.Color = Color.Red;
        cubeMaterial.Properties.Roughness = 0.2f;
        cubeMaterial.Properties.Metallic = 0.0f;
        cubeMaterial.ApplyChanges();

        // Tree texture — matte wood/leaves
        TextureAsset treeTexture = assetsManager.LoadTexture(
            @"resources://Assets/Models/Tree/Tree.png",
            TextureOptions.PixelArt);
        PBRMaterial treeMaterial = assetsManager.LoadPBRMaterial(@"resources://Shaders/PBRShader.glsl");
        treeMaterial.Properties.Color = Color.White;
        treeMaterial.Properties.Roughness = 0.85f;
        treeMaterial.Properties.Metallic = 0.0f;
        treeMaterial.AlbedoTexture = treeTexture.Handle;
        treeMaterial.ApplyChanges();

        Actor root = scene.CreateActor();
        root.Name = "Root";

        Actor actor = scene.CreateActor();
        actor.Parent = root;
        actor.Name = "Cube";
        C_Mesh mesh = new C_Mesh();
        mesh.Mesh = assetsManager.LoadCube();
        mesh.Material = cubeMaterial;
        actor.AddComponent(mesh);
        actor.Position = new Vector3(0, 0, 5);
        actor.AddComponent(new C_Testing() { Speed = 2, IsActive = true, JumpHeight = 2 });

        Actor tree = scene.CreateActor();
        tree.Parent = root;
        tree.Name = "tree";
        C_Mesh treeMesh = new C_Mesh();
        treeMesh.Mesh = assetsManager.LoadMesh("resources://Assets/Models/Tree/Tree.obj");
        treeMesh.Material = treeMaterial;
        tree.AddComponent(treeMesh);
        tree.Position = new Vector3(0, 1, 2);

        // Actor actor2 = scene.CreateActor();
        // actor2.Parent = root;
        // actor2.Name = "Testing";
        // actor2.AddComponent(new C_Testing() { Speed = 10, IsActive = true, JumpHeight = 2 });

        scene.AddProcessor<EP_Testing>();
        scene.AddProcessor<EP_DebugUI>();
        // scene.AddProcessor<EP_FlyCamera>();
    }

}
