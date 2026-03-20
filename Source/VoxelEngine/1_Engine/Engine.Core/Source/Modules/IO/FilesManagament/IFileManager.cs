namespace VoxelEngine.IO.FilesManagement;

public interface IFileManager
{
    // bool IsRestrictedMode { get; set; }

    /// <summary>
    /// Checks if the specified URI exists in any of the mounted providers.
    /// </summary>
    /// <param name="uri">The URI to check.</param>
    /// <returns>True if the URI exists, false otherwise.</returns>
    bool Exists(string uri);

    /// <summary>
    /// Mounts a provider to a specific protocol.
    /// </summary>
    /// <param name="protocol">The protocol to mount the provider to.</param>
    /// <param name="provider">The provider to mount.</param>
    void Mount(string protocol, IFilesProvider provider);

    /// <summary>
    /// Unmounts a provider from a specific protocol.
    /// </summary>
    /// <param name="protocol">The protocol to unmount the provider from.</param>
    void Unmount(string protocol);


    Stream OpenRead(string uri);
    Stream OpenWrite(string uri);

    byte[] ReadAllBytes(string uri);
    string ReadAllText(string uri);

    // Async
    Task<byte[]> ReadAllBytesAsync(string uri, CancellationToken ct = default);
    Task<string> ReadAllTextAsync(string uri, CancellationToken ct = default);

}
