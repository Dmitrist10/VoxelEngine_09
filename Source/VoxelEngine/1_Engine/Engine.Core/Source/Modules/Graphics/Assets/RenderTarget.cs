using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public record struct RenderTargetAsset(RenderTargetHandle Handle, uint Width, uint Height) : IDisposable
{
    public void Dispose()
    {
        // EngineContext.Get<IGraphicsDevice>().Factory.DestroyRenderTarget(Handle);
    }
}