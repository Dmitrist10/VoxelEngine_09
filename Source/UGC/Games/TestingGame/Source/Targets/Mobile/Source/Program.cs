using VoxelEngine.Core;
using VoxelEngine.Core.Runtime;
using VoxelEngine.Platforms.Desktop;

namespace Game.Targets.Mobile;

public class Program
{

    static void Main(string[] args)
    {
        try
        {
            IGame game = new Game();
            DesktopProfile profile = new DesktopProfile();

            EntryPoint.GuardedMain(args, game, profile);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e);
        }
    }

}
