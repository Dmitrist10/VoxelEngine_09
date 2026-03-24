using System.Numerics;
using VoxelEngine.Common;

namespace VoxelEngine.Core;

/// <summary>
/// Component for storing the local-space bounding box of an entity.
/// Used for selection, physics, and culling.
/// </summary>
[NoInspect]
public record struct C_AABB : IComponent
{
    public AABB LocalAABB;

    public C_AABB(AABB aabb)
    {
        LocalAABB = aabb;
    }

    public C_AABB(Vector3 min, Vector3 max)
    {
        LocalAABB = new AABB(min, max);
    }
}
