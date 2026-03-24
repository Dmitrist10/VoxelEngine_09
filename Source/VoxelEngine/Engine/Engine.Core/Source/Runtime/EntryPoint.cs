using VoxelEngine.Common;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core.Runtime;

public sealed class EntryPoint
{

    /// <summary>
    /// The Main Engine Starting Point 
    /// </summary>
    /// <param name="args">args provided by the user at launch in program.main(string[] args)</param>
    /// <param name="game">your current games instnace that implement IGame interface</param>
    /// <param name="profile">platform specific profile that implement IEngineProfile interface</param>
    public static void GuardedMain(string[] args, IGame game, IEngineProfile profile)
    {

        try
        {
            Logger.Initialize(message: $"VoxelEngine initializing");
            ArgumentsParser.BindArgs(args);


            if (!Environment.Is64BitOperatingSystem)
                Logger.Fatal("Only 64-bit operating systems are supported.");

            try
            {
                Logger.Info("Creating engine...");

                Engine engine = new EngineBuilder()
                    .WithProfile(profile)
                    .WithGame(game)
                    .Build();

                Logger.Info("Starting engine...");
                engine.Start();
            }
            catch (Exception ex)
            {
                Logger.Line();
                Logger.Line();
                // Logger.Fatal($"Unhandled engine exception: {e.Message}\nStack trace: {e.StackTrace}");

                Logger.Error($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Logger.Error($"Inner Exception: {ex.InnerException.Message}");
                    Logger.Error($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }
                Logger.Error($"Exception Stack Trace: {ex.StackTrace}");
            }
            finally
            {
                Logger.Info("Application Completed.\nExiting with code 0");
                Logger.Shutdown();
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"[BOOTSTRAP FATAL] {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                System.Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }

            System.Console.ResetColor();
            Environment.Exit(1);
        }

        // IPlatform CreatePlatform()
        // {
        //     // bool useOpenGL = ArgumentsParser.HasArg(args, "renderer-opengl");

        //     if (OperatingSystem.IsWindows())
        //     {
        //         // if (useOpenGL)
        //         // {
        //         //     Logger.Info("arg: '--renderer-opengl' -> Using OpenGL graphics backend.");
        //         return new WindowsPlatform();
        //         // }
        //         // Logger.Info("Using Veldrid graphics backend (default). Pass '--renderer-opengl' to override.");
        //         // return new Windows_VeldridPlatform();
        //     }
        //     else if (OperatingSystem.IsLinux())
        //     {
        //         // return Linux_Platform();
        //     }
        //     else if (OperatingSystem.IsMacOS())
        //     {
        //         // return MacOS_Platform();
        //     }

        //     Logger.Fatal("Your OS isn't supported!");
        //     Environment.Exit(1);
        //     return null!;
        // }

        Console.WriteLine("whait hold on for a sec how did you even get here? huh?\nsome kind of new cheats?");
        Console.ReadLine();
    }

}