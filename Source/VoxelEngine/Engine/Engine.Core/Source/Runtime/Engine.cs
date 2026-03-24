using Arch.Core;
using IniParser;
using IniParser.Model;

using VoxelEngine.Assets;
using VoxelEngine.Graphics;
using VoxelEngine.Rendering;
using VoxelEngine.Windowing;
using VoxelEngine.Diagnostics;
using VoxelEngine.IO.FilesManagement;
using VoxelEngine.Common;

namespace VoxelEngine.Core.Runtime;

public sealed class Engine
{
    private readonly IEngineProfile profile;
    private readonly IGame game;

    private readonly EngineSubSystemsManager _engineSubSystemsManager;

    private IWindowSurface? _window;
    private Multiverse? _multiverse;
    private Renderer? _renderer;

    private EngineSettings engineSettings;

    internal Engine(IEngineProfile profile, IGame game)
    {
        this.profile = profile;
        this.game = game;

        _engineSubSystemsManager = new();
        engineSettings = new();
    }

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

        Console.WriteLine("whait hold on for a sec how did you even get here? huh?\nsome kind of new cheats?");
        Console.ReadLine();
    }

    internal void Start()
    {
        PreInit();
        Init();

        Run();

        ShutDown();
    }

    private void PreInit()
    {
        Logger.Info("PreInit");

        IEventBus eventBus = new EventBus();
        IServiceContainer serviceContainer = new ServiceContainer();

        EngineContext engineContext = new(eventBus, serviceContainer);
        EngineContext.Instance = engineContext;

        // Stage 0 Load Settings from file
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "Settings.ini");

        if (File.Exists(configPath))
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(configPath);

            engineSettings.Window_Title = data["Window"]["Title"] ?? engineSettings.Window_Title;

            if (int.TryParse(data["Window"]["Width"], out int w)) engineSettings.Window_Width = w;
            if (int.TryParse(data["Window"]["Height"], out int h)) engineSettings.Window_Height = h;
            if (int.TryParse(data["Window"]["VSync"], out int vs)) engineSettings.Window_VSync = vs;
            if (int.TryParse(data["Window"]["FPS"], out int fps)) engineSettings.Window_FPS = fps;
        }


        // Stage 1
        profile.Initialize();
        _window = profile.CreateWindowSurface(engineSettings.Window_Title, engineSettings.Window_Width, engineSettings.Window_Height);
        EngineContext.Register<IWindowSurface>(_window);

        _window.OnLoad += () =>
        {
            Logger.Info("OnWindowLoad Started...");

            _engineSubSystemsManager.Add(profile.CreateEngineSubSystems());
        };

        _window.Initialize();

        // Stage 2 Services

        // --Register Core engine Services--
        IFileManager fileManager = new FileManager();
        EngineContext.Register<IFileManager>(fileManager);

        IAssetsManager assetsManager = new AssetsManager();
        EngineContext.Register<IAssetsManager>(assetsManager);

        // --Register AssetsLoaders--
        IGraphicsDevice graphicsDevice = EngineContext.Get<IGraphicsDevice>();
        IGraphicsFactory graphicsFactory = graphicsDevice.Factory;

        // Mesh
        assetsManager.MountAssetsLoader<MeshAsset, MeshOptions>(new AssetLoader_MeshAsset(graphicsFactory)); // GPU Mesh
        assetsManager.MountAssetsLoader<STDMeshData, MeshOptions>(new AssetLoader_STDMeshData()); // CPU Vertecies
        assetsManager.MountAssetsLoader<MeshDataRaw, MeshOptions>(new AssetLoader_MeshDataRaw()); // Raw CPU Mesh Data (Pos, Norm, UV)

        // Texture
        assetsManager.MountAssetsLoader<TextureData, TextureOptions>(new AssetsLoader_TextureData(fileManager)); // Raw CPU Texture Data
        assetsManager.MountAssetsLoader<TextureAsset, TextureOptions>(new AssetsLoader_TextureAsset(fileManager, graphicsFactory)); // GPU Texture

        // Pipeline & Material
        assetsManager.MountAssetsLoader<ShaderData, ShaderOptions>(new AssetsLoader_ShaderData(fileManager, graphicsFactory)); // Raw CPU ShaderData
        assetsManager.MountAssetsLoader<PipelineAsset, PipelineOptions>(new AssetsLoader_PipelineAsset(fileManager, graphicsFactory)); // Raw CPU PipelineAsset
        assetsManager.MountAssetsLoader<PBRMaterial, MaterialOptions>(new AssetsLoader_PBRMaterial(fileManager, graphicsFactory)); // PBR Material

        _multiverse = new Multiverse();
        EngineContext.Register<Multiverse>(_multiverse);

        LightingService lightingService = new LightingService();
        EngineContext.Register<LightingService>(lightingService);

        _renderer = new Renderer(_window, graphicsDevice);
        EngineContext.Register<IRenderer>(_renderer);

        game.OnPreInit();
    }

    private void Init()
    {
        Logger.Info("Init");

        // Initialize Arch ECS SharedJobScheduler for parallel queries
        var archJobConfig = new Schedulers.JobScheduler.Config
        {
            ThreadPrefixName = "VoxelEngine_Arch",
            ThreadCount = 0, // Auto-detect core count
            MaxExpectedConcurrentJobs = 256,
            StrictAllocationMode = false
        };
        World.SharedJobScheduler = new Schedulers.JobScheduler(archJobConfig);

        _engineSubSystemsManager.OnInitialize();
        game.OnInit();
    }

    private void Run()
    {
        Logger.Info("Run");

        Heartbeat heartbeat = new();
        EngineLoop engineLoop = new(_engineSubSystemsManager, _renderer!, _multiverse!);

        heartbeat.SetTargetFrameRate(165);
        heartbeat.Start(engineLoop, _window!);
    }

    private void ShutDown()
    {
        Logger.Info("ShutDown");

        _engineSubSystemsManager.Dispose();
        _window?.Dispose();
    }

}
