namespace VoxelEngine.Assets;

public interface IAssetLoader { }
public interface IAssetOptions { }

public interface IAssetLoader<TAsset, TOptions> : IAssetLoader
    where TOptions : IAssetOptions
    where TAsset : IAsset
{
    TAsset Load(string path, TOptions options);
}

public readonly record struct EmptyOptions : IAssetOptions
{
    public static readonly EmptyOptions Default = new();
}



public readonly record struct TextureOptions(bool PixelPerfect = false, bool GenerateMipMaps = true) : IAssetOptions;
public readonly record struct MeshOptions(bool Triangulate = true, bool GenerateNormals = true) : IAssetOptions;
