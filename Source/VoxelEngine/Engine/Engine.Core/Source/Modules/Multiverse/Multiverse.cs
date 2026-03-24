using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core;

public sealed class Multiverse
{
    private readonly List<Universe> _universes = new();

    public Universe Create()
    {
        Universe u = new(this);
        _universes.Add(u);
        u.OnInitialize();

        EngineContext.Publish(new E_UniverseCreated(u));
        return u;
    }

    public void Destroy(Universe universe)
    {
        // EngineContext.Publish(new Event_OnUniverseDestroyed(universe));
        _universes.Remove(universe);
        universe.OnShutdown();
    }

    public void OnUpdate()
    {
        foreach (var universe in _universes)
        {
            universe.OnUpdate();
        }
    }
    public void OnRender()
    {
        foreach (var universe in _universes)
        {
            universe.OnRender();
        }
    }
    public void OnFixedUpdate()
    {
        foreach (var universe in _universes)
        {
            universe.OnFixedUpdate();
        }
    }
    public void OnTick()
    {
        foreach (var universe in _universes)
        {
            universe.OnTick();
        }
    }

}