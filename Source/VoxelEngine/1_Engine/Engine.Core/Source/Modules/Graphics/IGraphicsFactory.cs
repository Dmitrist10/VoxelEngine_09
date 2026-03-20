using VoxelEngine.Assets;

namespace VoxelEngine.Graphics;

public interface IGraphicsFactory
{
    IGraphicsCommandsList CreateCommandsList();

    BufferHandle CreateBuffer(BufferDescription description);
    PipelineHandle CreatePipeline(PipelineDescription description);
    MeshHandle CreateMesh<T>(MeshData<T> meshData) where T : unmanaged, IVertexType;
    TextureHandle CreateTexture(TextureData textureData);
    TextureHandle CreateTextureArray(Texture2DArrayData textureData);

    void DestroyMesh(MeshHandle handle);
    void DestroyTexture(TextureHandle handle);
    void DestroyBuffer(BufferHandle handle);
}
