using Arch.Core;

namespace VoxelEngine.Core;

public sealed partial class Scene
{
    public readonly string Name;
    public readonly uint ID;

    public readonly World World;

    private readonly ActorsRegistry _actorsRegistry;
    private readonly SceneGameServicesRegistry _servicesRegistry;
    private readonly EntityProcessorsRegistry _entityProcessorsRegistry;

    public Scene(string name, uint id)
    {
        Name = name;
        ID = id;

        World = World.Create();

        _actorsRegistry = new ActorsRegistry(this, World);
        _servicesRegistry = new SceneGameServicesRegistry(this);
        _entityProcessorsRegistry = new EntityProcessorsRegistry(this);
    }

    public override string ToString()
    {
        return $"(Scene: {Name}, ID: {ID})";
    }

}
