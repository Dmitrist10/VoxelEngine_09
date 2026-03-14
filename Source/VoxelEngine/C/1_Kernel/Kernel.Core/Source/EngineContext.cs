using VoxelEngine.Common;

namespace VoxelEngine.Kernel;

public record EngineContext
{
    public readonly IEventBus EventBus;
    public readonly IServiceContainer ServiceContainer;

    public EngineContext(IEventBus eventBus, IServiceContainer serviceContainer)
    {
        EventBus = eventBus;
        ServiceContainer = serviceContainer;
    }

}