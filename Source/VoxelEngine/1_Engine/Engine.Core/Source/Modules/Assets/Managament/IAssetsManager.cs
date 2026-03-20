namespace VoxelEngine.Assets;

public interface IAssetsManager
{
    /// <summary>
    /// Mounts a specific Assets Loader to the Assets Manager for a specified Asset type and settings type.
    /// </summary>
    /// <typeparam name="TAsset">The type of the asset to load.</typeparam>
    /// <typeparam name="TOptions">The type of the options to use for loading the asset.</typeparam>
    /// <param name="loader">The asset loader to mount.</param>
    void MountAssetsLoader<TAsset, TOptions>(IAssetLoader<TAsset, TOptions> loader)
        where TAsset : IAsset
        where TOptions : IAssetOptions;

    /// <summary>
    /// Loads an asset from the specified path using the provided options.
    /// </summary>
    /// <typeparam name="TAsset">The type of the asset to load.</typeparam>
    /// <typeparam name="TOptions">The type of the options to use for loading the asset.</typeparam>
    /// <param name="path">The path to the asset to load.</param>
    /// <param name="options">The options to use for loading the asset.</param>
    /// <returns>Loaded Asset</returns>
    /// <exception cref="NotSupportedException">Thrown if no loader is registered for the specified asset type and options type.</exception>
    /// <exception cref="InvalidCastException">Thrown if the loaded asset is not of the specified type.</exception>
    TAsset Load<TAsset, TOptions>(string path, TOptions options)
        where TAsset : IAsset
        where TOptions : IAssetOptions;

    /// <summary>
    /// Loads an asset from the specified path using the default options.
    /// </summary>
    /// <typeparam name="TAsset">The type of the asset to load.</typeparam>
    /// <param name="path">The path to the asset to load.</param>
    /// <returns>Loaded Asset</returns>
    /// <exception cref="NotSupportedException">Thrown if no loader is registered for the specified asset type.</exception>
    /// <exception cref="InvalidCastException">Thrown if the loaded asset is not of the specified type.</exception>
    TAsset Load<TAsset>(string path) where TAsset : IAsset;

    void FreeAsset(string path);
    // void FreeAsset<TAsset>(TAsset asset) where TAsset : IAsset; // maybe later

}
