using VoxelEngine.Windowing;

namespace VoxelEngine.Core.Runtime;

public record struct Event_WindowLoadedEvent(IWindowSurface WindowSurface, IPlatform Platform);
