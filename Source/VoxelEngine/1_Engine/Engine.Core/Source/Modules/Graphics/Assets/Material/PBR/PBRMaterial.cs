using System.Runtime.InteropServices;
using VoxelEngine.Assets;
using VoxelEngine.Common;
using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public sealed class PBRMaterial : Material<PBRMaterialProperties>
{
    /// <summary>Optional albedo texture. When null a shared 1×1 white texture is used,
    /// so material.Color is unaffected and cubes render with their flat colour.</summary>
    public TextureHandle? AlbedoTexture;

    // Shared 1×1 white fallback — created once, reused across all PBR materials.
    private static TextureHandle? _whiteFallback;

    public PBRMaterial(PBRMaterialProperties properties, PipelineHandle pipeline) : base(properties, pipeline) { }
    public PBRMaterial(PipelineHandle pipeline) : base(pipeline) { }

    public override void SetForRendering(IGraphicsCommandsList cmdBuffer)
    {
        base.SetForRendering(cmdBuffer); // binds pipeline + UBO

        // Lazily create the shared white fallback once
        if (_whiteFallback == null)
        {
            // 1×1 RGBA white pixel — 4 channels explicit
            byte[] white = { 255, 255, 255, 255 };
            var texData = new TextureData(white, 1, 1, 4, new TextureOptions());
            _whiteFallback = _factory.CreateTexture(texData);
        }

        // Bind the real texture, or the white fallback when none is assigned
        cmdBuffer.BindTexture(AlbedoTexture ?? _whiteFallback.Value, 0);
    }
}

public sealed class ChunkMaterial : Material<ChunkMaterialProperties>
{
    public TextureHandle AlbedoTexture;

    public ChunkMaterial(ChunkMaterialProperties properties, PipelineHandle pipeline) : base(properties, pipeline)
    {

    }
    public ChunkMaterial(PipelineHandle pipeline) : base(pipeline)
    {

    }

    public override void SetForRendering(IGraphicsCommandsList cmdBuffer)
    {
        base.SetForRendering(cmdBuffer);
        cmdBuffer.BindTexture(AlbedoTexture, 0);
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 16)]
public record struct ChunkMaterialProperties : IMaterialProperties
{
    public Color Color;
    // public float Metallic;
    // public float Roughness;
    // public float AO;

    public ChunkMaterialProperties()
    {
        Color = Color.White;
    }

    public ChunkMaterialProperties(Color color)
    {
        Color = color;
    }

}
