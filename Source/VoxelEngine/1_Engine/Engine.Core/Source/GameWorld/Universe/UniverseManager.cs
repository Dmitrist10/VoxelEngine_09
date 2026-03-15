using VoxelEngine.Core.UGC;

namespace VoxelEngine.Core;

public sealed class UniverseManager
{
    public Universe Create()
    {
        Universe u = new(this);
        return u;
    }

}