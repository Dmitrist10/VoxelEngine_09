using VoxelEngine.Common;

namespace VoxelEngine.Core;

public class SystemGroup : EntityProcessor, IUpdatable, IFixedUpdatable, IRenderable
{
    public string Name { get; }

    // Pre-casted lists for maximum performance during the loop. No casting needed!
    private readonly List<EntityProcessor> _processors = new();
    private readonly List<IUpdatable> _updatables = new();
    private readonly List<IFixedUpdatable> _fixedUpdatables = new();
    private readonly List<IRenderable> _renderables = new();

    public SystemGroup(string name)
    {
        Name = name;
    }

    public void Add(EntityProcessor processor)
    {
        processor.scene = this.scene;
        processor.OnInitialize();
        _processors.Add(processor);

        // Pre-cache interfaces to avoid type-checking during tight game loops
        if (processor is IUpdatable u) _updatables.Add(u);
        if (processor is IFixedUpdatable f) _fixedUpdatables.Add(f);
        if (processor is IRenderable r) _renderables.Add(r);
    }

    // Notice we do NOT use LINQ or IEnumerable here. A pure `foreach` on a `List<T>` 
    // compiles down to a very fast pointer iteration in C#.
    public void OnUpdate()
    {
        if (!Enabled) return;
        foreach (var p in _updatables) p.OnUpdate();
    }

    public void OnFixedUpdate()
    {
        if (!Enabled) return;
        foreach (var p in _fixedUpdatables) p.OnFixedUpdate();
    }

    public void OnRender()
    {
        if (!Enabled) return;
        foreach (var p in _renderables) p.OnRender();
    }
}


/* 

// Inside your Scene.cs or Engine initialization:

var rootPipeline = new SystemGroup("Root Pipeline");

var physicsGroup = new SystemGroup("1 - Physics");
var logicGroup = new SystemGroup("2 - Game Logic");
var transformGroup = new SystemGroup("3 - Transforms");

// The order of insertion GUARANTEES execution order
rootPipeline.Add(physicsGroup);
rootPipeline.Add(logicGroup);
rootPipeline.Add(transformGroup);

// Now populate the groups
physicsGroup.Add(new EP_Gravity());
physicsGroup.Add(new EP_CollisionSolve());

transformGroup.Add(new EP_UpdateWorldMatrices());

// Add the ONE root pipeline to the Scene
scene.AddProcessor(rootPipeline);


 */