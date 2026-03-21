using VoxelEngine.Common;
using VoxelEngine.Core;
using VoxelEngine.Core.Runtime;
using VoxelEngine.Core.UGC;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Platforms.Desktop;

public sealed class EntryPoint
{

    // public void Run(string[] args, IRuntimeContext context, List<IFeatureCollection> collections)
    public void Run(string[] args, IRuntimeContext context, GameBase game)
    {
        try
        {
            Logger.Initialize(message: $"VoxelEngine initialized from Desktop platform with {context.Name} RuntimeContext.");
            ArgumentsParser.BindArgs(args);

#if AOT
                // Register Silk.NET platforms for Native AOT compatibility
                // Must be done before any Window.Create() calls
                Silk.NET.Windowing.Glfw.GlfwWindowing.RegisterPlatform();
                Silk.NET.Input.Glfw.GlfwInput.RegisterPlatform();
#endif

            if (ArgumentsParser.HasArg(args, "windowing-backend-glfw"))
            {
                Logger.Info("arg: '--windowing-backend-glfw' -> Registering GLFW window backend...");
                Silk.NET.Windowing.Glfw.GlfwWindowing.RegisterPlatform();
                Silk.NET.Input.Glfw.GlfwInput.RegisterPlatform();
            }

            if (!Environment.Is64BitOperatingSystem)
                Logger.Fatal("Only 64-bit operating systems are supported.");

            try
            {
                Logger.Info("Creating engine");

                Engine engine = new EngineBuilder()
                    .WithPlatform(CreatePlatform())
                    .WithRuntimeContext(context)
                    .WithGame(game)
                    // .AddFeatureCollection(new CoreFeaturesCollection())
                    // .AddFeatureCollection(new DesktopFeaturesCollection())
                    // .AddFeatureCollectionsList(collections)
                    .Build();

                Logger.Info("Starting engine...");
                engine.StartEngine();
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
            // Critical bootstrap failure - Logger may not be initialized
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

        IPlatform CreatePlatform()
        {
            // bool useOpenGL = ArgumentsParser.HasArg(args, "renderer-opengl");

            if (OperatingSystem.IsWindows())
            {
                // if (useOpenGL)
                // {
                //     Logger.Info("arg: '--renderer-opengl' -> Using OpenGL graphics backend.");
                return new WindowsPlatform();
                // }
                // Logger.Info("Using Veldrid graphics backend (default). Pass '--renderer-opengl' to override.");
                // return new Windows_VeldridPlatform();
            }
            else if (OperatingSystem.IsLinux())
            {
                // return Linux_Platform();
            }
            else if (OperatingSystem.IsMacOS())
            {
                // return MacOS_Platform();
            }

            Logger.Fatal("Your OS isn't supported!");
            Environment.Exit(1);
            return null!;
        }
    }

}
