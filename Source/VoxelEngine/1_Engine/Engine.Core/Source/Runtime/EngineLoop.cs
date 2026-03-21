using VoxelEngine.Common;
using VoxelEngine.Rendering;

namespace VoxelEngine.Core.Runtime;

public sealed class EngineLoop : IUpdateCallbacksHandler
{
    private UniverseManager universeManager;
    private IRenderer renderer;

    private IRuntimeContext runtimeContext;
    private IEngineState? engineState;

    public EngineLoop(UniverseManager universeManager, IRuntimeContext runtimeContext, IRenderer renderer)
    {
        this.universeManager = universeManager;
        this.runtimeContext = runtimeContext;
        this.renderer = renderer;
    }

    // private IRenderer renderer;
    // private UniverseManager universeManager;

    public void OnInitialize()
    {
        engineState = runtimeContext.CreateFirstState();
        engineState.OnInitialize();
    }

    public void OnUpdate()
    {
        universeManager?.OnUpdate();
    }
    public void OnFixedUpdate()
    {
        universeManager?.OnFixedUpdate();
    }
    public void OnRender()
    {
        universeManager?.OnRender();
        renderer.RenderFrame();
    }
    public void OnTick()
    {
        universeManager?.OnTick();
    }

    public void OnSecond()
    {
        EngineContext.Publish(new Event_OnSecond());
    }

    public void Dispose()
    {
    }

}