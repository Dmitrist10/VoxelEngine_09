using VoxelEngine.Core;

namespace VoxelEngine.Core;

public interface IRuntimeContext
{
    string Name { get; }

    IEngineState CreateFirstState();
}