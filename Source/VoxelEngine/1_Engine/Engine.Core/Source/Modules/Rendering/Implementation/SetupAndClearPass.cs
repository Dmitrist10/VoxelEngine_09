// namespace VoxelEngine.Rendering;

// public class SetupAndClearPass : IRenderPass
// {
//     public string Name => "Setup & Clear";
//     public bool IsEnabled { get; set; } = true;

//     public void Execute(RenderContext context)
//     {
//         var cmd = context.CommandList;
//         var cam = context.Camera;

//         // If the camera has a specific target, bind it. 
//         // Otherwise, we bind the Pipeline's intermediate Main Color Target, NOT the screen.
//         if (cam.Target.HasValue)
//             cmd.BindRenderTarget(cam.Target.Value.Handle);

//         if (cam.ClearFlags.HasFlag(CameraClearFlags.Color))
//             cmd.ClearColor(cam.ClearColor);
//     }
// }
