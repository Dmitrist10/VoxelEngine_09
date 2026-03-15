using VoxelEngine.RuntimeContexts.Standalone;
using VoxelEngine.Platforms.Desktop;
using VoxelEngine.Core.UGC;

namespace TestingGame.Targets.Desktop;

public class Program
{

    static void Main(string[] args)
    {

        try
        {
            IGame game = new Game();
            var context = new StandaloneRuntimeContext(game);
            var entryPoint = new EntryPoint();
            
            entryPoint.Run(args, context);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e);
        }

    }

}
