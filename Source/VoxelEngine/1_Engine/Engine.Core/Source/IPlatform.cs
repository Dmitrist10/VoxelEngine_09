using VoxelEngine.Graphics;
using VoxelEngine.Input;
using VoxelEngine.Windowing;

namespace VoxelEngine.Core;

public interface IPlatform : IDisposable
{
    IWindowSurface CreateWindowSurface();

    // Drivers
    IGraphicsDriver CreateGraphicsDriver();
    IInputDriver CreateInputDriver();
    // IAudioDriver CreateAudioDriver();

}