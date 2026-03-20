using VoxelEngine.Assets;
using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public abstract class Material : IAsset
{
    public required PipelineHandle Pipeline;

    public string Name { get; set; } = "Material.New()";

    protected readonly IGraphicsFactory _factory;
    protected BufferHandle _BufferHandle;
    protected bool _isDirty = true; // Dirty by default on creation

    public const uint MATERIAL_BINDING_SLOT = 1;

    public virtual void SetRendering(IGraphicsCommandsList cmdBuffer)
    {
        if (_isDirty)
        {
            ApplyChangesCommand(cmdBuffer);
            _isDirty = false;
        }

        cmdBuffer.BindPipeline(Pipeline);
        cmdBuffer.BindUniformBuffer(_BufferHandle, MATERIAL_BINDING_SLOT);
    }

    protected virtual void ApplyChangesCommand(IGraphicsCommandsList cmdBuffer) { }

    public Material()
    {
        _factory = EngineContext.Get<IGraphicsDevice>().Factory;
    }

    public virtual void Dispose()
    {
        // _factory.DestroyBuffer(_BufferHandle);
    }

    // ~Material()
    // {
    //     Dispose();
    // }

}

public class Material<T> : Material, IDisposable where T : unmanaged, IMaterialProperties
{
    public T Properties;

    public Material(T properties) : base()
    {
        Properties = properties;

        _BufferHandle = _factory.CreateBuffer(new BufferDescription()
        {
            Size = (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<T>(),
            Usage = BufferUsage.UniformBuffer
        });
    }
    public Material() : this(new T()) { }

    public void ApplyChanges()
    {
        _isDirty = true;
    }

    protected override void ApplyChangesCommand(IGraphicsCommandsList cmdBuffer)
    {
        cmdBuffer.UpdateBuffer(_BufferHandle, 0, ref Properties);
    }
}
