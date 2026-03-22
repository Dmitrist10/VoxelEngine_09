using Arch.Core;
using Arch.Core.Extensions;
using VoxelEngine.Common;
using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public class EP_MeshBounds : EntityProcessor, IUpdatable
{
    private QueryDescription _meshWithoutAABB = new QueryDescription().WithAll<C_Mesh>().WithNone<C_AABB>();

    public override void OnInitialize()
    {
        Group = ProcessorGroup.Engine;
    }

    public void OnUpdate()
    {
        // Auto-attach C_AABB to entities that have a Mesh but no bounds component
        world.Query(in _meshWithoutAABB, (Entity e, ref C_Mesh mesh) =>
        {
            // Only add if the mesh asset is valid and has bounds
            if (mesh.Mesh.Handle != default)
            {
                world.Add(e, new C_AABB(mesh.Mesh.Bounds));
            }
        });

        // Note: Future improvement could include updating existing AABBs if the mesh asset changes.
    }
}
