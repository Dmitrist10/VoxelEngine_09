namespace VoxelEngine.Core;

public interface IHost : IDisposable
{
    void OnInitialize();
    void OnTick();
    void OnUpdate();

    // void OnUniverseCreated(Universe universe);
    // void OnSceneCreated(Scene scene);
}