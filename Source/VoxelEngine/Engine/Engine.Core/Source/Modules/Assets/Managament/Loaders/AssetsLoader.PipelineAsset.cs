using VoxelEngine.Graphics;
using VoxelEngine.IO.FilesManagement;

namespace VoxelEngine.Assets;

public class AssetsLoader_PipelineAsset : IAssetLoader<PipelineAsset, PipelineOptions>
{
    private readonly IFileManager _fileManager;
    private readonly IGraphicsFactory _factory;

    public AssetsLoader_PipelineAsset(IFileManager fileManager, IGraphicsFactory factory)
    {
        _fileManager = fileManager;
        _factory = factory;
    }

    public PipelineAsset Load(string path, PipelineOptions options)
    {
        ShaderData data = ShaderLoader.Load(path, options.ShaderOptions, _fileManager); // Get shader Data
        PipelineDescription desc = new PipelineDescription(data); // Create Pipeline Description

        PipelineHandle handle = _factory.CreatePipeline(desc); // Create Pipeline
        PipelineAsset asset = new PipelineAsset(handle); // Create a wrapper for the handle
        return asset;
    }
}
