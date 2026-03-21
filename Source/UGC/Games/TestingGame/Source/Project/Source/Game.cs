using VoxelEngine.Core;
using VoxelEngine.Core.UGC;
using VoxelEngine.Diagnostics;
using VoxelEngine.IO;
using VoxelEngine.IO.FilesManagement;

namespace TestingGame;

public sealed class Game : GameBase
{
    private const string ASSETS_PATH = @"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_10\Source\UGC\Games\TestingGame\Source\Resources";

    public override void OnInitialize()
    {
        IFileManager fileManager = EngineContext.Get<IFileManager>();
        // fileManager.Mount("assets", new DiskFilesProvider(ASSETS_PATH, true, true));
        fileManager.Mount("resources", new DiskFilesProvider(ASSETS_PATH, true, true));
    }

    public override void StartSession()
    {
        Universe universe = universeManager.Create();
        Scene scene = universe.Create();

        GameSetUp.SetUp(scene);
    }

}
