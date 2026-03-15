using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public readonly record struct TextureAsset : IAsset
{
    public readonly TextureHandle Handle;
    public readonly uint Width;
    public readonly uint Height;

    public TextureAsset(TextureHandle handle, uint width, uint height) : this()
    {
        Handle = handle;
        Width = width;
        Height = height;
    }

}

