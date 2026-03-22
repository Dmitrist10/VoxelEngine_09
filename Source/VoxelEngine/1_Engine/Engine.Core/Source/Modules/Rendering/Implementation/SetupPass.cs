using VoxelEngine.Common;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Rendering;

public sealed class SetupPass : IRenderPass
{


    public void Execute(in RenderContext renderContext)
    {
        var commands = renderContext.CreateGraphicsCommandsList();

        commands.Begin();

        // commands.SetViewport(0, 0, renderContext.windowSurface.Width, renderContext.windowSurface.Height);
        // commands.SetScissor(0, 0, renderContext.windowSurface.Width, renderContext.windowSurface.Height);

        commands.ClearColor(Color.Gray);

        commands.End();

        renderContext.Submit(commands);
    }


}
