using Assimp;
using System.Numerics;

using VoxelEngine.Core;
using VoxelEngine.Common;
using VoxelEngine.Graphics;
using VoxelEngine.Diagnostics;
using VoxelEngine.IO.FilesManagement;

namespace VoxelEngine.Assets;

public static class MeshLoader
{

    public static AABB CalculateAABB(STDVertex[] vertices)
    {
        if (vertices.Length == 0) return new AABB(Vector3.Zero, Vector3.Zero);
        Vector3 min = vertices[0].Position;
        Vector3 max = vertices[0].Position;
        foreach (var v in vertices)
        {
            min = Vector3.Min(min, v.Position);
            max = Vector3.Max(max, v.Position);
        }
        return new AABB(min, max);
    }

    public static MeshDataRaw LoadRaw(string path, MeshOptions options)
    {
        Assimp.Scene scene = GetAssimpScene(path, options);

        var assimpMesh = scene.Meshes[0]; // TODO: Support multiple meshes
        if (scene.Meshes.Count > 1)
        {
            Logger.Warning($"Mesh '{path}' has more than one mesh! Only the first mesh will be loaded!");
        }
        var assimpPositions = new Vector3[assimpMesh.VertexCount];
        var assimpNormals = new Vector3[assimpMesh.VertexCount];
        var assimpTexCoords = new Vector2[assimpMesh.VertexCount];

        bool hasTexture = assimpMesh.HasTextureCoords(0);

        for (int i = 0; i < assimpMesh.VertexCount; i++)
        {
            var pos = assimpMesh.Vertices[i];
            var norm = assimpMesh.HasNormals ? assimpMesh.Normals[i] : new Vector3D(0, 1, 0);
            Vector2 texC = Vector2.Zero;

            if (hasTexture)
            {
                var coord = assimpMesh.TextureCoordinateChannels[0][i];
                texC = new Vector2(coord.X, coord.Y);
            }

            assimpPositions[i] = new Vector3(pos.X, pos.Y, pos.Z);
            assimpNormals[i] = new Vector3(norm.X, norm.Y, norm.Z);
            assimpTexCoords[i] = texC;
        }

        return new MeshDataRaw(
            assimpPositions,
            assimpNormals,
            assimpTexCoords
        );
    }
    public static MeshDataRawPositions LoadRawPositions(string path, MeshOptions options)
    {
        Assimp.Scene scene = GetAssimpScene(path, options);

        var assimpMesh = scene.Meshes[0]; // TODO: Support multiple meshes
        if (scene.Meshes.Count > 1)
        {
            Logger.Warning($"Mesh '{path}' has more than one mesh! Only the first mesh will be loaded!");
        }
        var assimpPositions = new Vector3[assimpMesh.VertexCount];

        for (int i = 0; i < assimpMesh.VertexCount; i++)
        {
            var pos = assimpMesh.Vertices[i];

            assimpPositions[i] = new Vector3(pos.X, pos.Y, pos.Z);
        }

        return new MeshDataRawPositions(assimpPositions);
    }

    public static STDMeshData LoadStdMesh(string path, MeshOptions options)
    {
        Assimp.Scene scene = GetAssimpScene(path, options);

        var assimpMesh = scene.Meshes[0]; // TODO: Support multiple meshes
        if (scene.Meshes.Count > 1)
        {
            Logger.Warning($"Mesh '{path}' has more than one mesh! Only the first mesh will be loaded!");
        }

        var assimpVertices = new STDVertex[assimpMesh.VertexCount];
        // var assimpIndices = new uint[assimpMesh.FaceCount * 3];
        var assimpIndices = new List<uint>();

        bool hasTexture = assimpMesh.HasTextureCoords(0);


        for (int i = 0; i < assimpMesh.VertexCount; i++)
        {
            var pos = assimpMesh.Vertices[i];
            var norm = assimpMesh.HasNormals ? assimpMesh.Normals[i] : new Vector3D(0, 1, 0);
            Vector2 texC = Vector2.Zero;

            if (hasTexture)
            {
                var coord = assimpMesh.TextureCoordinateChannels[0][i];
                texC = new Vector2(coord.X, coord.Y);
            }

            assimpVertices[i] = new STDVertex(
                new Vector3(pos.X, pos.Y, pos.Z),
                new Vector3(norm.X, norm.Y, norm.Z),
                texC
            );
        }

        foreach (var face in assimpMesh.Faces)
        {
            if (face.IndexCount == 3)
            {
                // assimpIndices.Add((uint)face.Indices[0]);
                // assimpIndices.Add((uint)face.Indices[1]);
                // assimpIndices.Add((uint)face.Indices[2]);
                assimpIndices.Add((uint)face.Indices[0]);
                assimpIndices.Add((uint)face.Indices[1]);
                assimpIndices.Add((uint)face.Indices[2]);
            }
            else
            {
                // throw new Exception($"Mesh '{path}' has a face with {face.IndexCount} indices! Only triangles are supported!");
                Logger.Info($"Mesh '{path}' has a face with {face.IndexCount} indices! Only triangles (3 indecies per face) are supported!");
            }
        }

        return new STDMeshData(assimpVertices, assimpIndices.ToArray());
    }

    private static Assimp.Scene GetAssimpScene(string path, MeshOptions options)
    {
        var fileManager = EngineContext.Get<IFileManager>() ??
            throw new Exception("FileManager not found in EngineContext!");

        using Stream stream = fileManager.OpenRead(path);
        string extension = System.IO.Path.GetExtension(path);

        using var context = new AssimpContext();

        Assimp.Scene scene;
        try
        {
            scene = context.ImportFileFromStream(stream, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals, extension);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to load mesh from stream '{path}': {ex.Message}", ex);
        }

        if (scene == null || scene.Meshes.Count == 0)
            throw new Exception($"Failed to load mesh '{path}'!");

        return scene;
    }
    private static IEnumerator<Assimp.Mesh> GetAssimpMeshes(string path, MeshOptions options)
    {
        Assimp.Scene scene = GetAssimpScene(path, options);
        foreach (var item in scene.Meshes)
        {
            yield return item;
        }
    }


}
