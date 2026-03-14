using VoxelEngine.Core;
using VoxelEngine.Core.UGC;

namespace TestingGame;

public sealed class Game : GameBase
{

    public override void OnInitialize()
    {
        
    }

    public override void StartSession()
    {
        Universe universe = universeManager.Create();
        Scene scene = universe.Create();

        GameSetUp.SetUp(scene);
    }

}
