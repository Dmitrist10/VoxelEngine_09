namespace VoxelEngine.Graphics;

public record Texture2DArrayData : TextureData
{
    public uint Layers { get; }

    public Texture2DArrayData(uint width, uint height, uint layers, byte[] pixels, TextureOptions options)
        : base(pixels, width, height, options)
    {
        Layers = layers;
    }

    public Texture2DArrayData(uint width, uint height, uint layers, byte[] pixels)
        : base(pixels, width, height, TextureOptions.VoxelAtlas)
    {
        Layers = layers;
    }
}
