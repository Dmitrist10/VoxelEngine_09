using VoxelEngine.Common;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Assets;

public sealed class AssetsManager : IAssetsManager
{
    private readonly record struct AssetCacheKey(string VirtualPath, IAssetOptions Options);

    private struct AssetCacheItem
    {
        // public AssetId Id;
        public AssetCacheKey Key;
        public ushort ReferenceCount;
        public IAsset? Asset;

        public AssetCacheItem(AssetCacheKey key, IAsset asset, ushort referenceCount = 1) => (Key, Asset, ReferenceCount) = (key, asset, referenceCount);
        public AssetCacheItem(AssetCacheKey key, ushort referenceCount = 1) => (Key, ReferenceCount) = (key, referenceCount);

        public AssetCacheItem Clone() => new AssetCacheItem() { Key = Key, ReferenceCount = 0, Asset = Asset };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddReference() => ReferenceCount++;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveReference() => ReferenceCount--;

        public bool IsValid => ReferenceCount > 0;
    }

    private Dictionary<string, AssetCacheItem> _assetCache = new();

    private readonly Dictionary<Type, IAssetLoader> _loaders = new();
    private readonly List<string> _unloadQueue = new();

    public AssetsManager()
    {
        // EngineContext.Subscribe<Event_OnSecond>(PerformCleanUp);
    }

    private void PerformCleanUp(Event_OnSecond @event)
    {
        foreach (var item in _assetCache)
        {
            if (item.Value.IsValid)
                continue;

            _unloadQueue.Add(item.Key);
        }

        foreach (var key in _unloadQueue)
        {
            Logger.Debug($"Unloading item: '{key}'; RefCount: {_assetCache[key].ReferenceCount}");
            // _assetCache[key].Asset?.Dispose();
            _assetCache.Remove(key);
        }

        _unloadQueue.Clear();
    }

    public void MountAssetsLoader<TAsset, TOptions>(IAssetLoader<TAsset, TOptions> loader)
        where TOptions : IAssetOptions
        where TAsset : IAsset
    {
        _loaders[typeof(TAsset)] = loader;
    }

    public TAsset Load<TAsset, TOptions>(string path, TOptions options)
        where TOptions : IAssetOptions
        where TAsset : IAsset
    {
        AssetCacheKey key = new(path, options);

        // Check hash
        if (_assetCache.TryGetValue(key.VirtualPath, out var cache))
        {
            cache.AddReference();

            if (cache.Asset is TAsset asset)
                return asset;

            throw new InvalidCastException($"Asset {path} is not of type {typeof(TAsset)}");
        }

        // Load if not in cache
        if (_loaders.TryGetValue(typeof(TAsset), out var loader) && loader is IAssetLoader<TAsset, TOptions> typedLoader)
        {
            TAsset asset = typedLoader.Load(path, options);
            var item = new AssetCacheItem(key, asset);
            _assetCache.Add(key.VirtualPath, item);

            return asset;
        }

        throw new NotSupportedException($"No loader registered for type: {typeof(TAsset)} with options: {typeof(TOptions)}");
    }

    public TAsset Load<TAsset>(string path) where TAsset : IAsset
    {
        return Load<TAsset, EmptyOptions>(path, EmptyOptions.Default);
    }

    public void FreeAsset(string path)
    {
        if (_assetCache.TryGetValue(path, out var item))
        {
            item.RemoveReference();
        }
#if GameDebug
        else
        {
            Logger.Error($"Asset under path: '{path}' not found in cache");
        }
#endif
    }

    public void FreeAsset<TAsset>(TAsset asset) where TAsset : IAsset
    {
        throw new NotImplementedException("zhe shi not implemented yet!");
    }
}