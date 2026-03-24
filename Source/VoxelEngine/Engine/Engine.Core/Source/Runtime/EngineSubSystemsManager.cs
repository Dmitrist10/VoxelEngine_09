using VoxelEngine.Common;

namespace VoxelEngine.Core.Runtime;

internal sealed class EngineSubSystemsManager : IUpdateCallbacksHandler
{
    private readonly List<IEngineSubsystem> _subsystems = new();


    public void Add(IEngineSubsystem[] engineSubsystems)
    {
        _subsystems.AddRange(engineSubsystems);
    }
    public void Add(IEngineSubsystem subsystem)
    {
        _subsystems.Add(subsystem);
    }

    public void Remove(IEngineSubsystem subsystem)
    {
        _subsystems.Remove(subsystem);
    }

    public void OnInitialize()
    {
        foreach (var subsystem in _subsystems)
        {
            subsystem.OnInitialize();
        }
    }

    public void OnUpdate()
    {
        foreach (var subsystem in _subsystems)
        {
            subsystem.OnUpdate();
        }
    }

    public void OnFixedUpdate()
    {
        foreach (var subsystem in _subsystems)
        {
            subsystem.OnFixedUpdate();
        }
    }
    public void OnRender()
    {
        foreach (var subsystem in _subsystems)
        {
            subsystem.OnRender();
        }
    }
    public void OnTick()
    {
        foreach (var subsystem in _subsystems)
        {
            subsystem.OnTick();
        }
    }

    public void Dispose()
    {
        foreach (var subsystem in _subsystems)
        {
            subsystem.Dispose();
        }
    }
}