using VoxelEngine.Core;
using VoxelEngine.Core.Runtime;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Assets;

public class Features_Assets : IFeature
{
    public int Priority => 10;

    public void OnPreInit()
    {
        var assetsManager = new AssetsManager();
        EngineContext.Register<IAssetsManager>(assetsManager);
    }

    private void OnWindowLoaded(Event_WindowLoadedEvent @event)
    {
    }

}