namespace VoxelEngine.Core;

// [OnlyView]
[NoInspect]
public record struct C_ID : IComponent
{
    public Guid ID;

    public C_ID()
    {
        ID = Guid.NewGuid();
    }
    public C_ID(Guid id)
    {
        ID = id;
    }
}
