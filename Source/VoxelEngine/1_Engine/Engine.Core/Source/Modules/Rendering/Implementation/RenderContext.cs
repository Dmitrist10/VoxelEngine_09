using VoxelEngine.Graphics;
using VoxelEngine.Windowing;

namespace VoxelEngine.Rendering;

// public sealed class RenderContext
// {
//     private CameraData _camera;
//     private List<RenderCommand> _renderCommands = new();

//     private readonly IWindowSurface _windowSurface;
//     private readonly IGraphicsDevice _graphicsDevice;
//     private readonly IGraphicsFactory _graphicsFactory;

//     public RenderContext(IWindowSurface windowSurface, IGraphicsDevice graphicsDevice)
//     {
//         _windowSurface = windowSurface;
//         _graphicsDevice = graphicsDevice;
//         _graphicsFactory = graphicsDevice.Factory;
//     }

//     public void Begin(CameraData cameraData)
//     {
//         _camera = cameraData;
//     }

//     public void End()
//     {

//     }

// }
public readonly struct RenderContext
{
    public readonly CameraData camera;
    public readonly List<RenderCommand> renderCommands;

    public readonly IWindowSurface windowSurface;
    public readonly IGraphicsDevice graphicsDevice;
    public readonly IGraphicsFactory graphicsFactory;

    private readonly List<IGraphicsCommandsList> _graphicsCommandsLists = new(16);
    public IReadOnlyList<IGraphicsCommandsList> GraphicsCommandsLists => _graphicsCommandsLists;

    public RenderContext(IWindowSurface windowSurface, IGraphicsDevice graphicsDevice, List<RenderCommand> renderCommands, CameraData cameraData)
    {
        this.windowSurface = windowSurface;
        this.graphicsDevice = graphicsDevice;
        this.graphicsFactory = graphicsDevice.Factory;
        this.renderCommands = renderCommands;
        this.camera = cameraData;
    }

    public IGraphicsCommandsList CreateGraphicsCommandsList()
    {
        return graphicsFactory.CreateCommandsList();
    }

    internal void Submit(IGraphicsCommandsList commands)
    {
        // _graphicsCommandsLists.Add(commands);
        graphicsDevice.Submit(commands);
    }
}
