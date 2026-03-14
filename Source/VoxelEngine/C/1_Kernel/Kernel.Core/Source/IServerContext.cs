
namespace VoxelEngine.Kernel;

public interface IServerContext
{
    void OnUpdate();
    void OnUniverseCreated();
    void OnSceneCreated();
}
