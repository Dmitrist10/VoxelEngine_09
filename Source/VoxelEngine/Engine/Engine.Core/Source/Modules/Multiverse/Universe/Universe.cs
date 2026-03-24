using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core;

public sealed partial class Universe
{
    private readonly Multiverse _multiverse;
    public Multiverse Multiverse => _multiverse;

    private readonly List<Scene> _scenes = new();
    public IReadOnlyList<Scene> Scenes => _scenes;

    public Universe(Multiverse multiverse)
    {
        _multiverse = multiverse;
    }

    public Scene CreateScene()
    {
        Scene scene = new(this);
        _scenes.Add(scene);
        scene.OnInitialize();
        
        EngineContext.Publish(new E_SceneCreated(scene));
        return scene;
    }
    public void DestroyScene(Scene scene)
    {
        _scenes.Remove(scene);
        scene.OnShutdown();
    }

    // public Scene GetScene(int index)
    // {
    //     return _scenes[index];
    // }

    public void OnInitialize()
    {
        foreach (var scene in _scenes)
        {
            scene.OnInitialize();
        }
    }
    public void OnShutdown()
    {
        foreach (var scene in _scenes)
        {
            scene.OnShutdown();
        }
    }

    public void OnUpdate()
    {
        foreach (var scene in _scenes)
        {
            scene.OnUpdate();
        }
    }

    public void OnFixedUpdate()
    {
        foreach (var scene in _scenes)
        {
            scene.OnFixedUpdate();
        }
    }

    public void OnTick()
    {
        foreach (var scene in _scenes)
        {
            scene.OnTick();
        }
    }

    public void OnRender()
    {
        foreach (var scene in _scenes)
        {
            scene.OnRender();
        }

    }


}
