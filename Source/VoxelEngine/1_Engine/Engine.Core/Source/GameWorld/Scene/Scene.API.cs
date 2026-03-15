using System.Diagnostics.CodeAnalysis;
using Arch.Core;
using VoxelEngine.Common;

namespace VoxelEngine.Core;

public sealed partial class Scene
{

    // Actors
    public Actor CreateActor() => _actorsRegistry.CreateActor();
    public Entity CreateEntity() => _actorsRegistry.CreateEntity();

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


    // Game Services
    public T AddService<T>() where T : SceneGameService, new() => _servicesRegistry.AddService(new T() { scene = this });
    public T AddService<T>(T service) where T : SceneGameService => _servicesRegistry.AddService(service);
    public T? GetService<T>() where T : SceneGameService => _servicesRegistry.GetService<T>();
    public bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : SceneGameService
    {
        service = _servicesRegistry.GetService<T>();
        return service != null;
    }
    public bool HasService<T>() where T : SceneGameService => _servicesRegistry.HasService<T>();
    public void RemoveService<T>() where T : SceneGameService => _servicesRegistry.RemoveService<T>();

    // Entity Processors
    public T AddProcessor<T>() where T : EntityProcessor, new() => _entityProcessorsRegistry.AddProcessor<T>();
    public T AddProcessor<T>(T processor) where T : EntityProcessor => _entityProcessorsRegistry.AddProcessor(processor);
    public T? GetProcessor<T>() where T : EntityProcessor => _entityProcessorsRegistry.GetProcessor<T>();
    public bool TryGetProcessor<T>([NotNullWhen(true)] out T? processor) where T : EntityProcessor
    {
        processor = _entityProcessorsRegistry.GetProcessor<T>();
        return processor != null;
    }
    public bool HasProcessor<T>() where T : EntityProcessor => _entityProcessorsRegistry.HasProcessor<T>();
    public void RemoveProcessor<T>() where T : EntityProcessor => _entityProcessorsRegistry.RemoveProcessor<T>();

}
