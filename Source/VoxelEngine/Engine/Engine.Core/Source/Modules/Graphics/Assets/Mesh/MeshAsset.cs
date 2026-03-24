using System.Numerics;
using VoxelEngine.Assets;
using VoxelEngine.Core;
using VoxelEngine.Common;

namespace VoxelEngine.Graphics;

public enum WindingOrder : byte
{
    /// <summary>
    /// Clockwise
    /// </summary>
    CW = 0,
    /// <summary>
    /// Counter Clockwise
    /// </summary>
    CCW = 1
}

public record struct MeshAsset : IGraphicsAsset
{
    public MeshHandle Handle;
    public uint VertexCount;
    public uint IndexCount;
    public AABB Bounds;
    public WindingOrder WindingOrder;

    public MeshAsset(MeshHandle handle, uint vertexCount, uint indexCount, AABB bounds, WindingOrder windingOrder = WindingOrder.CCW)
    {
        Handle = handle;
        VertexCount = vertexCount;
        IndexCount = indexCount;
        Bounds = bounds;
        WindingOrder = windingOrder;
    }
}

public record MeshData<TVertex> : IAssetData where TVertex : unmanaged, IVertexType
{
    public readonly TVertex[] Vertices;
    public readonly uint[] Indices;

    public uint VertexCount => (uint)Vertices.Length;
    public uint IndexCount => (uint)Indices.Length;

    public MeshData(TVertex[] vertices, uint[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }
}

public record STDMeshData : MeshData<STDVertex>
{
    public STDMeshData(STDVertex[] vertices, uint[] indices) : base(vertices, indices) { }
}
public record VoxelMeshData : MeshData<VoxelVertex>
{
    public VoxelMeshData(VoxelVertex[] vertices, uint[] indices) : base(vertices, indices) { }
}


public readonly record struct MeshDataRaw(Vector3[] Position, Vector3[] Normal, Vector2[] TexCoord) : IAssetData
{
    public static readonly uint Size = 12 + 12 + 8;
}
public readonly record struct MeshDataRawPositions(Vector3[] Position) : IAssetData
{
    public static readonly uint Size = 12;
}
public readonly record struct MeshDataRawNormals(Vector3[] Normal) : IAssetData
{
    public static readonly uint Size = 12;
}
public readonly record struct MeshDataRawTexCoords(Vector2[] TexCoord) : IAssetData
{
    public static readonly uint Size = 8;
}
