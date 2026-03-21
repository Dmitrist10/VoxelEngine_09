using VoxelEngine.Core;
using VoxelEngine.Windowing;

namespace VoxelEngine.Graphics.OpenGL;

public sealed class GL_GraphicsDriver : IGraphicsDriver
{
    private GL_GraphicsDevice _graphicsDevice = null!;

    public IGraphicsDevice CreateGraphicsDevice()
    {
        _graphicsDevice = new GL_GraphicsDevice();
        return _graphicsDevice;
    }


    public void Initialize()
    {
        IWindowSurface windowSurface = EngineContext.Get<IWindowSurface>();
        _graphicsDevice.Init(windowSurface);
    }

    public void Dispose()
    {
    }


}