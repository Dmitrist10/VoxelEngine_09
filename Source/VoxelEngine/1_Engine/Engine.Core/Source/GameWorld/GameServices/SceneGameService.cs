namespace VoxelEngine.Core;

public abstract class SceneGameService
{
    public required Scene scene { get; init; }

    public virtual void OnInitialized() { }
    public virtual void OnShutdown() { }

}