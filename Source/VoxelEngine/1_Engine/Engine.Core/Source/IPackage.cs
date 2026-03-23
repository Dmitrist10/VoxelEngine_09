using VoxelEngine.Common;

namespace VoxelEngine.Core;

public interface IPackage : IUpdatable, IDisposable
{
    string Name { get; }
    string Version { get; }

    void Initialize();
}

/* 

    string Version { get; }
    string Description { get; }
    string Author { get; }
    string License { get; }
    string Homepage { get; }
    string Repository { get; }
    string[] Dependencies { get; }
    string[] OptionalDependencies { get; }
    string[] Conflicts { get; }
    string[] Provides { get; }
    string[] Replaces { get; }
    string[] Keywords { get; }
    string[] Tags { get; }
    string[] Files { get; }
    string[] Scripts { get; }
    string[] Binaries { get; }
    string[] DLLs { get; }

 */