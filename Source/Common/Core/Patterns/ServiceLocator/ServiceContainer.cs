using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Common;

/// <summary>
/// Implementation of <see cref="IServiceContainer"/> using high-performance concurrent storage.
/// </summary>
public sealed class ServiceContainer : IServiceContainer
{
    private readonly ConcurrentDictionary<Type, object> _services = new();

    /// <summary>
    /// Registers a service instance.
    /// </summary>
    public void Register<T>(T service) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(service);

        var type = typeof(T);
#if DEBUG
        Logger.ExtraInfo($"Registering service: {type.Name}");
#endif
        _services[type] = service;
    }

    /// <summary>
    /// Fast retrieval of a service instance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? GetNullable<T>() where T : notnull
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }

        // Logger.Error($"[ServiceContainer] Requested service '{typeof(T).Name}' is not registered.");
        return default;
    }

    /// <summary>
    /// Fast retrieval of a service instance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get<T>() where T : notnull
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;

        throw new InvalidOperationException($"Requested service '{typeof(T).Name}' is not registered.");
    }

    /// <summary>
    /// Safe attempt to retrieve a service. No logging on failure.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGet<T>([NotNullWhen(true)] out T? service) where T : notnull
    {
        if (_services.TryGetValue(typeof(T), out var instance))
        {
            service = (T)instance;
            return true;
        }

        service = default;
        return false;
    }

    /// <summary>
    /// Unregisters a service.
    /// </summary>
    public void Unregister<T>()
    {
        _services.TryRemove(typeof(T), out _);
    }

    /// <summary>
    /// Checks for registration.
    /// </summary>
    public bool IsRegistered<T>()
    {
        return _services.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Clears all entries.
    /// </summary>
    public void Clear()
    {
#if DEBUG
        Logger.ExtraInfo("Clearing all registered services.");
#endif
        _services.Clear();
    }
}