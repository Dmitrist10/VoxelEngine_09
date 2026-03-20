using VoxelEngine.Common;
using VoxelEngine.Rendering;

namespace VoxelEngine.Core.Runtime;

public sealed class EngineLoop : IUpdateCallbacksHandler
{

    // private IRenderer renderer;
    // private UniverseManager universeManager;

    public void OnInitialize()
    {
    }


    public void OnUpdate()
    {
    }
    public void OnFixedUpdate()
    {
    }


    public void OnRender()
    {
    }

    public void OnTick()
    {
    }

    public void OnSecond()
    {
        EngineContext.Publish(new Event_OnSecond());
    }

    public void Dispose()
    {
    }

}