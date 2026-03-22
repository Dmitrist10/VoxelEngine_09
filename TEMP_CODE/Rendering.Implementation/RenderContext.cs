using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VoxelEngine.Graphics;

namespace VoxelEngine.Rendering;

public sealed class RenderContext
{
    public CameraData Camera { get; }
    
    // Thread-safe dictionary to store recorded command lists per-pass
    private readonly ConcurrentDictionary<IRenderPass, List<IGraphicsCommandsList>> _recordedLists;

    // The intermediate render target that Opaque and Transparent passes draw to
    public RenderTargetHandle CameraColorTarget { get; set; }
    
    // Commands that survived frustum culling
    public IReadOnlyList<RenderCommand> VisibleCommands { get; }

    public RenderContext(CameraData camera, IReadOnlyList<RenderCommand> visibleCommands)
    {
        Camera = camera;
        VisibleCommands = visibleCommands;
        _recordedLists = new ConcurrentDictionary<IRenderPass, List<IGraphicsCommandsList>>();
    }

    /// <summary>
    /// Thread-safe way for a Pass to get a fresh command list to write to.
    /// In a Vulkan/DX12 engine, this fetches from a CommandList Pool.
    /// </summary>
    public IGraphicsCommandsList AllocateCommandList()
    {
        // TODO: Get from GraphicsDevice.Factory or a CommandListPool
        return null; 
    }

    /// <summary>
    /// Thread-safe way for a Pass to submit its finished command list back to the context.
    /// </summary>
    public void SubmitCommandList(IRenderPass pass, IGraphicsCommandsList list)
    {
        _recordedLists.AddOrUpdate(pass, 
            new List<IGraphicsCommandsList> { list }, 
            (key, existing) => { existing.Add(list); return existing; });
    }

    public IEnumerable<IGraphicsCommandsList> GetRecordedListsForPass(IRenderPass pass)
    {
        if (_recordedLists.TryGetValue(pass, out var lists))
            return lists;
        return Array.Empty<IGraphicsCommandsList>();
    }
}