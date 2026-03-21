// using System.Numerics;

// namespace VoxelEngine.Rendering;


// public sealed class Renderer : IRenderer
// {
//     private List<CameraData> _cameras;
//     private List<RenderCommand> _renderCommands;


//     public Renderer()
//     {
//         _renderCommands = new List<RenderCommand>(2048 * 2);
//         _cameras = new List<CameraData>();
//     }

//     public void RenderFrame()
//     {


//         _renderCommands.Clear();
//         _cameras.Clear();
//     }

//     private void CullAndFilterCommands(CameraData camera)
//     {
//         // Pre-calculate Frustum planes from camera.View * camera.Projection here...

//         foreach (var cmd in _renderCommands)
//         {
//             // Layer Masking: Skip if the camera mask doesn't intersect the object's layer
//             if ((cmd.Layer & camera.CullingMask) == 0)
//                 continue;

//             // Frustum Culling: Skip if AABB is outside camera frustum
//             // if (!FrustumIntersects(cameraFrustum, cmd.Bounds)) continue;

//             bool isTransparent = cmd.Material.IsTransparent;

//             // Generate a SortKey based on camera distance for proper sorting
//             var sortKey = GenerateSortKey(cmd, camera.Position, isTransparent);
//             var processedCmd = cmd with { SortingKey = sortKey };

//             _renderContext.AddToQueue(processedCmd, isTransparent);
//         }
//     }

//     private ulong GenerateSortKey(RenderCommand cmd, Vector3 cameraPos, bool isTransparent)
//     {
//         float distance = Vector3.DistanceSquared(cameraPos, cmd.Transform.Translation);

//         // Convert distance to an integer representation (flipping bits if transparent so it sorts backwards)
//         uint distInt = BitConverter.SingleToUInt32Bits(distance);
//         if (isTransparent) distInt = ~distInt;

//         ulong key = 0;

//         // Example layout:
//         // [ 16 bits Material Pipeline ] [ 16 bits Material Buffer ] [ 16 bits Mesh ] [ 16 bits Distance ]
//         key |= ((ulong)cmd.Material.Pipeline.Handle.Index & 0xFFFF) << 48;
//         key |= ((ulong)cmd.Material.BufferHandle.Handle.Index & 0xFFFF) << 32;
//         key |= ((ulong)cmd.Mesh.Handle.Handle.Index & 0xFFFF) << 16;
//         key |= distInt;

//         // key |= cmd.Mesh.Handle.Handle.Index << 16;

//         return key;
//     }

//     [MethodImpl(AggressiveInlining)]
//     public void Submit(RenderCommand renderCommand)
//     {
//         _renderCommands.Add(renderCommand);
//     }

//     [MethodImpl(AggressiveInlining)]
//     public void Submit(CameraData cameraData)
//     {
//         _cameras.Add(cameraData);
//     }

// }



using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Common;

namespace VoxelEngine.Rendering;

public sealed class Renderer : IRenderer
{
    private List<CameraData> _cameras;
    private List<RenderCommand> _renderCommands;

    private RenderContext _renderContext;

    public Renderer()
    {
        _renderCommands = new List<RenderCommand>(2048 * 2);
        _cameras = new List<CameraData>();
        _renderContext = new RenderContext();
    }

    public void RenderFrame()
    {

    }

    public void Submit(RenderCommand renderCommand)
    {
        _renderCommands.Add(renderCommand);
    }
    public void Submit(CameraData cameraData)
    {
        _cameras.Add(cameraData);
    }

}

public sealed class RenderContext
{
    private List<RenderCommand> _opaqueCommands;
    private List<IGraphicsCommandsList> _graphicsCommandsList;

    // private List<RenderCommand> _transparentCommands;

    public RenderContext()
    {
        _opaqueCommands = new List<RenderCommand>(2048 * 2);
        _graphicsCommandsList = new List<IGraphicsCommandsList>(16);
    }

}

public interface IRenderPass
{
    string Name { get; }
    bool IsActive { get; set; }
    void Execute(RenderContext context);
    void Submit();
}

public sealed class SetUpRenderPass : IRenderPass
{
    public string Name => "Setup Pass";

    public bool IsActive { get; set; } = true;

    public void Execute(RenderContext context)
    {

    }


}