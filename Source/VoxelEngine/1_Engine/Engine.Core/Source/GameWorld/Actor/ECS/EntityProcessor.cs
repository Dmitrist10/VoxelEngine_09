using Arch.Core;

namespace VoxelEngine.Core;


public abstract class EntityProcessor : IEntityProcessor
{
    public Scene scene { get; internal set; } = null!;
    public World world => scene.World;

    public virtual ProcessorGroup Group { get; protected set; } = ProcessorGroup.Engine;
    public virtual void OnInitialize() { }
    public virtual void OnShutDown() { }
}