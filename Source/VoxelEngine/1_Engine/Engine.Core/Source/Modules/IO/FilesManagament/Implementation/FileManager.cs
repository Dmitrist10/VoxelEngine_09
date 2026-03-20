using System.Collections.Concurrent;

namespace VoxelEngine.IO.FilesManagement;

public sealed class FileManager : IFileManager
{

    /// <summary>
    /// protocol -> provider
    /// </summary>
    private readonly ConcurrentDictionary<string, IFilesProvider> _providers = new();

    public void Mount(string protocol, IFilesProvider provider)
    {
        // if (IsRestrictedMode)
        //     throw new UnauthorizedAccessException("[VFS Security] Cannot mount new providers while in Restricted Mode.");

        _providers[protocol + "://"] = provider;
    }
    public void Unmount(string protocol)
    {
        if (_providers.TryRemove(protocol + "://", out var provider) && provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public bool Exists(string uri)
    {
        var (protocol, path) = ParseUri(uri); // Get Protocol

        if (_providers.TryGetValue(protocol, out var provider)) // Get Provider for the protocol
        {
            return provider.Exists(path); // Check if file exists
        }

        // throw new DirectoryNotFoundException($"Protocol '{protocol}://' is not mounted.");
        return false;
    }


    // public Stream OpenRead(string uri) => GetProviderByUri(uri).OpenRead(uri);
    // public Stream OpenWrite(string uri) => GetProviderByUri(uri).OpenWrite(uri);

    // public byte[] ReadAllBytes(string uri) => GetProviderByUri(uri).ReadAllBytes(uri);
    // public string ReadAllText(string uri)
    // {
    //     using var stream = OpenRead(uri);
    //     using var reader = new StreamReader(stream);
    //     return reader.ReadToEnd();
    // }


    // public async Task<byte[]> ReadAllBytesAsync(string uri, CancellationToken ct = default)
    // {
    //     return await GetProviderByUri(uri).ReadAllBytesAsync(uri, ct);
    // }

    // public async Task<string> ReadAllTextAsync(string uri, CancellationToken ct = default)
    // {
    //     using var stream = OpenRead(uri);
    //     using var reader = new StreamReader(stream);
    //     return await reader.ReadToEndAsync();
    // }


    public Stream OpenRead(string uri)
    {
        var (protocol, path) = ParseUri(uri);
        return GetProvider(protocol).OpenRead(path);
    }

    public Stream OpenWrite(string uri)
    {
        var (protocol, path) = ParseUri(uri);
        return GetProvider(protocol).OpenWrite(path);
    }



    public byte[] ReadAllBytes(string uri)
    {
        var (protocol, path) = ParseUri(uri);
        return GetProvider(protocol).ReadAllBytes(path);
    }

    public string ReadAllText(string uri)
    {
        using var stream = OpenRead(uri);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }




    public async Task<byte[]> ReadAllBytesAsync(string uri, CancellationToken ct = default)
    {
        var (protocol, path) = ParseUri(uri);
        return await GetProvider(protocol).ReadAllBytesAsync(path, ct);
    }

    public async Task<string> ReadAllTextAsync(string uri, CancellationToken ct = default)
    {
        using var stream = OpenRead(uri);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    //  -----------------Internals/Privates------------------------

    /// <summary>
    /// Parses "protocol://path/to/file" into (protocol, path).
    /// </summary>
    private (string protocol, string path) ParseUri(string uri)
    {
        int index = uri.IndexOf("://", StringComparison.Ordinal);
        if (index == -1) throw new ArgumentException($"Invalid VFS URI format: {uri}");

        string protocol = uri.Substring(0, index);
        string path = uri.Substring(index + 3);

        // Level 1 URI Security
        if (path.Contains(".."))
        {
            throw new UnauthorizedAccessException($"[VFS Security] Path traversal attempted in URI: {uri}");
        }

        return (protocol, path);
    }

    /// <summary>
    /// Returns the provider assigned to the given protocol.
    /// throws DirectoryNotFoundException if no provider is found.
    /// </summary>
    /// <param name="protocol"></param>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
    private IFilesProvider GetProvider(string protocol)
    {
        // if (IsRestrictedMode && (protocol == "sys" || protocol == "raw"))
        //     throw new UnauthorizedAccessException($"[VFS Security] Access to protocol '{protocol}' denied in Restricted Mode.");

        if (_providers.TryGetValue(protocol, out var provider))
            return provider;

        throw new DirectoryNotFoundException($"Protocol '{protocol}://' is not mounted.");
    }


    private IFilesProvider GetProviderByUri(string uri)
    {
        var (protocol, path) = ParseUri(uri);
        if (_providers.TryGetValue(protocol, out var provider))
            return provider;

        throw new DirectoryNotFoundException($"Protocol '{protocol}://' is not mounted.");
    }


}
