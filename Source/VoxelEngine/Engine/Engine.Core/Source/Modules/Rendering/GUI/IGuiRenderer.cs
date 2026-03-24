namespace VoxelEngine.Rendering;

public interface IGuiRenderer : IDisposable
{
    void Update(float deltaTime);
    void Render();
}
