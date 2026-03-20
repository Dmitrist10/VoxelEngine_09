using VoxelEngine.Core;
using VoxelEngine.Core.Runtime;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Graphics.OpenGL;

public class Features_OpenGL : IFeature
{
    public int Priority => 100;

    private GL_GraphicsDevice? _device;

    public void OnPreInit()
    {
        _device = new GL_GraphicsDevice();
        EngineContext.Register<IGraphicsDevice>(_device);

        EngineContext.Subscribe<Event_WindowLoadedEvent>(OnWindowLoaded);
    }

    private void OnWindowLoaded(Event_WindowLoadedEvent @event)
    {
        _device!.Init(@event.WindowSurface);
    }

}