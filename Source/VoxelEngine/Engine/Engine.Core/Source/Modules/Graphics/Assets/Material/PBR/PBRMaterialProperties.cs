using System.Runtime.InteropServices;
using VoxelEngine.Common;

namespace VoxelEngine.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 16)]
public record struct PBRMaterialProperties : IMaterialProperties
{
    public Color Color;
    public float Metallic;
    public float Roughness;
    public float AO;

    public PBRMaterialProperties()
    {
        Color = Color.White;
        Metallic = 0.0f;
        Roughness = 0.5f;
        AO = 1.0f;
    }

    public PBRMaterialProperties(Color color, float metallic = 0, float roughness = 0.5f, float ao = 1.0f)
    {
        Color = color;
        Metallic = metallic;
        Roughness = roughness;
        AO = ao;
    }

}
