namespace VoxelEngine.IO.FilesManagement;


public interface IFilesProvider
{
    bool IsReadOnly { get; }
    bool Exists(string path);

    // Stream Access
    Stream OpenRead(string path);
    /// <summary>
    /// throws if IsReadOnly is true
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Stream OpenWrite(string path);

    // Direct Access
    byte[] ReadAllBytes(string path);
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken ct = default);

    // Utility
    IEnumerable<string> EnumerateFiles(string directory, string searchPattern);
}

