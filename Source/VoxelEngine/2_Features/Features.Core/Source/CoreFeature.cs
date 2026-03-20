using VoxelEngine.Core;
using VoxelEngine.Core.Runtime;

namespace VoxelEngine.Core.Features;

public sealed class CoreFeature : IFeature
{
    public int Priority => 100;

    public void OnInit()
    {
        EngineContext.Register(new UniverseManager());
    }

    public void OnPreInit()
    {
    }

}
