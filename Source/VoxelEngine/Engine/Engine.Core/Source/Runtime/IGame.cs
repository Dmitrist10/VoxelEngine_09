namespace VoxelEngine.Core.Runtime;

public interface IGame
{
    IEnginePackage[] GetPackages();

    void OnPreInit();
    void OnInit();
    void StartGame();
}
