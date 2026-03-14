using VoxelEngine.RuntimeContexts.Standalone;
using VoxelEngine.Platforms.Desktop;
using VoxelEngine.Kernel.UGC;

namespace TestingGame.Targets.Desktop;

public class Program
{

    static void Main(string[] args)
    {

        try
        {
            var game = new Game();
            var layer = new GameState(game);
            var entryPoint = new EntryPoint();
            
            entryPoint.Run(args, layer);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e);
        }

    }

}