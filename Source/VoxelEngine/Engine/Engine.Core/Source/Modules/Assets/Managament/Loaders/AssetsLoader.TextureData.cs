using VoxelEngine.Graphics;
using VoxelEngine.IO.FilesManagement;

namespace VoxelEngine.Assets;

public class AssetsLoader_TextureData : IAssetLoader<TextureData, TextureOptions>
{
    private readonly IFileManager _fileManager;

    public AssetsLoader_TextureData(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public TextureData Load(string path, TextureOptions options)
    {
        return TextureLoader.Load(path, options, _fileManager);
    }
}

public class AssetsLoader_TextureAsset : IAssetLoader<TextureAsset, TextureOptions>
{
    private readonly IFileManager _fileManager;
    private readonly IGraphicsFactory _factory;

    public AssetsLoader_TextureAsset(IFileManager fileManager, IGraphicsFactory factory)
    {
        _fileManager = fileManager;
        _factory = factory;
    }

    public TextureAsset Load(string path, TextureOptions options)
    {
        TextureData data = TextureLoader.Load(path, options, _fileManager); // Get Data from file
        TextureHandle handle = _factory.CreateTexture(data); // upload to the GPU
        TextureAsset asset = new TextureAsset(handle, data.Width, data.Height); // Create a wrapper for the handle
        return asset;
    }
}
