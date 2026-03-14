using VoxelEngine.Graphics;

namespace VoxelEngine.Kernel;

public interface IPlatform : IDisposable
{
    IWindowSurface CreateWindowSurface();

}