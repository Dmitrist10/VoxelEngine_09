using VoxelEngine.Graphics;

namespace VoxelEngine.Rendering;

// public class RenderContext
// {
//     public IGraphicsCommandsList CommandList;
//     public CameraData Camera;
//     public DebugRenderMode DebugMode;

//     private List<RenderCommand> _opaqueQueue = new();
//     private List<RenderCommand> _transparentQueue = new();

//     public ReadOnlySpan<RenderCommand> OpaqueQueue => _opaqueQueue.ToArray().AsSpan();
//     public ReadOnlySpan<RenderCommand> TransparentQueue => _transparentQueue.ToArray().AsSpan();

//     internal RenderContext(IGraphicsCommandsList commandList)
//     {
//         CommandList = commandList;
//     }

//     internal void ClearQueues()
//     {
//         _opaqueQueue.Clear();
//         _transparentQueue.Clear();
//     }

//     [MethodImpl(AggressiveInlining)]
//     internal void AddToQueue(RenderCommand cmd, bool isTransparent)
//     {
//         if (isTransparent) _transparentQueue.Add(cmd);
//         else _opaqueQueue.Add(cmd);
//     }

//     internal void SortQueues()
//     {
//         // Opaque: Front-to-Back (Minimize overdraw), then group by Material/Mesh
//         _opaqueQueue.Sort((a, b) => a.SortingKey.CompareTo(b.SortingKey));

//         // Transparent: Back-to-Front (Crucial for alpha blending)
//         _transparentQueue.Sort((a, b) => b.SortingKey.CompareTo(a.SortingKey));
//     }
// }