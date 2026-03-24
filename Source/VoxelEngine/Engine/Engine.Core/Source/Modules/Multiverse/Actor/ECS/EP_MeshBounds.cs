using Arch.Core;
using Arch.Core.Extensions;
using VoxelEngine.Common;
using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public class EP_MeshBounds : SEntityProcessor, IUpdatable
{
    private QueryDescription _meshWithoutAABB = new QueryDescription().WithAll<C_Mesh>().WithNone<C_AABB>();

    public void OnUpdate()
    {
        world.Query(in _meshWithoutAABB, (Entity e, ref C_Mesh mesh) =>
        {
            // Only add if the mesh asset is valid and has bounds
            if (mesh.Mesh.Handle != default)
            {
                world.Add(e, new C_AABB(mesh.Mesh.Bounds));
            }
        });

    }

}
