using VoxelEngine.Common;
using VoxelEngine.Graphics;
using VoxelEngine.Windowing;
using VoxelEngine.Diagnostics;

using VoxelEngine.Assets;
using VoxelEngine.IO.FilesManagement;
using VoxelEngine.Core.UGC;
using VoxelEngine.Rendering;

namespace VoxelEngine.Core.Runtime;

public sealed class Engine
{
    private IPlatform _platform;
    private IRuntimeContext _runtimeContext;
    private GameBase _game;

    private IWindowSurface? _windowSurface;
    private UniverseManager? _universeManager;
    private IRenderer? _renderer;

    public Engine(IPlatform platform, IRuntimeContext runtimeContext, GameBase game)
    {
        _platform = platform;
        _runtimeContext = runtimeContext;
        _game = game;
    }

    public void StartEngine()
    {
        PreInit();
        Init();
        PostInit();

        Run();
    }

    private void PreInit()
    {
        // Init EngineContext
        IEventBus eventBus = new EventBus();
        IServiceContainer serviceContainer = new ServiceContainer();

        EngineContext engineContext = new(eventBus, serviceContainer);
        EngineContext.Instance = engineContext;
    }

    private void Init()
    {
        Logger.Debug("[Engine] Initializing Core Services & Drivers...");

        // Create Window & Initialize it
        _windowSurface = _platform.CreateWindowSurface();
        EngineContext.Register<IWindowSurface>(_windowSurface);
        _windowSurface.OnLoad += () =>
        {
            EngineContext.Publish(new Event_WindowLoadedEvent(_windowSurface, _platform));

            // Create GraphicsDriver 
            IGraphicsDriver graphicsDriver = _platform.CreateGraphicsDriver();
            EngineContext.Register<IGraphicsDriver>(graphicsDriver);
            graphicsDriver.CreateGraphicsDevice();
            graphicsDriver.Initialize();
        };

        _windowSurface.Initialize();


        // --Register Core engine Services--
        IFileManager fileManager = new FileManager();
        EngineContext.Register<IFileManager>(fileManager);

        IAssetsManager assetsManager = new AssetsManager();
        EngineContext.Register<IAssetsManager>(assetsManager);

        // --Register AssetsLoaders--
        IGraphicsFactory graphicsFactory = EngineContext.Get<IGraphicsDevice>().Factory;

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

        _universeManager = new UniverseManager();
        EngineContext.Register<UniverseManager>(_universeManager);

        _renderer = new Renderer();
        EngineContext.Register<IRenderer>(_renderer);
    }


    private void PostInit()
    {
        _game.universeManager = _universeManager!;
        _game.OnInitialize();
    }

    private void Run()
    {
        Heartbeat heartbeat = new();
        EngineLoop engineLoop = new(_universeManager!, _runtimeContext, _renderer!);

        heartbeat.SetTargetFrameRate(165);
        heartbeat.Start(engineLoop, _windowSurface!);
    }


}
