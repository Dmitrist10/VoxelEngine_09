namespace VoxelEngine.Core;

public sealed partial class Universe
{

    private readonly List<Scene> _scenes = new();
    private readonly UniverseGameServicesRegistry _servicesRegistry;
    private readonly UniverseManager _universeManager;

    public Universe(UniverseManager universeManager)
    {
        _universeManager = universeManager;
        _servicesRegistry = new UniverseGameServicesRegistry(this);
    }



}