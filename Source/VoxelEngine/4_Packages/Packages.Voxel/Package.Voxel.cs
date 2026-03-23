using VoxelEngine.Core;

namespace VoxelEngine.Packages.Voxel;

public sealed class VoxelPackage : IPackage
{
    public string Name => "VoxelEngine BuildIn: Voxel";
    public string Version => "0.0.1-alpha";

    public void Initialize()
    {
        EngineContext.Subscribe<Event_OnSceneCreated>(OnSceneCreated);
    }

    private void OnSceneCreated(Event_OnSceneCreated @event)
    {
        @event.scene.AddProcessor<EP_VoxelWorld>();
    }

    public void OnUpdate()
    {

    }

    public void Dispose() { }

}
