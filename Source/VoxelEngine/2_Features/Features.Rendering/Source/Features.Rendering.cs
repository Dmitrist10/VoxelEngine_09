using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Rendering;

public class RenderingFeature : IFeature
{

    private RenderManager _renderManager = null!;

    public int Priority => 1;

    public void OnPreInit()
    {
        EngineContext.Subscribe<Event_OnSceneCreated>(OnSceneCreated);
        EngineContext.Subscribe<Event_OnUniverseCreated>(OnUniverseCreated);
        EngineContext.Subscribe<Event_OnRender>(OnRender);
    }

    public void OnInit()
    {
        _renderManager = new();
        _renderManager.Initialize();
        EngineContext.Register(_renderManager);
    }

    private void OnRender(Event_OnRender render)
    {
        _renderManager.Render();
    }

    private void OnSceneCreated(Event_OnSceneCreated e)
    {
        e.scene.AddProcessor<EP_Camera>();
        e.scene.AddProcessor<EP_MeshRenderer>();
    }
    private void OnUniverseCreated(Event_OnUniverseCreated e)
    {
    }


}