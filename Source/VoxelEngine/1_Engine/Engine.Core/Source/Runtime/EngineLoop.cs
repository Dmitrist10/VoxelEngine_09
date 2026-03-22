using VoxelEngine.Common;
using VoxelEngine.Rendering;
using VoxelEngine.Input;

namespace VoxelEngine.Core.Runtime;

public sealed class EngineLoop : IUpdateCallbacksHandler
{
    private UniverseManager universeManager;
    private IRenderer renderer;
    private IGuiRenderer _guiRenderer;

    private IRuntimeContext runtimeContext;
    private IEngineState? engineState;
    private IInputContext _input;

    public EngineLoop(UniverseManager universeManager, IRuntimeContext runtimeContext, IRenderer renderer)
    {
        this.universeManager = universeManager;
        this.runtimeContext = runtimeContext;
        this.renderer = renderer;

        _guiRenderer = EngineContext.Get<IGuiRenderer>();
        _input = EngineContext.Get<IInputContext>();
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
        _input.Update();
        universeManager?.OnUpdate();
        _guiRenderer.Update(Time.DeltaTime);
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