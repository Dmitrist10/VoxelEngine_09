using VoxelEngine.Assets;
using VoxelEngine.Common;


// Graphics
namespace VoxelEngine.Graphics
{
    public readonly record struct RenderTargetHandle(ResourceHandle Handle) : IAsset;
    public readonly record struct MeshHandle(ResourceHandle Handle) : IAsset;
    public readonly record struct TextureHandle(ResourceHandle Handle) : IAsset;
    public readonly record struct BufferHandle(ResourceHandle Handle) : IAsset;
    public readonly record struct PipelineHandle(ResourceHandle Handle) : IAsset;
    public readonly record struct MaterialHandle(ResourceHandle Handle) : IAsset;
    public readonly record struct ShaderHandle(ResourceHandle Handle) : IAsset;
}


namespace VoxelEngine.Audio
{
    public readonly record struct AudioHandle(ResourceHandle Handle) : IAsset;
}

namespace VoxelEngine.Physics
{
    public readonly record struct PhysicsHandle(ResourceHandle Handle) : IAsset;
    public readonly record struct PhysicsMeshColliderHandle(ResourceHandle Handle) : IAsset;
}