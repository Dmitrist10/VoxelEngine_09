using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxelEngine.Common;
using VoxelEngine.Graphics;

namespace VoxelEngine.Rendering;

/// <summary>
/// This is a high-level representation of a modern AAA Rendering Pipeline.
/// It shows how multithreading, multiple cameras, passes, and hardware submission work together.
/// </summary>
public sealed class ModernRendererExample : IRenderer
{
    private readonly IGraphicsDevice _graphicsDevice;
    private readonly List<CameraData> _cameras;
    private readonly List<LocalRenderCommand> _renderCommands;
    
    // The defined passes for our pipeline, in topological order.
    // 0: Setup/Shadows, 1: Opaque, 2: Skybox, 3: Transparent, 4: PostProcess, 5: UI
    private readonly List<IRenderPassExample> _renderPasses;

    public ModernRendererExample(IGraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _cameras = new List<CameraData>();
        _renderCommands = new List<LocalRenderCommand>(4096);
        _renderPasses = new List<IRenderPassExample>();
        
        // Initialize passes in the exact order we want them to be submitted to the GPU
        // _renderPasses.Add(new ShadowPass());
        // _renderPasses.Add(new OpaquePass());
        // _renderPasses.Add(new PostProcessPass());
    }

    public void Submit(RenderCommand renderCommand)
    {
        // Just store the command for now. We process it during RenderFrame.
        // E.g., convert to LocalRenderCommand with bounds.
    }

    public void Submit(CameraData cameraData)
    {
        _cameras.Add(cameraData);
    }

    public void RenderFrame()
    {
        // 1. Sort cameras by Priority. 
        // e.g. Priority 0 = Main Game Camera. Priority 1 = UI Overlay Camera.
        _cameras.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        // Let's pretend we have a pool to get command lists from
        var commandListPool = new CommandListPoolExample(_graphicsDevice);

        // 2. Execute the Pipeline per Camera
        foreach (var camera in _cameras)
        {
            // A. Culling and Sorting for THIS camera
            var visibleCommands = CullAndSortCommands(camera);

            // B. Create a Context. The context holds the commands and a way to get command lists
            var context = new RenderContextExample(camera, visibleCommands, commandListPool);

            // C. Multithreaded CPU Recording! 
            // The ThreadPool will execute these passes simultaneously. 
            // Because each pass requests its OWN CommandList from the Context, there are no thread collisions!
            Parallel.ForEach(_renderPasses, pass =>
            {
                if (pass.IsActive)
                {
                    // Execute runs on a background thread. It records commands into a list.
                    pass.Execute(context);
                }
            });

            // --- At this point, all CPU threads have finished writing their command lists ---

            // D. Submission to GPU (Main Thread)
            // We iterate through passes IN ORDER and submit their recorded command lists.
            List<IGraphicsCommandsList> listsToSubmit = new List<IGraphicsCommandsList>();
            
            foreach (var pass in _renderPasses)
            {
                if (pass.IsActive)
                {
                    // Since passes execute in parallel, they deposit their finished command lists
                    // back into the context, but we pull them out IN THE CORRECT ORDER here!
                    var listsForPass = context.GetRecordedListsForPass(pass);
                    listsToSubmit.AddRange(listsForPass);
                }
            }

            // E. Send the ordered lists to the GPU queue
            _graphicsDevice.Submit(listsToSubmit.ToArray());
        }

        // 3. Swap the Backbuffer to the Screen Monitor
        _graphicsDevice.Present();

        // 4. Cleanup for next frame
        _cameras.Clear();
        _renderCommands.Clear();
    }

    private List<LocalRenderCommand> CullAndSortCommands(CameraData camera)
    {
        // 1. Frustum Culling
        // 2. Create Sorting Keys
        // 3. Sort Opaque front-to-back, Transparent back-to-front
        return _renderCommands; // Mock
    }
}


// --- SUPPORTING STRUCTURES BEYOND THIS POINT ---

public interface IRenderPassExample
{
    string Name { get; }
    bool IsActive { get; set; }
    
    // Notice NO Submit() method. 
    // Passes don't submit themselves. They just record instructions into the context.
    void Execute(RenderContextExample context);
}

public class RenderContextExample
{
    public CameraData Camera { get; }
    public IReadOnlyList<LocalRenderCommand> VisibleCommands { get; }
    
    private readonly CommandListPoolExample _pool;
    
    // We use a concurrent dictionary to store finished lists per pass, since Passes execute on different threads.
    private readonly System.Collections.Concurrent.ConcurrentDictionary<IRenderPassExample, List<IGraphicsCommandsList>> _recordedLists;

    public RenderContextExample(CameraData camera, IReadOnlyList<LocalRenderCommand> visibleCommands, CommandListPoolExample pool)
    {
        Camera = camera;
        VisibleCommands = visibleCommands;
        _pool = pool;
        _recordedLists = new();
    }

    /// <summary>
    /// Thread-safe way for a Pass to get a fresh command list to write to.
    /// </summary>
    public IGraphicsCommandsList AllocateCommandList()
    {
        return _pool.GetOrCreate();
    }

    /// <summary>
    /// Thread-safe way for a Pass to submit its finished command list back to the context.
    /// </summary>
    public void SubmitCommandList(IRenderPassExample pass, IGraphicsCommandsList list)
    {
        _recordedLists.AddOrUpdate(pass, 
            new List<IGraphicsCommandsList> { list }, 
            (key, existingList) => { existingList.Add(list); return existingList; });
    }

    public IEnumerable<IGraphicsCommandsList> GetRecordedListsForPass(IRenderPassExample pass)
    {
        if (_recordedLists.TryGetValue(pass, out var lists))
            return lists;
        return Array.Empty<IGraphicsCommandsList>();
    }
}

/// <summary>
/// A dummy mock pool of command lists. Creating command lists is slow, so we recycle them.
/// </summary>
public class CommandListPoolExample
{
    private IGraphicsDevice _device;
    public CommandListPoolExample(IGraphicsDevice device) => _device = device;
    public IGraphicsCommandsList GetOrCreate() => _device.Factory.CreateGraphicsCommandsList(); // Mock creation
}

// -----------------------------------------------------
// EXAMPLE REAL-WORLD PASSES
// -----------------------------------------------------

public class OpaquePass : IRenderPassExample
{
    public string Name => "Opaque Pass";
    public bool IsActive { get; set; } = true;

    public void Execute(RenderContextExample context)
    {
        // 1. Get a fresh command list from the pool
        IGraphicsCommandsList cmd = context.AllocateCommandList();
        
        cmd.Begin();

        // 2. Handle Clearing depending on the Camera's settings!
        // If this is a UI camera on top of the game, ClearFlags might be "None".
        // But if it's the main camera, we MUST clear.
        if (context.Camera.Target != null)
        {
            cmd.BindRenderTarget(context.Camera.Target.Value.Handle);
        }
        
        if ((context.Camera.ClearFlags & CameraClearFlags.Color) != 0)
        {
            cmd.ClearColor(context.Camera.ClearColor);
        }

        // 3. Execute all Opaque draw calls assigned to this camera
        foreach (var renderItem in context.VisibleCommands)
        {
            // Only render Opaque materials that match the camera's Layer mask
            if (/* is opaque */ true) 
            {
               cmd.BindMesh(renderItem.Mesh.Handle);
               // cmd.BindPipeline...
               cmd.DrawIndexed(0); // Dummy draw
            }
        }
        
        cmd.End();

        // 4. Submit the recorded instructions back to the context!
        context.SubmitCommandList(this, cmd);
    }
}
