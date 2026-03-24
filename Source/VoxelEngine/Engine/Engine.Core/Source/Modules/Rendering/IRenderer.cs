namespace VoxelEngine.Rendering;

public interface IRenderer
{
    void RenderFrame();
    void Submit(RenderCommand renderCommand);
    void Submit(CameraData cameraData);
}
