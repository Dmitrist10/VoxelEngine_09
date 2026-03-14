using VoxelEngine.Kernel.UGC;

namespace VoxelEngine.Core.UGC;

public abstract class GameBase : IGame
{
    public UniverseManager universeManager { get; private set; } = null!;

    public void SetUniverseManager(UniverseManager universeManager)
    {
        this.universeManager = universeManager;
    }

    // protected GameBase(UniverseManager universeManager)
    // {
    //     this.universeManager = universeManager;
    // }


    public abstract void OnInitialize();
    public abstract void StartSession();
}