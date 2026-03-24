// using Arch.Core;
// using VoxelEngine.Core;
// using VoxelEngine.Common;
// using VoxelEngine.Graphics;
// using VoxelEngine.Diagnostics;

// namespace VoxelEngine.Rendering;

// public sealed class EP_MeshRenderer : EntityProcessor, IRenderable
// {
//     private readonly QueryDescription _query;
//     private readonly IRenderer _renderer;

//     public EP_MeshRenderer()
//     {
//         _renderer = EngineContext.Get<IRenderer>();
//         _query = new QueryDescription().WithAll<C_Mesh, C_WorldTransformMatrix>();
//     }

//     // public override void OnInitialize()
//     // {
//     // }

//     public void OnRender()
//     {
//         // MeshRenderingQuery meshRenderingQuery = new(_renderManager);
//         // world.InlineQuery(_query, ref meshRenderingQuery);

//         world.Query(_query, (ref C_Mesh mesh, ref C_WorldTransformMatrix transform) =>
//         {
//             _renderer.Submit(new RenderCommand(
//                 mesh.Mesh,
//                 mesh.Material,
//                 transform.WorldMatrix
//             // 0,
//             // new AABB(),
//             // 0
//             ));
//         });

//     }

// }

// // public struct MeshRenderingQuery : IForEach<C_Mesh, C_WorldTransformMatrix>
// // {
// //     public RenderManager _renderManager;

// //     public MeshRenderingQuery(RenderManager renderManager)
// //     {
// //         _renderManager = renderManager;
// //     }

// //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
// //     public void Update(ref C_Mesh mesh, ref C_WorldTransformMatrix transform)
// //     {
// //         _renderManager.Submit(new RenderCommand(
// //             mesh.Mesh,
// //             mesh.IndexCount,
// //             mesh.Pipeline,
// //             mesh.MaterialUniforms,
// //             transform.WorldMatrix
// //         ));
// //     }

// // }
