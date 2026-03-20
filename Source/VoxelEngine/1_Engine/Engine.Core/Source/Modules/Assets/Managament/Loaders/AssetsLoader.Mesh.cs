using Assimp;
using System.Numerics;

using VoxelEngine.Core;
using VoxelEngine.Assets;
using VoxelEngine.Graphics;
using VoxelEngine.IO.FilesManagement;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Assets;

public class AssetLoader_MeshAsset : IAssetLoader<MeshAsset, MeshOptions>
{

    private IGraphicsFactory _factory;

    public AssetLoader_MeshAsset(IGraphicsFactory factory)
    {
        _factory = factory;
    }


    public MeshAsset Load(string path, MeshOptions options)
    {
        STDMeshData data = MeshLoader.LoadStdMesh(path, options); // Get Data from file
        MeshHandle handle = _factory.CreateMesh(data); // upload to the GPU
        MeshAsset asset = new MeshAsset(handle, data.VertexCount, data.IndexCount); // Create a wrapper for the handle
        return asset;
    }

}
public class AssetLoader_STDMeshData : IAssetLoader<STDMeshData, MeshOptions>
{

    public AssetLoader_STDMeshData()
    {

    }

    public STDMeshData Load(string path, MeshOptions options)
    {
        return MeshLoader.LoadStdMesh(path, options);
    }

}
public class AssetLoader_MeshDataRaw : IAssetLoader<MeshDataRaw, MeshOptions>
{

    public AssetLoader_MeshDataRaw()
    {

    }


    public MeshDataRaw Load(string path, MeshOptions options)
    {
        return MeshLoader.LoadRaw(path, options);
    }

}
