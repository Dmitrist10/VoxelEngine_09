using System;
using VoxelEngine.Diagnostics;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace VoxelEngine.Common;

/// <summary>
/// Professional Standart implementation of <see cref="IEventBus"/>.
/// Uses a lock-free read mechanism for maximum performance during event dispatch.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, Delegate> _handlers = new();

    /// <summary>
    /// Subscribes to an event of type <typeparamref name="T"/>.
    /// </summary>
    public void Subscribe<T>(Action<T> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handlers.AddOrUpdate(
            typeof(T),
            handler,
            (_, existing) => Delegate.Combine(existing, handler)!
        );
    }

    /// <summary>
    /// Unsubscribes from an event of type <typeparamref name="T"/>.
    /// </summary>
    public void Unsubscribe<T>(Action<T> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var type = typeof(T);
        while (_handlers.TryGetValue(type, out var existing))
        {
            var updated = Delegate.Remove(existing, handler);

            if (updated == existing)
                return; // Nothing to remove or already removed

            if (updated == null)
            {
                // Attempt to remove the key only if the value is still 'existing'
                if (((ICollection<KeyValuePair<Type, Delegate>>)_handlers).Remove(new KeyValuePair<Type, Delegate>(type, existing)))
                    return;
            }
            else
            {
                // Attempt to update only if the value is still 'existing'
                if (_handlers.TryUpdate(type, updated, existing))
                    return;
            }
        }
    }

    /// <summary>
    /// Publishes an event to all subscribers. Highly optimized for frequent calls.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Publish<T>(T eventData)
    {
        if (_handlers.TryGetValue(typeof(T), out var del))
        {
            // Multicast delegates are thread-safe for invocation once the reference is obtained.
            ((Action<T>)del).Invoke(eventData);
        }
    }

    /// <summary>
    /// Clears all subscriptions from the bus.
    /// </summary>
    public void Clear()
    {
#if DEBUG
        Logger.ExtraInfo("EventBus.Clear()");
#endif
        _handlers.Clear();
    }
}
