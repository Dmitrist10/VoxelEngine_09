using System.Runtime.InteropServices;
using VoxelEngine.Common;

namespace VoxelEngine.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 16)]
public record struct PBRMaterialProperties : IMaterialProperties
{
    public Color Color;
    // public float Metallic;
    // public float Roughness;
    // public float AO;

    public PBRMaterialProperties()
    {
        Color = Color.White;
    }

    public PBRMaterialProperties(Color color)
    {
        Color = color;
    }

}
