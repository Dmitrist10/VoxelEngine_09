namespace VoxelEngine.Graphics.OpenGL;

public sealed class GL_GraphicsDriver : IGraphicsDriver
{
    public IGraphicsDevice CreateGraphicsDevice()
    {
        return new GL_GraphicsDevice();
    }

    public void Dispose()
    {
    }

    public void Initialize()
    {

    }

}