namespace VoxelEngine.Core;

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
