namespace VoxelEngine.Physics.Adapter;

public class ModulePhysics : IPhysicsModule
{
    public string Name => "Physics.Adapter";
    public Version Version => new Version(1, 0, 0);

    public void OnInitialize()
    {
        Console.WriteLine("Physics.Adapter initialized");
    }

    public void OnCleanUp()
    {
        Console.WriteLine("Physics.Adapter cleaned up");
    }
}