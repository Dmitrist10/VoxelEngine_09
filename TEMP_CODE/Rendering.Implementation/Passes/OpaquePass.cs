using System.Collections.Generic;
using VoxelEngine.Graphics;

namespace VoxelEngine.Rendering;

public sealed class OpaquePass : IRenderPass
{
    public string Name => "Opaque Geometry Pass";
    public bool IsActive { get; set; } = true;

    // This runs during the main opaque geometry phase
    public RenderPassEvent InjectionPoint => RenderPassEvent.BeforeOpaque;

    public void Execute(RenderContext context)
    {
        IGraphicsCommandsList cmdList = context.AllocateCommandList();
        if (cmdList == null) return; // Null check for our mock
        
        cmdList.Begin();

        // 1. Bind our intermediate Color Target (NOT the screen!)
        // In a real engine, the Renderer allocates this target for the camera.
        cmdList.BindRenderTarget(context.CameraColorTarget);

        // 2. Clear if the camera asked for it
        if ((context.Camera.ClearFlags & CameraClearFlags.Color) != 0)
        {
            cmdList.ClearColor(context.Camera.ClearColor);
        }

        // 3. Draw Geometry
        foreach (var cmd in context.VisibleCommands)
        {
            // Here you would check if cmd.Material is Opaque
            // if (cmd.Material.IsOpaque) ...

            cmdList.BindMesh(cmd.Mesh.Handle);
            
            // In reality you need the index count from the mesh here!
            cmdList.DrawIndexed(0); 
        }
        
        cmdList.End();

        // Submit the recorded instructions back to the context!
        context.SubmitCommandList(this, cmdList);
    }
}
