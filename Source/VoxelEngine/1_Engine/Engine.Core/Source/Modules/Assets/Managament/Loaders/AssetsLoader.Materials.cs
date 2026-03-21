using VoxelEngine.Common;
using VoxelEngine.Graphics;
using VoxelEngine.IO.FilesManagement;

namespace VoxelEngine.Assets;

public class AssetsLoader_PBRMaterial : IAssetLoader<PBRMaterial, MaterialOptions>
{
    private readonly IFileManager _fileManager;
    private readonly IGraphicsFactory _factory;

    public AssetsLoader_PBRMaterial(IFileManager fileManager, IGraphicsFactory factory)
    {
        _fileManager = fileManager;
        _factory = factory;
    }

    public PBRMaterial Load(string path, MaterialOptions options)
    {
        ShaderData data = ShaderLoader.Load(path, options.PipelineOptions.ShaderOptions, _fileManager); // Get shader Data
        PipelineDescription desc = new PipelineDescription(data); // Create Pipeline Description

        PipelineHandle handle = _factory.CreatePipeline(desc); // Create Pipeline
        PipelineAsset asset = new PipelineAsset(handle); // Create a wrapper for the handle

        PBRMaterialProperties properties = new()
        {
            Color = Color.White,
        };
        PBRMaterial material = new PBRMaterial(properties, handle);
        return material;
    }
}
