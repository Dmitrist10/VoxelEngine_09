namespace VoxelEngine.Rendering;

public interface IRenderPass
{
    void Execute(in RenderContext renderContext);
    void Initialize() { }
}
