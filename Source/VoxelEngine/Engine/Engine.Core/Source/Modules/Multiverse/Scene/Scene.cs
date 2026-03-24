using System.Diagnostics.CodeAnalysis;
using Arch.Core;
using VoxelEngine.Rendering;

namespace VoxelEngine.Core;

public sealed partial class Scene
{
    private Universe universe;
    public Universe Universe => universe;

    public World World { get; private set; }
    public string Name { get; internal set; }

    private ActorsRegistry _actorsRegistry;
    private readonly SEntityProcessorGlobalGroup _entityProcessors;

    public Scene(Universe universe, string name = "Scene")
    {
        this.universe = universe;
        Name = name;

        World = World.Create();
        _actorsRegistry = new(this, World);
        _entityProcessors = new()
        {
            scene = this,
            multiverse = universe.Multiverse
        };

        SEPGroup engineProcessors = CreateEPGroup();
        engineProcessors.Add<EP_MeshBounds>();
        engineProcessors.Add<EP_Transform>();

        SEPGroup renderingProcessors = CreateEPGroup();
        renderingProcessors.Add<EP_MeshRenderer>();
        renderingProcessors.Add<EP_Camera>();

        _entityProcessors.Add(engineProcessors);
        _entityProcessors.Add(renderingProcessors);
    }

    public SEPGroup CreateEPGroup()
    {
        SEPGroup group = new()
        {
            scene = this,
            multiverse = universe.Multiverse
        };

        return group;
    }
    public T CreateEPGroup<T>() where T : SEntityProcessorGroup, new()
    {
        T group = new()
        {
            scene = this,
            multiverse = universe.Multiverse
        };

        return group;
    }
    public T CreateEP<T>() where T : SEntityProcessor, new()
    {
        T group = new()
        {
            scene = this,
            multiverse = universe.Multiverse
        };

        return group;
    }


    // Actors
    public Actor CreateActor() => _actorsRegistry.CreateActor();
    public Entity CreateEntity() => _actorsRegistry.CreateEntity();

    public uint GetEntityCount() => _actorsRegistry.EntitysCount;

    // public void Destroy(Entity e) => _commandBuffer.Enqueue(() => _actorsRegistry.Destroy(e));
    // public void Destroy(in Entity entity) => _commandBuffer.Enqueue(() => _actorsRegistry.Destroy(entity));


    // Behavior
    public T AddBehavior<T>(Entity e) where T : Behavior, new()
    {
        return _actorsRegistry.AddBehavior(e, new T());
    }
    public T AddBehavior<T>(Entity e, T instance) where T : Behavior
    {
        return _actorsRegistry.AddBehavior(e, instance);
    }
    public T? GetBehavior<T>(Entity e) where T : Behavior
    {
        return _actorsRegistry.GetBehavior<T>(e);
    }
    public bool TryGetBehavior<T>(Entity e, [NotNullWhen(true)] out T? behavior) where T : Behavior
    {
        behavior = _actorsRegistry.GetBehavior<T>(e);
        return behavior != null;
    }
    public bool HasBehavior<T>(Entity e) where T : Behavior
    {
        return _actorsRegistry.HasBehavior<T>(e);
    }
    public void RemoveBehavior<T>(Entity e) where T : Behavior
    {
        _actorsRegistry.RemoveBehavior<T>(e);
    }

    public void OnInitialize()
    {
        _entityProcessors.OnInitialize();
    }
    public void OnShutdown()
    {
        _entityProcessors.OnShutdown();
    }

    public void OnUpdate()
    {
        _entityProcessors.OnUpdate();
    }
    public void OnFixedUpdate()
    {
        _entityProcessors.OnFixedUpdate();
    }
    public void OnTick()
    {
        _entityProcessors.OnTick();
    }
    public void OnRender()
    {
        _entityProcessors.OnRender();
    }


    public void AddProcessor(SEntityProcessor processor)
    {
        _entityProcessors.Add(processor);
    }
    public void RemoveProcessor(SEntityProcessor processor)
    {
        _entityProcessors.Remove(processor);
    }
    public T? GetProcessor<T>() where T : SEntityProcessor
    {
        return _entityProcessors.GetProcessor<T>();
    }


}