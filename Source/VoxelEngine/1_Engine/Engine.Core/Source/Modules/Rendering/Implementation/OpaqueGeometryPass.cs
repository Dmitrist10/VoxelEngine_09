namespace VoxelEngine.Rendering;

public class OpaqueGeometryPass : IRenderPass
{
    public string Name => "Opaque Geometry";
    public bool IsEnabled { get; set; } = true;

    public void Execute(RenderContext context)
    {
        var cmd = (IGraphicsCommandsListExt)context.CommandList;

        Material lastMaterial = null;
        MeshAsset lastMesh = default;

        // Handle Wireframe Debug Mode (Assuming dynamic state toggle for simplicity)
        bool isWireframe = context.DebugMode == DebugRenderMode.Wireframe;
        // if (isWireframe) cmd.SetRasterizerState(FillMode.Wireframe); 

        foreach (ref readonly var renderCmd in context.OpaqueQueue)
        {
            // Minimize state changes (State Caching)
            if (lastMaterial != renderCmd.Material)
            {
                renderCmd.Material.SetForRendering(cmd);
                lastMaterial = renderCmd.Material;
            }

            if (lastMesh.Handle != renderCmd.Mesh.Handle)
            {
                cmd.BindMesh(renderCmd.Mesh.Handle);
                lastMesh = renderCmd.Mesh;
            }

            // Push transform via Push Constants for massive performance gains
            cmd.PushConstants(renderCmd.Transform);

            cmd.DrawIndexed(renderCmd.Mesh.IndexCount);
        }

        // if (isWireframe) cmd.SetRasterizerState(FillMode.Solid); // Reset state
    }
}