namespace VoxelEngine.Rendering;

public sealed class Renderer : IRenderer
{
    private List<RenderCommand> _renderCommands;

    public Renderer()
    {
        _renderCommands = new List<RenderCommand>(2048 * 2);
    }

    public void Render()
    {

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Submit(RenderCommand renderCommand)
    {
        _renderCommands.Add(renderCommand);
    }

}