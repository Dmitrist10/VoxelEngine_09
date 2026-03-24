using VoxelEngine.Windowing;

namespace VoxelEngine.Core.Runtime;

public interface IEngineProfile
{
    void Initialize();

    IWindowSurface CreateWindowSurface(string title, int width, int height);
    IEngineSubsystem[] CreateEngineSubSystems();
}
