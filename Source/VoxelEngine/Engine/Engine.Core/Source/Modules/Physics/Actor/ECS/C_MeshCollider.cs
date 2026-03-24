using System.Numerics;
using VoxelEngine.Graphics;

namespace VoxelEngine.Physics;

public record struct C_MeshCollider
{
    public Vector3 center;
    public MeshAsset? mesh;

    public PhysicsMeshColliderHandle ColliderHandle;

    public C_MeshCollider(Vector3 center, MeshAsset mesh)
    {
        this.center = center;
        this.mesh = mesh;
    }
}