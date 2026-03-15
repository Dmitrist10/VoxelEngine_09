using VoxelEngine.Common;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core.Runtime;

internal class EngineLoop : IUpdateCallbacksHandler
{
    private IRuntimeContext runtimeContext;
    private IEngineState? state;

    private UniverseManager? universeManager;

    public EngineLoop(IRuntimeContext runtimeContext)
    {
        this.runtimeContext = runtimeContext;

    }

    public void OnInitialize()
    {
        universeManager = EngineContext.Get<UniverseManager>();

        state = runtimeContext.CreateFirstState();
        state.OnInitialize();
    }

    public void OnFixedUpdate()
    {
    }


    public void OnRender()
    {
        // universeManager!.OnRender();
    }

    public void OnTick()
    {
    }

    public void OnUpdate()
    {
    }

    public void Dispose()
    {
    }


}