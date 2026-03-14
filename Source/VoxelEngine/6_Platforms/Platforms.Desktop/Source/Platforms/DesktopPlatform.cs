using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Kernel;

namespace VoxelEngine.Platforms.Desktop;

public class DesktopPlatform : IPlatform
{

    public IWindowSurface CreateWindowSurface()
    {
        return new DesktopWindow("VoxelEngine", 1280, 720);
    }


    public void Dispose()
    {
    }


}
