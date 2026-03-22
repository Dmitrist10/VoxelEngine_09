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

namespace VoxelEngine.Rendering;

public sealed class Renderer : IRenderer
{
    private readonly IGraphicsDevice _graphicsDevice;
    private readonly List<CameraData> _cameras;
    private readonly List<RenderCommand> _renderCommands;
    
    private readonly List<IRenderPass> _renderPasses;

    public Renderer()
    {
        _graphicsDevice = EngineContext.Get<IGraphicsDevice>();
        _cameras = new List<CameraData>();
        _renderCommands = new List<RenderCommand>(4096);
        _renderPasses = new List<IRenderPass>();
        
        // Add default engine passes here
        AddPass(new OpaquePass()); // <= Added Opaque Pass
        AddPass(new CompositePass());
    }

    /// <summary>
    /// Registers a pass and sorts it automatically into the correct timeline position.
    /// External UGC (User Generated Content) can call this to inject passes!
    /// </summary>
    public void AddPass(IRenderPass pass)
    {
        _renderPasses.Add(pass);
        // Sort passes by their Injection Point so they always run in the correct chronological order!
        _renderPasses.Sort((a, b) => a.InjectionPoint.CompareTo(b.InjectionPoint));
    }

    public void Submit(RenderCommand renderCommand)
    {
        _renderCommands.Add(renderCommand);
    }

    public void Submit(CameraData cameraData)
    {
        _cameras.Add(cameraData);
    }

    public void RenderFrame()
    {
        // 1. Sort cameras by priority
        _cameras.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        // 2. Loop over every camera and execute the pass pipeline
        foreach (var camera in _cameras)
        {
            // Fake culling - just use all commands for now
            IReadOnlyList<RenderCommand> visibleCommands = _renderCommands;

            var context = new RenderContext(camera, visibleCommands);
            
            // In a real engine, Renderer allocates a temporary RenderTarget (GBuffer) for this camera
            // context.CameraColorTarget = _graphicsDevice.Factory.CreateRenderTarget(...);

            // Multithreaded command recording!
            Parallel.ForEach(_renderPasses, pass =>
            {
                if (pass.IsActive)
                {
                    pass.Execute(context);
                }
            });

            // Gather all the recorded commands IN THE CORRECT ORDER
            var listsToSubmit = new List<IGraphicsCommandsList>();
            foreach (var pass in _renderPasses)
            {
                if (pass.IsActive)
                {
                    listsToSubmit.AddRange(context.GetRecordedListsForPass(pass));
                }
            }

            // Submit to GPU
            _graphicsDevice.Submit(listsToSubmit.ToArray());
        }

        // Present the final visual to the monitor
        _graphicsDevice.Present();

        _cameras.Clear();
        _renderCommands.Clear();
    }
}