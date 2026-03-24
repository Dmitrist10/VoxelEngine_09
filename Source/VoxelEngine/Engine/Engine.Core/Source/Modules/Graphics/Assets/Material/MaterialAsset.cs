using VoxelEngine.Assets;
using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public abstract class Material : IAsset
{
    public PipelineHandle Pipeline;

    public string Name { get; set; } = "Material.New()";
    public bool IsTransparent = false;

    protected readonly IGraphicsFactory _factory;
    public BufferHandle BufferHandle;
    protected bool _isDirty = true; // Dirty by default on creation

    public const uint MATERIAL_BINDING_SLOT = 1;

    public Material()
    {
        _factory = EngineContext.Get<IGraphicsDevice>().Factory;
    }

    public virtual void SetForRendering(IGraphicsCommandsList cmdBuffer)
    {
        if (_isDirty)
        {
            ApplyChangesImmediately(cmdBuffer);
            _isDirty = false;
        }

        cmdBuffer.BindPipeline(Pipeline);
        cmdBuffer.BindUniformBuffer(BufferHandle, MATERIAL_BINDING_SLOT);
    }


    // public virtual T Clone<T>() where T : Material
    // {
    //     return (T)MemberwiseClone();
    // }

    public void ApplyChanges() => _isDirty = true;
    protected virtual void ApplyChangesImmediately(IGraphicsCommandsList cmdBuffer) { }

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

    public Material(T properties, PipelineHandle pipeline) : base()
    {
        Properties = properties;
        Pipeline = pipeline;

        BufferHandle = _factory.CreateBuffer(new BufferDescription()
        {
            Size = (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<T>(),
            Usage = BufferUsage.UniformBuffer
        });
    }
    public Material(PipelineHandle pipeline) : this(new T(), pipeline) { }

    protected override void ApplyChangesImmediately(IGraphicsCommandsList cmdBuffer)
    {
        cmdBuffer.UpdateBuffer(BufferHandle, 0, ref Properties);
    }
}
