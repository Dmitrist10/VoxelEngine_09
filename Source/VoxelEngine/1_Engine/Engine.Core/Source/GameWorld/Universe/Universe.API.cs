using System.Diagnostics.CodeAnalysis;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core;

public sealed partial class Universe
{



    public Scene Create(string name = "scene_01")
    {
        Scene s = new(name, (uint)_scenes.Count);
        _scenes.Add(s);
        return s;
    }

    public void RemoveScene(Scene scene)
    {
        _scenes.Remove(scene);
        Logger.ExtraInfo("A Scene was destroyed");
        // _universeManager.CallSceneDestroyed(scene);
    }


    public bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : UniverseGameService
    {
        service = _servicesRegistry.GetService<T>();
        return service != null;
    }
    public T? GetService<T>() where T : UniverseGameService
    {
        return _servicesRegistry.GetService<T>();
    }
    public T AddService<T>() where T : UniverseGameService, new()
    {
        return _servicesRegistry.AddService(new T() { universe = this });
    }
    public T AddService<T>(T service) where T : UniverseGameService
    {
        return _servicesRegistry.AddService(service);
    }
    public void RemoveService<T>() where T : UniverseGameService
    {
        _servicesRegistry.RemoveService<T>();
    }
    public bool HasService<T>() where T : UniverseGameService
    {
        return _servicesRegistry.HasService<T>();
    }

}