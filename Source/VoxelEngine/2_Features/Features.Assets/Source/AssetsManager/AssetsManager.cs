using VoxelEngine.Core;

namespace VoxelEngine.Assets;

public sealed class AssetsManager : IAssetsManager
{
    private readonly Dictionary<Type, IAssetLoader> _loaders = new();

    public void RegisterLoader<TAsset, TOptions>(IAssetLoader<TAsset, TOptions> loader)
        where TOptions : IAssetOptions
        where TAsset : IAsset
    {
        _loaders[typeof(TAsset)] = loader;
    }

    public TAsset Load<TAsset, TOptions>(string path, TOptions options)
        where TOptions : IAssetOptions
        where TAsset : IAsset
    {
        if (_loaders.TryGetValue(typeof(TAsset), out var loader) &&
            loader is IAssetLoader<TAsset, TOptions> typedLoader)
        {
            return typedLoader.Load(path, options);
        }

        throw new Exception($"No loader registered for type: {typeof(TAsset)} with options: {typeof(TOptions)}");
    }
    public TAsset Load<TAsset>(string path) where TAsset : IAsset
    {
        if (_loaders.TryGetValue(typeof(TAsset), out var loader) &&
            loader is IAssetLoader<TAsset, EmptyOptions> typedLoader)
        {
            return typedLoader.Load(path, EmptyOptions.Default);
        }

        throw new Exception($"No loader registered for type: {typeof(TAsset)}");
    }

}

