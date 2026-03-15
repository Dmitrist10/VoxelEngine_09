using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public interface IPlatform : IDisposable
{
    IWindowSurface CreateWindowSurface();

    IGraphicsDevice CreateGraphicsDevice();
}