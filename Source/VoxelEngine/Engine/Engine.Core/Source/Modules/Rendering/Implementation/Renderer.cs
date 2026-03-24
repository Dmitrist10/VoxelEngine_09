using VoxelEngine.Graphics;
using VoxelEngine.Windowing;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Rendering;

public sealed class Renderer : IRenderer
{

    private List<RenderCommand> _renderCommands = new(2048 * 2);
    private List<CameraData> _cameras = new(2);

    private readonly IWindowSurface windowSurface;
    private readonly IGraphicsDevice graphicsDevice;
    // private readonly RenderContext _renderContext;

    private readonly RenderGraph _renderGraph;

    public Renderer(IWindowSurface windowSurface, IGraphicsDevice graphicsDevice)
    {
        this.windowSurface = windowSurface;
        this.graphicsDevice = graphicsDevice;
        // _renderContext = new();
        _renderGraph = new RenderGraph(this);
    }

    public void RenderFrame()
    {
        foreach (var camera in _cameras)
        {
            RenderContext renderContext = new(windowSurface, graphicsDevice, _renderCommands, camera);
            _renderGraph.Execute(renderContext);
        }

        graphicsDevice.Render();

        // EngineContext.Get<IGuiRenderer>()?.Render();

        graphicsDevice.Present();

        _renderCommands.Clear();
        _cameras.Clear();
    }

    public void Submit(RenderCommand renderCommand)
    {
        _renderCommands.Add(renderCommand);
    }
    public void Submit(CameraData cameraData)
    {
        _cameras.Add(cameraData);
    }



}