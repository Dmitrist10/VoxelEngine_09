using System.Numerics;
using VoxelEngine.Assets;
using VoxelEngine.Core;
using VoxelEngine.Common;
using VoxelEngine.Core.Runtime;
using VoxelEngine.Graphics;
using VoxelEngine.IO.FilesManagement;
using VoxelEngine.Packages.Voxel;

namespace TestingGame;

public sealed class Game : IGame
{

    private const string AssetsPath = @"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_11\Source\UGC\Games\TestingGame\Resources\Public\Assets";
    private const string ResourcesPath = @"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_11\Source\UGC\Games\TestingGame\Resources";

    public IEnginePackage[] GetPackages()
    {
        return
        [
            new VoxelPackage()
        ];
    }

    public void OnPreInit()
    {
        IFileManager fileManager = EngineContext.Get<IFileManager>();
        fileManager.Mount("resources", new DiskFilesProvider(ResourcesPath, true, true));
        fileManager.Mount("assets", new DiskFilesProvider(AssetsPath, true, true));
    }

    public void OnInit()
    {

    }

    public void StartGame()
    {
        var multiverse = EngineContext.Get<Multiverse>();
        var universe = multiverse.Create();
        var scene = universe.CreateScene();

        Actor a = scene.CreateActor();
        a.AddComponent(new C_Camera(CameraProjectionType.Perspective));
        a.Position = new Vector3(0, 0, 5);

        Actor tree = scene.CreateActor();
        var mesh = EngineContext.Get<IAssetsManager>().LoadMesh("assets://Models/Tree/Tree.obj");
        var texture = EngineContext.Get<IAssetsManager>().LoadTexture("assets://Models/Tree/Tree.png");
        var material = EngineContext.Get<IAssetsManager>().LoadPBRMaterial("assets://Models/Tree/Tree.mtl");
        material.Properties.Color = Color.Red;
        material.Properties.AO = 1f;
        material.Properties.Metallic = 2f;
        material.AlbedoTexture = texture.Handle;
        material.ApplyChanges();

        tree.AddComponent(new C_Mesh(mesh, material));
        tree.Position = new Vector3(0, -5, -15);
    }


}
