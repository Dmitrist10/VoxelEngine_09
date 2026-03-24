using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Windowing;

namespace VoxelEngine.Graphics.OpenGL;

public class GL_GraphicsDriver : IGraphicsDriver
{
    private readonly GL_GraphicsDevice _graphicsDevice;

    public GL_GraphicsDriver()
    {
        IWindowSurface windowSurface = EngineContext.Get<IWindowSurface>();
        _graphicsDevice = new GL_GraphicsDevice(windowSurface);
        EngineContext.Register<IGraphicsDevice>(_graphicsDevice);
    }

    // public IGraphicsDevice CreateGraphicsDevice()
    // {
    //     return new GL_GraphicsDevice();
    // }

    public void OnInitialize()
    {
    }
    public void OnRender()
    {
    }
    public void OnFixedUpdate()
    {
    }
    public void OnTick()
    {
    }
    public void OnUpdate()
    {
    }


    public void Dispose()
    {
    }

}
