using VoxelEngine.Assets;
using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public record struct ShaderAsset : IAsset
{
    public ShaderHandle Handle;
}

public record ShaderData : IAssetData
{
    public readonly string Vert;
    public readonly string Frag;

    public ShaderData(string vert, string frag)
    {
        Vert = vert;
        Frag = frag;
    }
}