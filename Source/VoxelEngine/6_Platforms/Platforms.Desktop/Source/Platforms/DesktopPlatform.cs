using VoxelEngine.Core;

using VoxelEngine.Graphics;
using VoxelEngine.Graphics.OpenGL;
using VoxelEngine.Windowing;

using VoxelEngine.Input;
using VoxelEngine.Input.SilkNet;

namespace VoxelEngine.Platforms.Desktop;

public class DesktopPlatform : IPlatform
{
    public IGraphicsDriver CreateGraphicsDriver()
    {
        return new GL_GraphicsDriver();
    }

    public IInputDriver CreateInputDriver()
    {
        return new InputDriver();
    }

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
