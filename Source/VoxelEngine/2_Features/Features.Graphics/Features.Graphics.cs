using System.ComponentModel.Design;
using VoxelEngine.Core;
using VoxelEngine.Core.Runtime;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Graphics;

public sealed class GraphicsFeature : IFeature
{
    public int Priority => 0;

    public void OnPreInit()
    {
        EngineContext.Subscribe<Event_WindowLoadedEvent>(OnWindowLoaded);
    }

    private void OnWindowLoaded(Event_WindowLoadedEvent @event)
    {
        EngineContext.Register<IWindowSurface>(@event.WindowSurface);


        // IGraphicsDevice graphicsDevice = @event.Platform.CreateGraphicsDevice();
        // IGraphicsDevice graphicsDevice = EngineContext.Get<IGraphicsDevice>();
        // GraphicsContext graphicsContext = new(@event.WindowSurface, graphicsDevice);
        // EngineContext.Register<GraphicsContext>(graphicsContext);
    }


}