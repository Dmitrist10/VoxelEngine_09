using VoxelEngine.RuntimeContexts.Standalone;
using VoxelEngine.Platforms.Desktop;

namespace TestingGame.Targets.Desktop;

public class Program
{

    static void Main(string[] args)
    {

        try
        {
            var layer = new GameState();
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