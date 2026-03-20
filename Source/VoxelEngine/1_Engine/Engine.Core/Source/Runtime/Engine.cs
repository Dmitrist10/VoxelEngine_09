using VoxelEngine.Common;
using VoxelEngine.Graphics;
using VoxelEngine.Windowing;
using VoxelEngine.Diagnostics;

using VoxelEngine.Assets;
using VoxelEngine.IO.FilesManagement;

namespace VoxelEngine.Core.Runtime;

public sealed class Engine
{
    private IPlatform _platform;
    private IWindowSurface? _windowSurface;

    public Engine(IPlatform platform)
    {
        _platform = platform;
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
        // Create Window & Initialize it
        _windowSurface = _platform.CreateWindowSurface();
        EngineContext.Register<IWindowSurface>(_windowSurface);
        _windowSurface.OnLoad += () =>
        {
            EngineContext.Publish(new Event_WindowLoadedEvent(_windowSurface, _platform));

            // Create GraphicsDriver 
            IGraphicsDriver graphicsDriver = _platform.CreateGraphicsDriver();
            EngineContext.Register<IGraphicsDriver>(graphicsDriver);
            graphicsDriver.Initialize();
        };

        _windowSurface.Initialize();
    }


    private void PostInit()
    {
        // Register Core engine Services
        IFileManager fileManager = new FileManager();
        EngineContext.Register<IFileManager>(fileManager);

        IAssetsManager assetsManager = new AssetsManager();
        EngineContext.Register<IAssetsManager>(assetsManager);

        // Register AssetsLoaders
        IGraphicsFactory graphicsFactory = EngineContext.Get<IGraphicsDevice>().Factory;

        assetsManager.MountAssetsLoader<MeshAsset, MeshOptions>(new AssetLoader_MeshAsset(graphicsFactory)); // GPU Mesh
        assetsManager.MountAssetsLoader<STDMeshData, MeshOptions>(new AssetLoader_STDMeshData()); // CPU Vertecies
        assetsManager.MountAssetsLoader<MeshDataRaw, MeshOptions>(new AssetLoader_MeshDataRaw()); // Raw CPU Mesh Data (Pos, Norm, UV)

        assetsManager.MountAssetsLoader<TextureData, TextureOptions>(new AssetsLoader_TextureData(fileManager)); // Raw CPU Texture Data
        assetsManager.MountAssetsLoader<TextureAsset, TextureOptions>(new AssetsLoader_TextureAsset(fileManager, graphicsFactory)); // GPU Texture

        assetsManager.MountAssetsLoader<ShaderData, ShaderOptions>(new AssetsLoader_ShaderData(fileManager, graphicsFactory)); // Raw CPU ShaderData
        // assetsManager.MountAssetsLoader<PipelineAsset, PipelineOptions>(new AssetsLoader_PipelineAsset(fileManager, graphicsFactory)); // Raw CPU PipelineAsset
    }

    private void Run()
    {
        Heartbeat heartbeat = new();
        EngineLoop engineLoop = new();

        heartbeat.Start(engineLoop, _windowSurface!);
    }


}
