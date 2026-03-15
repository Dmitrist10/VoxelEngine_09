namespace VoxelEngine.Core.UGC;

public interface IGame
{

    void SetUniverseManager(UniverseManager universeManager);

    void OnInitialize();
    void StartSession();

    // void OnUniverseCreated(Universe universe);
    // void OnSceneCreated(Scene scene);
}