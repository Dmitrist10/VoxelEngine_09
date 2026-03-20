using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Graphics.OpenGL;

namespace VoxelEngine.Platforms.Desktop;

public class DesktopPlatform : IPlatform
{

    public IWindowSurface CreateWindowSurface()
    {
        return new DesktopWindow("VoxelEngine", 1280, 720);
    }

    // public IGraphicsDevice CreateGraphicsDevice()
    // {
    //     return new GL_GraphicsDevice();
    // }

    public void Dispose()
    {
    }


}
