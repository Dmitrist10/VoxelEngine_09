// using Arch.Core;
// using VoxelEngine.Common;
// using VoxelEngine.Core;
// using VoxelEngine.Windowing;

// namespace VoxelEngine.Rendering;

// public sealed class EP_Camera : EntityProcessor, IRenderable
// {
//     private readonly IRenderer renderer;
//     private readonly IWindowSurface window;
//     private readonly QueryDescription query;

//     public EP_Camera()
//     {
//         renderer = EngineContext.Get<IRenderer>();
//         window = EngineContext.Get<IWindowSurface>();

//         query = new QueryDescription().WithAll<C_Transform, C_Camera>();
//     }


//     public override void OnInitialize()
//     {
//     }

//     public void OnRender()
//     {
//         float aspectRatio = window.Size.X / window.Size.Y;

//         world.Query(in query, (ref C_Transform transform, ref C_Camera camera) =>
//         {
//             camera.UpdateView(transform.WorldPosition, transform.WorldPosition + transform.Forward, transform.Up);
//             camera.UpdateAspectRatio(aspectRatio); // Auto update projection
//             renderer.Submit(new CameraData(camera.View, camera.Projection, camera.Priority));
//         });
//     }
// }
