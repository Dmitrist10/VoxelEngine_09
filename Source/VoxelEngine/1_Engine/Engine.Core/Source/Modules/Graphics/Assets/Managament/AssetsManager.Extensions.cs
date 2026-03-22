using System.Numerics;
using VoxelEngine.Assets;
using VoxelEngine.Common;
using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public static class AssetsManagerGraphicsExtensions
{

    private const string VertexShaderSource = @"
#version 460 core
layout(location = 1) in vec3 aNormal;
layout(location = 0) in vec3 aPosition;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
} camera;

layout(std140, binding = 2) uniform ModelBlock {
    mat4 model;
} modelData;

void main()
{
    gl_Position = camera.projection * camera.view * modelData.model * vec4(aPosition, 1.0);
}
            ";
    private const string FragmentShaderSource = @"
            #version 460 core
layout(std140, binding = 1) uniform PBRMaterialProperties {
    vec4 Color;
} material;

out vec4 FragColor;
void main()
{
    FragColor = material.Color;
}
            ";

    public static TextureAsset LoadTexture(this IAssetsManager assetsManager, string path, TextureOptions options)
    {
        return assetsManager.Load<TextureAsset, TextureOptions>(path, options);
    }
    public static TextureAsset LoadTexture(this IAssetsManager assetsManager, string path)
    {
        TextureOptions options = new TextureOptions();
        return assetsManager.Load<TextureAsset, TextureOptions>(path, options);
    }
    public static MeshAsset LoadMesh(this IAssetsManager assetsManager, string path)
    {
        MeshOptions options = new MeshOptions();
        return assetsManager.Load<MeshAsset, MeshOptions>(path, options);
    }

    public static MeshAsset LoadCube(this IAssetsManager assetsManager)
    {
        STDVertex[] vertices = {
            // Front face  (+Z)
            new STDVertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(0, 1)),

            // Back face  (-Z)
            new STDVertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(0, 0)),
            new STDVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(1, 0)),
            new STDVertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(1, 1)),
            new STDVertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(0, 1)),

            // Left face  (-X)
            new STDVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1,  0,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(-1,  0,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(-1,  0,  0), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(-1,  0,  0), new Vector2(0, 1)),

            // Right face  (+X)
            new STDVertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3( 1,  0,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 1,  0,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 1,  0,  0), new Vector2(1, 1)),
            new STDVertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 1,  0,  0), new Vector2(0, 1)),

            // Top face  (+Y)
            new STDVertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3( 0,  1,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 0,  1,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 0,  1,  0), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3( 0,  1,  0), new Vector2(0, 1)),

            // Bottom face  (-Y)
            new STDVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3( 0, -1,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 0, -1,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3( 0, -1,  0), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3( 0, -1,  0), new Vector2(0, 1)),
        };

        uint[] indices = {
             0,  1,  2,  2,  3,  0,  // Front
             4,  5,  6,  6,  7,  4,  // Back
             8,  9, 10, 10, 11,  8,  // Left
            12, 13, 14, 14, 15, 12,  // Right
            16, 17, 18, 18, 19, 16,  // Top
            20, 21, 22, 22, 23, 20,  // Bottom
        };

        IGraphicsDevice device = EngineContext.Get<IGraphicsDevice>();

        MeshHandle handle = device.Factory.CreateMesh(new STDMeshData(vertices, indices));
        MeshAsset asset = new MeshAsset(handle, (uint)vertices.Length, (uint)indices.Length);
        return asset;
    }
    public static PBRMaterial LoadPBRMaterial(this IAssetsManager assetsManager, string path)
    {
        // Load (or retrieve from cache) the compiled pipeline — the SHADER is shared.
        PipelineAsset pipeline = assetsManager.Load<PipelineAsset, PipelineOptions>(
            path, new PipelineOptions());

        // Always create a FRESH material instance wrapping the shared pipeline.
        // This mirrors how Unreal "Material Instances" or Unity "Material.Instantiate" works:
        //   - The heavy GPU resource (compiled shader / pipeline) is cached and reused.
        //   - The lightweight property block (color, texture slot) is unique per instance.
        PBRMaterial material = new PBRMaterial(pipeline.Handle);
        return material;
    }

}





// using System.Numerics;
// using VoxelEngine.Assets;
// using VoxelEngine.Common;
// using VoxelEngine.Core;

// namespace VoxelEngine.Graphics;

// public static class AssetsManagerGraphicsExtensions
// {

//     public static TextureAsset LoadTexture(this IAssetsManager assetsManager, string path, TextureOptions options)
//     {
//         return assetsManager.Load<TextureAsset, TextureOptions>(path, options);
//     }
//     public static TextureAsset LoadTexture(this IAssetsManager assetsManager, string path)
//     {
//         return assetsManager.Load<TextureAsset>(path);
//     }
//     public static MeshAsset LoadMesh(this IAssetsManager assetsManager, string path)
//     {
//         return assetsManager.Load<MeshAsset>(path);
//     }

//     public static MeshAsset LoadCube(this IAssetsManager assetsManager)
//     {
//         STDVertex[] vertices = {
//             // Front face  (+Z)
//             new STDVertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(0, 0)),
//             new STDVertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(1, 0)),
//             new STDVertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(1, 1)),
//             new STDVertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(0, 1)),

//             // Back face  (-Z)
//             new STDVertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(0, 0)),
//             new STDVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(1, 0)),
//             new STDVertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(1, 1)),
//             new STDVertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(0, 1)),

//             // Left face  (-X)
//             new STDVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1,  0,  0), new Vector2(0, 0)),
//             new STDVertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(-1,  0,  0), new Vector2(1, 0)),
//             new STDVertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(-1,  0,  0), new Vector2(1, 1)),
//             new STDVertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(-1,  0,  0), new Vector2(0, 1)),

//             // Right face  (+X)
//             new STDVertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3( 1,  0,  0), new Vector2(0, 0)),
//             new STDVertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 1,  0,  0), new Vector2(1, 0)),
//             new STDVertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 1,  0,  0), new Vector2(1, 1)),
//             new STDVertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 1,  0,  0), new Vector2(0, 1)),

//             // Top face  (+Y)
//             new STDVertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3( 0,  1,  0), new Vector2(0, 0)),
//             new STDVertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 0,  1,  0), new Vector2(1, 0)),
//             new STDVertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 0,  1,  0), new Vector2(1, 1)),
//             new STDVertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3( 0,  1,  0), new Vector2(0, 1)),

//             // Bottom face  (-Y)
//             new STDVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3( 0, -1,  0), new Vector2(0, 0)),
//             new STDVertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 0, -1,  0), new Vector2(1, 0)),
//             new STDVertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3( 0, -1,  0), new Vector2(1, 1)),
//             new STDVertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3( 0, -1,  0), new Vector2(0, 1)),
//         };

//         uint[] indices = {
//              0,  1,  2,  2,  3,  0,  // Front
//              4,  5,  6,  6,  7,  4,  // Back
//              8,  9, 10, 10, 11,  8,  // Left
//             12, 13, 14, 14, 15, 12,  // Right
//             16, 17, 18, 18, 19, 16,  // Top
//             20, 21, 22, 22, 23, 20,  // Bottom
//         };

//         IGraphicsDevice device = EngineContext.Get<IGraphicsDevice>();

//         MeshHandle handle = device.Factory.CreateMesh(new STDMeshData(vertices, indices));
//         MeshAsset asset = new MeshAsset(handle, (uint)vertices.Length, (uint)indices.Length);
//         return asset;
//     }
//     public static PBRMaterial LoadMaterial(this IAssetsManager assetsManager)
//     {


//         IGraphicsDevice device = EngineContext.Get<IGraphicsDevice>();

//         PipelineDescription pipelineDesc = new()
//         {
//             VertexShaderSource = @"
// #version 460 core
// layout(location = 1) in vec3 aNormal;
// layout(location = 0) in vec3 aPosition;

// layout(std140, binding = 0) uniform CameraBlock {
//     mat4 view;
//     mat4 projection;
// } camera;

// layout(std140, binding = 2) uniform ModelBlock {
//     mat4 model;
// } modelData;

// void main()
// {
//     gl_Position = camera.projection * camera.view * modelData.model * vec4(aPosition, 1.0);
// }
//             ",
//             FragmentShaderSource = @"
//             #version 460 core
// layout(std140, binding = 1) uniform PBRMaterialProperties {
//     vec4 Color;
// } material;

// out vec4 FragColor;
// void main()
// {
//     FragColor = material.Color;
// }
//             "
//         };
//         PipelineHandle handle = device.Factory.CreatePipeline(pipelineDesc);
//         PBRMaterial asset = new PBRMaterial(new PBRMaterialProperties(Color.Red)) { Pipeline = handle };
//         return asset;
//     }

// }