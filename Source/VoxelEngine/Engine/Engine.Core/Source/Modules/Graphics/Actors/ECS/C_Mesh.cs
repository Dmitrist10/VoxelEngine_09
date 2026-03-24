using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

[NoInspect]
public record struct C_Mesh
{
    public MeshAsset Mesh;
    public Material Material;

    public C_Mesh(MeshAsset mesh, Material material)
    {
        Mesh = mesh;
        Material = material;
    }
}
