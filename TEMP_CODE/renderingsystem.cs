using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Graphics
{
    // ==========================================
    // 1. IMPROVED INTERFACES & STRUCTS
    // ==========================================

    [Flags]
    public enum CameraClearFlags
    {
        None = 0,
        Color = 1 << 0,
        Depth = 1 << 1,
        All = Color | Depth
    }

    public enum DebugRenderMode
    {
        None,
        Wireframe,
        Unlit,
        Normals,
        Albedo
    }

    public struct CameraData
    {
        public Matrix4x4 View;
        public Matrix4x4 Projection;
        public Vector3 Position;

        public int Priority; // Lower renders first (e.g., Main Camera = 0, UI Camera = 100)
        public CameraClearFlags ClearFlags;
        public Color ClearColor;
        public uint CullingMask; // Bitmask for layers
        public RenderTargetAsset? Target; // Null means render to swapchain/screen
    }

    public struct AABB
    {
        public Vector3 Min;
        public Vector3 Max;
    }

    // ==========================================
    // ADDED: MATERIAL SYSTEM INTEGRATION
    // ==========================================

    public interface IMaterialProperties { }

    public abstract class Material : IDisposable
    {
        public PipelineHandle Pipeline;
        public string Name { get; set; } = "Material.New()";
        public bool IsTransparent { get; set; } = false; // Added to explicitly define transparency

        protected readonly IGraphicsFactory _factory;
        protected BufferHandle _BufferHandle;
        protected bool _isDirty = true;

        public const uint MATERIAL_BINDING_SLOT = 1;

        public Material(IGraphicsFactory factory)
        {
            _factory = factory;
        }

        public virtual void SetForRendering(IGraphicsCommandsList cmdBuffer)
        {
            if (_isDirty)
            {
                ApplyChangesImmediately(cmdBuffer);
                _isDirty = false;
            }

            cmdBuffer.BindPipeline(Pipeline);
            cmdBuffer.BindUniformBuffer(_BufferHandle, MATERIAL_BINDING_SLOT);
        }

        protected abstract void ApplyChangesImmediately(IGraphicsCommandsList cmdBuffer);
        public abstract void Dispose();
    }

    public class Material<T> : Material where T : unmanaged, IMaterialProperties
    {
        public T Properties;

        public Material(IGraphicsFactory factory, T properties, PipelineHandle pipeline, bool isTransparent = false) : base(factory)
        {
            Properties = properties;
            Pipeline = pipeline;
            IsTransparent = isTransparent; // Evaluated ONLY on creation, fast for sorting

            _BufferHandle = _factory.CreateBuffer(new BufferDescription()
            {
                Size = (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<T>(),
                // Usage = BufferUsage.UniformBuffer // Add your enum here
            });
        }

        public Material(IGraphicsFactory factory, PipelineHandle pipeline) : this(factory, new T(), pipeline) { }

        protected override void ApplyChangesImmediately(IGraphicsCommandsList cmdBuffer)
        {
            cmdBuffer.UpdateBuffer(_BufferHandle, 0, ref Properties);
        }

        public override void Dispose()
        {
            _factory.DestroyBuffer(_BufferHandle);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public record struct PBRMaterialProperties : IMaterialProperties
    {
        public Color Color;
        public PBRMaterialProperties() { Color = new Color(); } // Assuming Color is a struct
    }

    // Improved Render Command - Now packed for performance
    [StructLayout(LayoutKind.Sequential)]
    public readonly record struct RenderCommand(
        ulong SortKey,         // Used to sort by Material -> Mesh -> Distance
        MeshAsset Mesh,
        Material Material,
        Matrix4x4 Transform,
        AABB Bounds,
        uint Layer
    );

    // Extensions to your existing IGraphicsCommandsList
    public interface IGraphicsCommandsListExt : IGraphicsCommandsList
    {
        // Instancing is critical for AAA performance
        void DrawIndexedInstanced(uint indexCount, uint instanceCount, uint firstIndex = 0, int vertexOffset = 0, uint firstInstance = 0);

        // Fast path for pushing per-object data (like transforms) without managing buffers
        void PushConstants<T>(T data) where T : unmanaged;

        // Used for post processing / compute
        void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ);
    }

    // ==========================================
    // 2. RENDER GRAPH / PASS SYSTEM
    // ==========================================

    /// <summary>
    /// Holds the state for the current frame and camera being rendered.
    /// Passes use this to get the data they need to draw.
    /// </summary>
    public class RenderContext
    {
        public IGraphicsCommandsList CommandList;
        public CameraData Camera;
        public DebugRenderMode DebugMode;

        // Pre-culled and sorted queues for this specific camera
        public ReadOnlySpan<RenderCommand> OpaqueQueue => _opaqueQueue.AsSpan();
        public ReadOnlySpan<RenderCommand> TransparentQueue => _transparentQueue.AsSpan();

        private List<RenderCommand> _opaqueQueue = new();
        private List<RenderCommand> _transparentQueue = new();

        public void ClearQueues()
        {
            _opaqueQueue.Clear();
            _transparentQueue.Clear();
        }

        public void AddToQueue(RenderCommand cmd, bool isTransparent)
        {
            if (isTransparent) _transparentQueue.Add(cmd);
            else _opaqueQueue.Add(cmd);
        }

        public void SortQueues()
        {
            // Opaque: Front-to-Back (Minimize overdraw), then group by Material/Mesh
            _opaqueQueue.Sort((a, b) => a.SortKey.CompareTo(b.SortKey));

            // Transparent: Back-to-Front (Crucial for alpha blending)
            _transparentQueue.Sort((a, b) => b.SortKey.CompareTo(a.SortKey));
        }
    }

    public interface IRenderPass
    {
        string Name { get; }
        bool IsEnabled { get; set; }
        void Execute(RenderContext context);
    }

    // --- Example Passes ---

    public class SetupAndClearPass : IRenderPass
    {
        public string Name => "Setup & Clear";
        public bool IsEnabled { get; set; } = true;

        public void Execute(RenderContext context)
        {
            var cmd = context.CommandList;
            var cam = context.Camera;

            // If the camera has a specific target, bind it. 
            // Otherwise, we bind the Pipeline's intermediate Main Color Target, NOT the screen.
            if (cam.Target.HasValue)
                cmd.BindRenderTarget(cam.Target.Value.Handle);

            if (cam.ClearFlags.HasFlag(CameraClearFlags.Color))
                cmd.ClearColor(cam.ClearColor);
        }
    }

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

    // Unity-style Custom Feature Example
    public class ScreenSpaceOutlinePass : IRenderPass
    {
        public string Name => "Screen Space Outlines";
        public bool IsEnabled { get; set; } = false; // Toggled by user

        public void Execute(RenderContext context)
        {
            // 1. Bind an offscreen single-channel "Mask" RenderTarget
            // 2. Iterate through objects with a specific tag/layer (e.g., 'Interactable')
            // 3. Draw them using a flat white material override
            // 4. Bind the Main Color Target again
            // 5. Draw a full-screen quad with a Sobel Edge-Detection shader, reading the Mask texture
        }
    }

    public class PostProcessPass : IRenderPass
    {
        public string Name => "Post Processing";
        public bool IsEnabled { get; set; } = true;

        public Material BloomMaterial;

        public void Execute(RenderContext context)
        {
            // 1. Bind the ACTUAL Swapchain/Screen RenderTarget here
            // 2. Bind the intermediate "Main Color Target" as a Shader Texture
            // 3. Draw a Fullscreen Quad
            // context.CommandList.BindTexture(MainColorTargetTexture, 0);
            // context.CommandList.Draw(3); // Draw fullscreen triangle
        }
    }

    // ==========================================
    // 3. THE CORE RENDER MANAGER
    // ==========================================

    /// <summary>
    /// The heart of the rendering system. ECS systems record commands here, 
    /// and the Render() method orchestrates the frame.
    /// </summary>
    public class RenderPipeline
    {
        private readonly IGraphicsDevice _device;
        private readonly List<CameraData> _activeCameras = new();
        private readonly List<RenderCommand> _globalCommandPool = new();
        private readonly List<IRenderPass> _renderPasses = new();

        private RenderContext _renderContext;

        // INTERMEDIATE TARGET: This is what we render the world to before post-processing
        private RenderTargetAsset _mainColorTarget;

        public DebugRenderMode GlobalDebugMode { get; set; } = DebugRenderMode.None;

        public RenderPipeline(IGraphicsDevice device)
        {
            _device = device;
            _renderContext = new RenderContext();

            // Create offscreen target for post processing (matches window size)
            // _mainColorTarget = _device.Factory.CreateRenderTarget(Window.Width, Window.Height);

            // Assemble the Render Graph / Pipeline
            _renderPasses.Add(new SetupAndClearPass());
            _renderPasses.Add(new OpaqueGeometryPass());
            _renderPasses.Add(new ScreenSpaceOutlinePass()); // Insert custom effects here!
            _renderPasses.Add(new PostProcessPass());
        }

        // ECS Systems call this
        public void SubmitCamera(CameraData camera)
        {
            _activeCameras.Add(camera);
        }

        // ECS Systems call this
        public void SubmitRenderCommand(RenderCommand command)
        {
            _globalCommandPool.Add(command);
        }

        // Called once per frame at the end of the game loop
        public void RenderFrame()
        {
            if (_activeCameras.Count == 0) return;

            // 1. Sort cameras by Priority (e.g. Main Camera -> Weapon Overlay -> UI)
            _activeCameras.Sort((a, b) => a.Priority.CompareTo(b.Priority));

            var cmdList = _device.Factory.CreateCommandsList();
            cmdList.Begin();

            _renderContext.CommandList = cmdList;
            _renderContext.DebugMode = GlobalDebugMode;

            // 2. Loop through every camera and render its view
            foreach (var camera in _activeCameras)
            {
                _renderContext.Camera = camera;
                _renderContext.ClearQueues();

                // 3. Frustum Culling & Layer Filtering for this camera
                CullAndFilterCommands(camera);

                // 4. Sort Queues (Front-to-back, state caching)
                _renderContext.SortQueues();

                // 5. Execute all render passes for this camera
                foreach (var pass in _renderPasses)
                {
                    if (pass.IsEnabled)
                    {
                        // Note: You can add GPU timestamp queries here to profile passes
                        pass.Execute(_renderContext);
                    }
                }
            }

            cmdList.End();
            _device.Submit(cmdList);
            _device.Render();
            _device.Present();

            // Cleanup for next frame
            _activeCameras.Clear();
            _globalCommandPool.Clear();
        }

        private void CullAndFilterCommands(CameraData camera)
        {
            // Pre-calculate Frustum planes from camera.View * camera.Projection here...

            foreach (var cmd in _globalCommandPool)
            {
                // Layer Masking: Skip if the camera mask doesn't intersect the object's layer
                if ((cmd.Layer & camera.CullingMask) == 0)
                    continue;

                // Frustum Culling: Skip if AABB is outside camera frustum
                // if (!FrustumIntersects(cameraFrustum, cmd.Bounds)) continue;

                // Determine transparency efficiently using explicit material state
                bool isTransparent = cmd.Material.IsTransparent;

                // Generate a SortKey based on camera distance for proper sorting
                var sortKey = GenerateSortKey(cmd, camera.Position, isTransparent);
                var processedCmd = cmd with { SortKey = sortKey };

                _renderContext.AddToQueue(processedCmd, isTransparent);
            }
        }

        private ulong GenerateSortKey(RenderCommand cmd, Vector3 cameraPos, bool isTransparent)
        {
            // AAA Sort Key trick: Pack data into a 64-bit integer
            // High bits = Most important sorting criteria

            float distance = Vector3.DistanceSquared(cameraPos, cmd.Transform.Translation);

            // Convert distance to an integer representation (flipping bits if transparent so it sorts backwards)
            uint distInt = BitConverter.SingleToUInt32Bits(distance);
            if (isTransparent) distInt = ~distInt;

            ulong key = 0;

            // Example layout:
            // [ 32 bits Distance ] [ 16 bits Material ID ] [ 16 bits Mesh ID ]
            key |= ((ulong)distInt) << 32;
            key |= ((ulong)cmd.Material.GetHashCode() & 0xFFFF) << 16;
            key |= ((ulong)cmd.Mesh.Handle.GetHashCode() & 0xFFFF);

            return key;
        }
    }
}