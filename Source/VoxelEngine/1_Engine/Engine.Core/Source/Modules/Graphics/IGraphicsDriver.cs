using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public interface IGraphicsDriver : IEngineDriver
{
    IGraphicsDevice CreateGraphicsDevice();
}