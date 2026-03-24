using Arch.Core;
using VoxelEngine.Common;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core;

public abstract class EntityProcessorBase
{
    public Multiverse multiverse { get; internal set; } = null!;

    public virtual void OnInitialize() { }
    public virtual void OnShutdown() { }
}

public class SEntityProcessor : EntityProcessorBase
{
    public Scene scene { get; internal set; } = null!;
    public Universe universe => scene.Universe;

    public World world => scene.World;

}
public abstract class SEntityProcessorGroup : SEntityProcessor, IUpdatable, IFixedUpdatable, ITickable, IRenderable
{

    private readonly List<SEntityProcessor> _processors = new();
    public IReadOnlyList<SEntityProcessor> Processors => _processors;

    private List<IUpdatable> _updatables = new();
    private List<IFixedUpdatable> _fixedUpdatables = new();
    private List<ITickable> _tickables = new();
    private List<IRenderable> _renderables = new();

    public bool IsActive { get; set; } = true;

    public void Add(SEntityProcessor processor)
    {
        processor.scene = scene;
        processor.multiverse = multiverse;
        _processors.Add(processor);

        if (processor is IUpdatable updatable) _updatables.Add(updatable);
        if (processor is IFixedUpdatable fixedUpdatable) _fixedUpdatables.Add(fixedUpdatable);
        if (processor is ITickable tickable) _tickables.Add(tickable);
        if (processor is IRenderable renderable) _renderables.Add(renderable);
    }
    public void Add<T>() where T : SEntityProcessor, new()
    {
        T processor = new();
        Add(processor);
    }
    public void Remove(SEntityProcessor processor)
    {
        _processors.Remove(processor);

        if (processor is IUpdatable updatable) _updatables.Remove(updatable);
        if (processor is IFixedUpdatable fixedUpdatable) _fixedUpdatables.Remove(fixedUpdatable);
        if (processor is ITickable tickable) _tickables.Remove(tickable);
        if (processor is IRenderable renderable) _renderables.Remove(renderable);
    }
    public void Remove<T>() where T : SEntityProcessor
    {
        T? processor = GetProcessor<T>();
        if (processor != null)
        {
            Remove(processor);
        }
    }

    public T? GetProcessor<T>() where T : SEntityProcessor
    {
        foreach (var processor in _processors)
        {
            if (processor is T t)
            {
                return t;
            }
        }
        return null;
    }

    // public void Clear()
    // {
    //     _processors.Clear();
    //     _updatables.Clear();
    //     _fixedUpdatables.Clear();
    //     _tickables.Clear();
    //     _renderables.Clear();
    // }

    public override void OnInitialize() => _processors.ForEach(p => p.OnInitialize());
    public override void OnShutdown() => _processors.ForEach(p => p.OnShutdown());

    public void OnUpdate()
    {
        if (!IsActive) return;

        foreach (var processor in _updatables)
        {
            processor.OnUpdate();
        }
    }
    public void OnFixedUpdate()
    {
        if (!IsActive) return;

        foreach (var processor in _fixedUpdatables)
        {
            processor.OnFixedUpdate();
        }
    }
    public void OnTick()
    {
        if (!IsActive) return;


        foreach (var processor in _tickables)
        {
            processor.OnTick();
        }
    }
    public void OnRender()
    {
        if (!IsActive) return;

        foreach (var processor in _renderables)
        {
            processor.OnRender();
        }
    }

}
public class SEPGroup : SEntityProcessorGroup
{

}
public sealed class SEntityProcessorGlobalGroup : SEntityProcessorGroup
{

}

public abstract class UEntityProcessor : EntityProcessorBase
{
    public Universe universe { get; internal set; } = null!;
    public IReadOnlyList<Scene> scenes => universe.Scenes;

}