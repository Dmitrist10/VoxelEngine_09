using Arch.Core;

namespace VoxelEngine.Core;

public class ActorsWorld
{
    public World World { get; internal set; }

    public ActorsWorld()
    {
        World = World.Create();
    }


}