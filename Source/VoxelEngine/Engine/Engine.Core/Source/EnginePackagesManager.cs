namespace VoxelEngine.Core.Runtime;

internal class EnginePackagesManager
{
    private List<IEnginePackage> _packages = new();

    public EnginePackagesManager()
    {
        EngineContext.Subscribe<E_SceneCreated>(OnSceneCreated);
        EngineContext.Subscribe<E_UniverseCreated>(OnUniverseCreated);
    }

    public void Add(IEnginePackage package)
    {
        _packages.Add(package);
    }

    public void Remove(IEnginePackage package)
    {
        _packages.Remove(package);
    }

    public void OnGameStarted()
    {
        foreach (var package in _packages)
        {
            package.OnGameStarted();
        }
    }

    private void OnSceneCreated(E_SceneCreated e)
    {
        foreach (var package in _packages)
        {
            package.OnSceneCreated(e.Scene);
        }
    }
    private void OnUniverseCreated(E_UniverseCreated e)
    {
        foreach (var package in _packages)
        {
            package.OnUniverseCreated(e.Universe);
        }
    }

    internal void OnInitialize()
    {
        foreach (var package in _packages)
        {
            package.OnInitialize();
        }
    }

    internal void Add(IEnginePackage[] packages)
    {
        _packages.AddRange(packages);
    }
}