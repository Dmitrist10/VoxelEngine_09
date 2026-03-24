using VoxelEngine.Common;
using VoxelEngine.Rendering;

namespace VoxelEngine.Core.Runtime;

public sealed class EngineLoop : IUpdateCallbacksHandler
{

    private readonly EngineSubSystemsManager _engineSubSystemsManager;
    private readonly Renderer _renderer;
    private readonly Multiverse _multiverse;

    internal EngineLoop(EngineSubSystemsManager engineSubSystemsManager, Renderer renderer, Multiverse multiverse)
    {
        _engineSubSystemsManager = engineSubSystemsManager;
        _renderer = renderer;
        _multiverse = multiverse;
    }

    // private IRenderer renderer;
    // private UniverseManager universeManager;

    public void OnInitialize()
    {
    }

    public void OnUpdate()
    {
        _engineSubSystemsManager.OnUpdate();
        _multiverse.OnUpdate();
    }
    public void OnFixedUpdate()
    {
        _engineSubSystemsManager.OnFixedUpdate();
        _multiverse.OnFixedUpdate();
    }
    public void OnRender()
    {
        _engineSubSystemsManager.OnRender();
        _multiverse.OnRender();
        _renderer.RenderFrame();
    }
    public void OnTick()
    {
        _engineSubSystemsManager.OnTick();
        _multiverse.OnTick();
    }

    public void OnSecond()
    {
        // _engineSubSystemsManager.OnSecond();
    }

    public void Dispose()
    {
        _engineSubSystemsManager.Dispose();
    }

}