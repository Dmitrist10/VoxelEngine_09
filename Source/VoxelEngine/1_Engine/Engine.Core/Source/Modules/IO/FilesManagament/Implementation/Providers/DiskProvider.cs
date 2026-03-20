namespace VoxelEngine.IO.FilesManagement;

public sealed class DiskFilesProvider : IFilesProvider
{
    private readonly string _rootDirectory;
    private readonly bool _enforceSandbox;

    public bool IsReadOnly { get; }

    public DiskFilesProvider(string rootDirectory, bool isReadOnly = true, bool enforceSandbox = true)
    {
        _rootDirectory = Path.GetFullPath(rootDirectory);
        IsReadOnly = isReadOnly;
        _enforceSandbox = enforceSandbox;

        if (!Directory.Exists(_rootDirectory))
            Directory.CreateDirectory(_rootDirectory);
    }

    public bool Exists(string path) => File.Exists(GetSafePhysicalPath(path));

    public Stream OpenRead(string path)
    {
        return new FileStream(GetSafePhysicalPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public Stream OpenWrite(string path)
    {
        if (IsReadOnly)
            throw new UnauthorizedAccessException("Provider is mounted as Read-Only.");

        string physicalPath = GetSafePhysicalPath(path);
        string? directory = Path.GetDirectoryName(physicalPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return new FileStream(physicalPath, FileMode.Create, FileAccess.Write, FileShare.None);
    }

    public byte[] ReadAllBytes(string path) => File.ReadAllBytes(GetSafePhysicalPath(path));

    public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken ct = default)
    {
        using var stream = new FileStream(GetSafePhysicalPath(path), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, ct);
        return ms.ToArray();
    }


    public IEnumerable<string> EnumerateFiles(string directory, string searchPattern)
    {
        string physDir = GetSafePhysicalPath(directory);
        if (!Directory.Exists(physDir)) return Enumerable.Empty<string>();

        // Yield relative virtual paths back to the engine
        return Directory.EnumerateFiles(physDir, searchPattern, SearchOption.AllDirectories)
                        .Select(p => p.Substring(_rootDirectory.Length).TrimStart(Path.DirectorySeparatorChar).Replace('\\', '/'));
    }


    private string GetSafePhysicalPath(string virtualPath)
    {
        // Normalize slashes
        virtualPath = virtualPath.Replace('/', Path.DirectorySeparatorChar);
        string physicalPath = Path.GetFullPath(Path.Combine(_rootDirectory, virtualPath));

        // Anti-Directory Traversal Security Check (e.g., preventing "../../Windows/System32")
        if (_enforceSandbox && !physicalPath.StartsWith(_rootDirectory, StringComparison.OrdinalIgnoreCase) && physicalPath.Contains("..", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException($"[FileManager Security] Path traversal blocked: {virtualPath}");
        }

        return physicalPath;
    }

}