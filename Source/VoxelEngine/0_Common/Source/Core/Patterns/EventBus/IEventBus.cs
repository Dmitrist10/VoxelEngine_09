namespace VoxelEngine.Common;

public interface IEventBus
{
    /// <summary>
    /// Subscribes a handler to a specific event type.
    /// </summary>
    /// <typeparam name="T">The type of event to listen for.</typeparam>
    /// <param name="handler">The callback to execute when the event is published.</param>
    void Subscribe<T>(Action<T> handler);

    /// <summary>
    /// Unsubscribes a handler from a specific event type.
    /// </summary>
    /// <typeparam name="T">The type of event to stop listening for.</typeparam>
    /// <param name="handler">The callback to remove.</param>
    void Unsubscribe<T>(Action<T> handler);

    /// <summary>
    /// Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="T">The type of event being published.</typeparam>
    /// <param name="eventData">The event data instance.</param>
    void Publish<T>(T eventData);

    /// <summary>
    /// Removes all subscribers from all event types.
    /// </summary>
    void Clear();
}