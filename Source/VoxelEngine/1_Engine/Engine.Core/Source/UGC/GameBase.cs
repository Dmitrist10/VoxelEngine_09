namespace VoxelEngine.Core.UGC;

public abstract class GameBase
{
    public UniverseManager universeManager { get; internal set; } = null!;

    public abstract void OnInitialize();
    public abstract void StartSession();
}