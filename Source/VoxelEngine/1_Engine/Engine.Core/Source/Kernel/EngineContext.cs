using VoxelEngine.Common;

namespace VoxelEngine.Core;

public sealed class EngineContext
{
    private static EngineContext? _instance;
    public static EngineContext Instance
    {
        get => _instance ?? throw new InvalidOperationException("EngineContext has not been initialized. Ensure the Engine is started.");
        set => _instance = value;
    }

    public readonly IEventBus EventBus;
    public readonly IServiceContainer ServiceContainer;

    public EngineContext(IEventBus eventBus, IServiceContainer serviceContainer)
    {
        EventBus = eventBus;
        ServiceContainer = serviceContainer;
    }


    // Service Container

    public static T Get<T>() where T : notnull => Instance.ServiceContainer.Get<T>();

    public static T? GetNullable<T>() where T : notnull => Instance.ServiceContainer.GetNullable<T>();

    public static void Register<T>(T service) where T : notnull => Instance.ServiceContainer.Register(service);

    public static void UnRegister<T>() where T : notnull => Instance.ServiceContainer.Unregister<T>();


    // Event Bus
    public static void Publish<T>(T message) where T : notnull => Instance.EventBus.Publish(message);

    public static void Subscribe<T>(Action<T> action) where T : notnull => Instance.EventBus.Subscribe(action);

    public static void Unsubscribe<T>(Action<T> action) where T : notnull => Instance.EventBus.Unsubscribe(action);



}