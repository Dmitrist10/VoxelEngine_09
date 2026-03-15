namespace VoxelEngine.Core;

public abstract class UniverseGameService
{
    public required Universe universe { get; init; }

    public virtual void OnInitialized() { }
    public virtual void OnShutdown() { }
}