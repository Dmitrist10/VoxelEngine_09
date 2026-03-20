using VoxelEngine.Core;
using System.Numerics;
using VoxelEngine.Graphics;

namespace VoxelEngine.Rendering;

public readonly record struct RenderCommand(
    MeshAsset Mesh,
    Material Material,
    Matrix4x4 Transform
);
