using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace VoxelEngine.Rendering;

public sealed class OpaquePass : IRenderPass
{

    private const uint MODEL_BINDING_SLOT = 2;
    private const uint CAMERA_BINDING_SLOT = 0;
    private const uint LIGHT_BINDING_SLOT = 3;

    private BufferHandle _modelBuffer;
    private BufferHandle _cameraBuffer;
    private BufferHandle _lightBuffer;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 16)]
    private struct CameraBlock
    {
        public Matrix4x4 View;        // 64
        public Matrix4x4 Projection;  // 64
        public Vector4 Position;      // 16
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 16)]
    private struct LightBlock
    {
        public Vector4 Direction; // 16: xyz: dir, w: padding
        public Vector4 Color;     // 16: rgb: color, w: ambient intensity
        public Vector4 Specular;  // 16: x: intensity, y: shininess, zw: padding
    }

    public void Initialize()
    {
        var factory = EngineContext.Get<IGraphicsDevice>().Factory;

        _modelBuffer = factory.CreateBuffer(new BufferDescription()
        {
            Size = (uint)Unsafe.SizeOf<Matrix4x4>(),
            Usage = BufferUsage.UniformBuffer
        });

        _cameraBuffer = factory.CreateBuffer(new BufferDescription()
        {
            Size = (uint)Unsafe.SizeOf<CameraBlock>(),
            Usage = BufferUsage.UniformBuffer
        });

        _lightBuffer = factory.CreateBuffer(new BufferDescription()
        {
            Size = (uint)Unsafe.SizeOf<LightBlock>(),
            Usage = BufferUsage.UniformBuffer
        });
    }

    public void Execute(in RenderContext renderContext)
    {
        var commands = renderContext.CreateGraphicsCommandsList();

        commands.Begin();

        // Get camera position from view matrix
        Matrix4x4.Invert(renderContext.camera.view, out Matrix4x4 invView);

        CameraBlock cameraBlock = new CameraBlock
        {
            View = renderContext.camera.view,
            Projection = renderContext.camera.projection,
            Position = new Vector4(invView.Translation, 1.0f)
        };
        commands.UpdateBuffer(_cameraBuffer, 0, ref cameraBlock);
        commands.BindUniformBuffer(_cameraBuffer, CAMERA_BINDING_SLOT);

        // Fetch lighting from global service
        var lighting = EngineContext.Get<LightingService>();

        LightBlock lightBlock = new LightBlock
        {
            Direction = new Vector4(lighting.Direction, 0.0f),
            Color = new Vector4(lighting.Color, lighting.AmbientIntensity),
            Specular = new Vector4(lighting.SpecularIntensity, lighting.Shininess, 0.0f, 0.0f)
        };
        commands.UpdateBuffer(_lightBuffer, 0, ref lightBlock);
        commands.BindUniformBuffer(_lightBuffer, LIGHT_BINDING_SLOT);

        foreach (var c in renderContext.renderCommands)
        {
            Matrix4x4 worldMatrix = c.Transform;
            commands.UpdateBuffer(_modelBuffer, 0, ref worldMatrix);
            commands.BindUniformBuffer(_modelBuffer, MODEL_BINDING_SLOT);

            c.Material.SetForRendering(commands);
            commands.BindMesh(c.Mesh.Handle);
            commands.DrawIndexed(c.Mesh.IndexCount);
        }

        commands.End();

        renderContext.Submit(commands);
    }


}