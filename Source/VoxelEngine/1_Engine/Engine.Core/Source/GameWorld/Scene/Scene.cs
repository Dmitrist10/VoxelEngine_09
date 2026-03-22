using Arch.Core;
using VoxelEngine.Rendering;

namespace VoxelEngine.Core;

public sealed partial class Scene
{
    public readonly string Name;
    public readonly uint ID;

    public readonly World World;
    public readonly Universe Universe;

    private readonly ActorsRegistry _actorsRegistry;
    private readonly SceneGameServicesRegistry _servicesRegistry;
    private readonly EntityProcessorsRegistry _entityProcessorsRegistry;

    internal Scene(Universe universe, string name, uint id)
    {
        Universe = universe;
        Name = name;
        ID = id;

        World = World.Create();

        _actorsRegistry = new ActorsRegistry(this, World);
        _servicesRegistry = new SceneGameServicesRegistry(this);
        _entityProcessorsRegistry = new EntityProcessorsRegistry(this);

        AddProcessor<EP_Transform>();
        AddProcessor<EP_Camera>();
        AddProcessor<EP_MeshRenderer>();
    }

    public override string ToString()
    {
        return $"(Scene: {Name}, ID: {ID})";
    }

}
