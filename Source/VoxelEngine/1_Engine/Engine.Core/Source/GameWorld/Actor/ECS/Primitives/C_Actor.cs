namespace VoxelEngine.Core;

[NoInspect]
public record struct C_Actor : IComponent
{
    // public Guid Scene;
    public string Name = "Actor";

    public C_Actor()
    {
    }
    public C_Actor(string name)
    {
        Name = name;
    }

}
