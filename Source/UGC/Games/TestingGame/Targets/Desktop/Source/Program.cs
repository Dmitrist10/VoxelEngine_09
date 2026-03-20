using VoxelEngine.Core;
using VoxelEngine.Client;
using VoxelEngine.Server;
using VoxelEngine.Core.UGC;
using VoxelEngine.Core.Runtime;
using VoxelEngine.Platforms.Desktop;
using VoxelEngine.RuntimeContexts.Standalone;

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
            var collections = new List<IFeatureCollection>()
            {
                new ClientFeaturesCollection(),
                new ServerFeaturesCollection(),
            };
            
            entryPoint.Run(args, context, collections);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e);
        }

    }

}
