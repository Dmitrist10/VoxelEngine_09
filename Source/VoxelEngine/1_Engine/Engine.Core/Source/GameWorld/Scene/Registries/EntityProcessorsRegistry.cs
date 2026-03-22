using VoxelEngine.Common;

namespace VoxelEngine.Core;

internal sealed class EntityProcessorsRegistry
{
    private readonly Dictionary<Type, EntityProcessor> _processors = new();
    private readonly List<EntityProcessor> _allProcessors = new();

    private readonly List<IUpdatable> _allUpdatables = new();
    private readonly List<IFixedUpdatable> _allFixedUpdatables = new();
    private readonly List<IRenderable> _allRenderables = new();
    private readonly List<ITickable> _allTickables = new();

    private readonly Scene _scene;

    public EntityProcessorsRegistry(Scene scene)
    {
        _scene = scene;
    }

    [MethodImpl(AggressiveInlining)]
    public T AddProcessor<T>() where T : EntityProcessor, new()
    {
        return AddProcessor(new T() { scene = _scene });
    }

    public T AddProcessor<T>(T processor) where T : EntityProcessor
    {
        _processors.Add(typeof(T), processor);
        _allProcessors.Add(processor);

        if (processor is IUpdatable updatable)
            _allUpdatables.Add(updatable);

        if (processor is IFixedUpdatable fixedUpdatable)
            _allFixedUpdatables.Add(fixedUpdatable);

        if (processor is IRenderable renderable)
            _allRenderables.Add(renderable);

        if (processor is ITickable tickable)
            _allTickables.Add(tickable);

        processor.OnInitialize();
        return processor;
    }
    [MethodImpl(AggressiveInlining)]
    public T? GetProcessor<T>() where T : EntityProcessor
    {
        return _processors.TryGetValue(typeof(T), out var processor) ? processor as T : null;
    }
    [MethodImpl(AggressiveInlining)]
    public bool HasProcessor<T>() where T : EntityProcessor
    {
        return _processors.ContainsKey(typeof(T));
    }
    [MethodImpl(AggressiveInlining)]
    public void RemoveProcessor<T>() where T : EntityProcessor
    {
        if (_processors.TryGetValue(typeof(T), out var processor))
        {
            processor.OnShutDown();
            _processors.Remove(typeof(T));
            _allProcessors.Remove(processor);

            if (processor is IUpdatable updatable)
                _allUpdatables.Remove(updatable);

            if (processor is IFixedUpdatable fixedUpdatable)
                _allFixedUpdatables.Remove(fixedUpdatable);

            if (processor is IRenderable renderable)
                _allRenderables.Remove(renderable);

            if (processor is ITickable tickable)
                _allTickables.Remove(tickable);
        }
    }

    internal void OnUpdate()
    {
        var mask = EngineContext.ActiveGroups;
        foreach (var i in _allProcessors)
        {
            if ((mask & i.Group) != 0 && i is IUpdatable u)
                u.OnUpdate();
        }
    }
    internal void OnFixedUpdate()
    {
        var mask = EngineContext.ActiveGroups;
        foreach (var i in _allProcessors)
        {
            if ((mask & i.Group) != 0 && i is IFixedUpdatable fu)
                fu.OnFixedUpdate();
        }
    }

    internal void OnRender()
    {
        var mask = EngineContext.ActiveGroups;
        foreach (var i in _allProcessors)
        {
            if ((mask & i.Group) != 0 && i is IRenderable r)
                r.OnRender();
        }
    }

    internal void OnTick()
    {
        var mask = EngineContext.ActiveGroups;
        foreach (var i in _allProcessors)
        {
            if ((mask & i.Group) != 0 && i is ITickable t)
                t.OnTick();
        }
    }
}