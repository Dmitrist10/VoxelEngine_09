namespace VoxelEngine.Rendering;

public interface IRenderer
{
    void Render();
    void Submit(RenderCommand renderCommand);
}
