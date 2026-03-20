using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace net11.VFS
{
    // -----------------------------------------------------------------------------
    // Core Interfaces
    // -----------------------------------------------------------------------------

    /// <summary>
    /// Represents a storage backend (Disk, Zip, Memory, Embedded).
    /// </summary>
    public interface IVFileProvider
    {
        bool IsReadOnly { get; }
        bool Exists(string path);

        // Stream Access
        Stream OpenRead(string path);
        Stream OpenWrite(string path); // Will throw if IsReadOnly is true

        // Direct Access
        byte[] ReadAllBytes(string path);
        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken ct = default);

        // Utility
        IEnumerable<string> EnumerateFiles(string directory, string searchPattern);
    }

    // -----------------------------------------------------------------------------
    // Providers
    // -----------------------------------------------------------------------------

    /// <summary>
    /// Maps a virtual path to a physical directory on the hard drive. Features strict sandboxing.
    /// </summary>
    public class DiskFileProvider : IVFileProvider
    {
        private readonly string _rootDirectory;
        private readonly bool _enforceSandbox;

        public bool IsReadOnly { get; }

        public DiskFileProvider(string rootDirectory, bool isReadOnly = true, bool enforceSandbox = true)
        {
            _rootDirectory = Path.GetFullPath(rootDirectory);
            IsReadOnly = isReadOnly;
            _enforceSandbox = enforceSandbox;

            if (!Directory.Exists(_rootDirectory))
                Directory.CreateDirectory(_rootDirectory);
        }

        private string GetSafePhysicalPath(string virtualPath)
        {
            // Normalize slashes
            virtualPath = virtualPath.Replace('/', Path.DirectorySeparatorChar);
            string physicalPath = Path.GetFullPath(Path.Combine(_rootDirectory, virtualPath));

            // Anti-Directory Traversal Security Check (e.g., preventing "../../Windows/System32")
            if (_enforceSandbox && !physicalPath.StartsWith(_rootDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"[VFS Security] Path traversal blocked: {virtualPath}");
            }

            return physicalPath;
        }

        public bool Exists(string path) => File.Exists(GetSafePhysicalPath(path));

        public Stream OpenRead(string path)
        {
            return new FileStream(GetSafePhysicalPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public Stream OpenWrite(string path)
        {
            if (IsReadOnly) throw new UnauthorizedAccessException("Provider is mounted as Read-Only.");

            string physicalPath = GetSafePhysicalPath(path);
            string directory = Path.GetDirectoryName(physicalPath);
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
    }

    /// <summary>
    /// Reads assets compiled directly into the C# Assembly. Perfect for "engine://" built-in assets!
    /// </summary>
    public class EmbeddedResourceProvider : IVFileProvider
    {
        private readonly Assembly _assembly;
        private readonly string _baseNamespace;

        public bool IsReadOnly => true;

        public EmbeddedResourceProvider(Assembly assembly, string baseNamespace)
        {
            _assembly = assembly;
            _baseNamespace = baseNamespace;
        }

        private string GetResourceName(string path)
        {
            // Converts virtual "shaders/cube.glsl" to C# embedded "net11.Assets.shaders.cube.glsl"
            return $"{_baseNamespace}.{path.Replace('/', '.')}";
        }

        public bool Exists(string path) => _assembly.GetManifestResourceInfo(GetResourceName(path)) != null;

        public Stream OpenRead(string path)
        {
            var stream = _assembly.GetManifestResourceStream(GetResourceName(path));
            if (stream == null) throw new FileNotFoundException($"Embedded resource not found: {path}");
            return stream;
        }

        public Stream OpenWrite(string path) => throw new NotSupportedException("Embedded resources are read-only.");

        public byte[] ReadAllBytes(string path)
        {
            using var stream = OpenRead(path);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken ct = default)
        {
            using var stream = OpenRead(path);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, ct);
            return ms.ToArray();
        }

        public IEnumerable<string> EnumerateFiles(string directory, string searchPattern)
        {
            string prefix = GetResourceName(directory);
            return _assembly.GetManifestResourceNames()
                            .Where(n => n.StartsWith(prefix))
                            .Select(n => n.Substring(_baseNamespace.Length + 1).Replace('.', '/'));
        }
    }

    /// <summary>
    /// Reads directly from a Zip file. Great for packaged game data (e.g., "res.pak").
    /// </summary>
    public class ZipFileProvider : IVFileProvider, IDisposable
    {
        private readonly ZipArchive _archive;

        public bool IsReadOnly => true;

        public ZipFileProvider(string zipFilePath)
        {
            _archive = ZipFile.OpenRead(zipFilePath);
        }

        public bool Exists(string path) => _archive.GetEntry(path) != null;

        public Stream OpenRead(string path)
        {
            var entry = _archive.GetEntry(path);
            if (entry == null) throw new FileNotFoundException($"File not found in zip: {path}");
            return entry.Open();
        }

        public Stream OpenWrite(string path) => throw new NotSupportedException("ZipProvider currently implemented as Read-Only.");

        public byte[] ReadAllBytes(string path)
        {
            using var stream = OpenRead(path);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken ct = default)
        {
            using var stream = OpenRead(path);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, ct);
            return ms.ToArray();
        }

        public IEnumerable<string> EnumerateFiles(string directory, string searchPattern)
        {
            return _archive.Entries
                .Where(e => e.FullName.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
                .Select(e => e.FullName);
        }

        public void Dispose() => _archive?.Dispose();
    }


    // -----------------------------------------------------------------------------
    // The Virtual File Manager
    // -----------------------------------------------------------------------------

    public class VFileManager : IVFileManager
    {
        // Thread-safe dictionary for mounts
        private readonly ConcurrentDictionary<string, IVFileProvider> _mounts = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Global sandbox flag. If true, UGC cannot mount new drives, and 
        /// restricted protocols (like "sys://") will automatically block access.
        /// </summary>
        public bool IsRestrictedMode { get; set; } = false;

        /// <summary>
        /// Mounts a provider to a specific protocol.
        /// </summary>
        public void Mount(string protocol, IVFileProvider provider)
        {
            if (IsRestrictedMode)
            {
                throw new UnauthorizedAccessException("[VFS Security] Cannot mount new providers while in Restricted Mode.");
            }
            _mounts[protocol] = provider;
        }

        public void Unmount(string protocol)
        {
            if (_mounts.TryRemove(protocol, out var provider) && provider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

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

        private IVFileProvider GetProvider(string protocol)
        {
            if (IsRestrictedMode && (protocol == "sys" || protocol == "raw"))
            {
                throw new UnauthorizedAccessException($"[VFS Security] Access to protocol '{protocol}' denied in Restricted Mode.");
            }

            if (_mounts.TryGetValue(protocol, out var provider))
            {
                return provider;
            }

            throw new DirectoryNotFoundException($"VFS Protocol '{protocol}://' is not mounted.");
        }

        // --- Synchronous API ---

        public bool Exists(string uri)
        {
            var (protocol, path) = ParseUri(uri);
            if (_mounts.TryGetValue(protocol, out var provider))
            {
                return provider.Exists(path);
            }
            return false;
        }

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

        // --- Asynchronous API ---

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
    }
}