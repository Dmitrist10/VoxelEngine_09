using VoxelEngine.Common;

namespace VoxelEngine.Rendering;

internal sealed class RenderGraph
{
    // private readonly Renderer _renderer;
    // private readonly RenderContext _renderContext;

    // public RenderGraph(Renderer renderer, RenderContext renderContext)
    // {
    //     _renderer = renderer;
    //     _renderContext = renderContext;
    // }
    private readonly Renderer _renderer;

    private List<IRenderPass> _RenderPasses = new(16);

    public RenderGraph(Renderer renderer)
    {
        _renderer = renderer;

        _RenderPasses.Add(new SetupPass());
        _RenderPasses.Add(new OpaquePass());

        Initialize();
    }

    internal void Initialize()
    {
        foreach (var pass in _RenderPasses)
        {
            pass.Initialize();
        }
    }

    internal void Execute(RenderContext renderContext)
    {
        // JobScheduler.instance.ProcessParallel(_RenderPasses, (pass) => pass.Execute(renderContext));

        foreach (var pass in _RenderPasses)
        {
            pass.Execute(renderContext);
        }
    }

}
