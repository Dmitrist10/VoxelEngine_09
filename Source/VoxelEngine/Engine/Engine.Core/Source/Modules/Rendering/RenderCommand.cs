using System.Numerics;
using VoxelEngine.Common;
using VoxelEngine.Graphics;

namespace VoxelEngine.Rendering;

public readonly record struct RenderCommand(
    MeshAsset Mesh,
    Material Material,
    Matrix4x4 Transform
);

public readonly record struct LocalRenderCommand(
    MeshAsset Mesh,
    Material Material,
    Matrix4x4 Transform,

    ulong SortingKey,
    AABB Bounds,
    uint Layer
);