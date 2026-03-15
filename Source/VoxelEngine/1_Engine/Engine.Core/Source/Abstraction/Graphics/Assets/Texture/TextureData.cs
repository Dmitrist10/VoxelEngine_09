using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public record TextureData : IAssetData
{
    public readonly byte[] pixelsData;
    public readonly uint Width;
    public readonly uint Height;
    public readonly uint Channels;
    public readonly TextureOptions Options;

    public TextureData(byte[] data, uint width, uint height, uint channels, TextureOptions options)
    {
        pixelsData = data;
        Width = width;
        Height = height;
        Channels = channels;
        Options = options;
    }
    public TextureData(byte[] data, uint width, uint height, TextureOptions options)
    {
        pixelsData = data;
        Width = width;
        Height = height;
        Options = options;
    }
}
