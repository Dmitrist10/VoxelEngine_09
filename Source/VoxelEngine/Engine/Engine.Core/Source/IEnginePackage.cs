namespace VoxelEngine.Core;

public interface IEnginePackage
{
    string Name { get; }

    void OnGameStarted();
    void OnInitialize();
    void OnSceneCreated(Scene scene);
    void OnUniverseCreated(Universe universe);
}
