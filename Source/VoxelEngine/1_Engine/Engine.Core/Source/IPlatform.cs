using VoxelEngine.Graphics;
using VoxelEngine.Windowing;

namespace VoxelEngine.Core;

public interface IPlatform : IDisposable
{
    IWindowSurface CreateWindowSurface();

    // Drivers
    IGraphicsDriver CreateGraphicsDriver();
    // IAudioDriver CreateAudioDriver();

}