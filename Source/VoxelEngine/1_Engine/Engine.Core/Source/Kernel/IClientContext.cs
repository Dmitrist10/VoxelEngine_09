
namespace VoxelEngine.Kernel;

public interface IClientContext
{
    void OnUpdate();
    void OnRender();
    void OnUniverseCreated();
    void OnSceneCreated();
}
