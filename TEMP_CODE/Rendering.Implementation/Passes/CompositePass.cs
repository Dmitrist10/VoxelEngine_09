using VoxelEngine.Graphics;

namespace VoxelEngine.Rendering;

/// <summary>
/// This is an example of the final engine pass. It takes the heavily-rendered GBuffer/HDR texture 
/// and simply draws it onto the screen (the Swapchain Backbuffer).
/// </summary>
public sealed class CompositePass : IRenderPass
{
    public string Name => "Composite To Screen Pass";
    public bool IsActive { get; set; } = true;
    
    // This MUST run at the very end of everything except UI
    public RenderPassEvent InjectionPoint => RenderPassEvent.BeforeUI;

    public void Execute(RenderContext context)
    {
        // 1. Get List
        IGraphicsCommandsList cmdList = context.AllocateCommandList();
        
        // Safety check if we have a pool configured
        if (cmdList == null) return; 

        cmdList.Begin();

        // 2. Bind the MAIN Screen Render Target
        // A null or special Handle tells the Graphics API to use the OS Window Backbuffer
        // cmdList.BindRenderTarget(BuiltIn.Swapchain);

        // 3. Optional Clear based on Camera
        if ((context.Camera.ClearFlags & CameraClearFlags.Color) != 0)
        {
            cmdList.ClearColor(context.Camera.ClearColor);
        }

        // 4. Bind the texture we spent the whole frame rendering to! (The intermediate CameraColorTarget)
        // Here we give the target to the shader so it can sample from it
        // cmdList.BindTexture(new TextureHandle(context.CameraColorTarget.Id)); // Use appropriate handle conversion

        // 5. Draw a Full-Screen Triangle to cover the screen
        cmdList.DrawIndexed(3);

        cmdList.End();

        // 6. Submit back to context
        context.SubmitCommandList(this, cmdList);
    }
}
