using System.Diagnostics.CodeAnalysis;

namespace VoxelEngine.Common;

public interface IServiceContainer
{
    /// <summary>
    /// Registers a service instance by its type or interface.
    /// </summary>
    /// <typeparam name="T">The type to register the service as.</typeparam>
    /// <param name="service">The instance of the service.</param>
    void Register<T>(T service) where T : notnull;

    /// <summary>
    /// Retrieves a service instance.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>The service instance, or null if not registered.</returns>
    T? GetNullable<T>() where T : notnull;

    /// <summary>
    /// Retrieves a service instance. Throws an exception if not found.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>The service instance.</returns>
    T Get<T>() where T : notnull;

    /// <summary>
    /// Attempts to retrieve a service instance without logging errors.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <param name="service">The retrieved service instance, if found.</param>
    /// <returns>True if the service was found, false otherwise.</returns>
    bool TryGet<T>([NotNullWhen(true)] out T? service) where T : notnull;

    /// <summary>
    /// Unregisters a service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to unregister.</typeparam>
    void Unregister<T>();

    /// <summary>
    /// Checks if a service of the specified type is registered.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns>True if registered, false otherwise.</returns>
    bool IsRegistered<T>();

    /// <summary>
    /// Clears all registered services from the container.
    /// </summary>
    void Clear();
}
