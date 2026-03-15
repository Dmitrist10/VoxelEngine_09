using Arch.Core;

namespace VoxelEngine.Core;


public abstract class EntityProcessor : IEntityProcessor
{
    public required Scene scene { get; init; }
    public World world => scene.World;

    // public virtual void OnInitialize() { }
    public abstract void OnInitialize();
    public virtual void OnShutDown() { }
}