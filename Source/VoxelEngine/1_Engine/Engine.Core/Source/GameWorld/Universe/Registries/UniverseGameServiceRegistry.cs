namespace VoxelEngine.Core;

internal sealed class UniverseGameServicesRegistry
{
    private readonly Dictionary<Type, UniverseGameService> _services = new();

    private readonly Universe _universe;

    public UniverseGameServicesRegistry(Universe universe)
    {
        _universe = universe;
    }

    public T AddService<T>(T service) where T : UniverseGameService
    {
        _services.Add(typeof(T), service);
        service.OnInitialized();
        return service;
    }
    [MethodImpl(AggressiveInlining)]
    public T? GetService<T>() where T : UniverseGameService
    {
        return _services.TryGetValue(typeof(T), out var service) ? service as T : null;
    }
    [MethodImpl(AggressiveInlining)]
    public bool HasService<T>() where T : UniverseGameService
    {
        return _services.ContainsKey(typeof(T));
    }
    [MethodImpl(AggressiveInlining)]
    public void RemoveService<T>() where T : UniverseGameService
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            service.OnShutdown();
            _services.Remove(typeof(T));
        }
    }

    // internal void OnUpdate()
    // {
    //     foreach (var service in _services.Values)
    //         service.OnUpdate();
    // }
}