using VoxelEngine.Graphics;
using VoxelEngine.IO.FilesManagement;

namespace VoxelEngine.Assets;

public class AssetsLoader_ShaderData : IAssetLoader<ShaderData, ShaderOptions>
{
    private readonly IFileManager _fileManager;
    private readonly IGraphicsFactory _factory;

    public AssetsLoader_ShaderData(IFileManager fileManager, IGraphicsFactory factory)
    {
        _fileManager = fileManager;
        _factory = factory;
    }

    public ShaderData Load(string path, ShaderOptions options)
    {
        ShaderData data = ShaderLoader.Load(path, options, _fileManager); // Get Data from file
        return data;
    }
}
