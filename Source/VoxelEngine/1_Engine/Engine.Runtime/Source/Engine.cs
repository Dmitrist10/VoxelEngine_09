using VoxelEngine.Common;
using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace VoxelEngine.Core.Runtime;

public sealed class Engine
{
    private IPlatform platform;
    private IRuntimeContext runtimeContext;

    private Kernel kernel;
    private EngineContext engineContext;
    private Heartbeat heartBeat;
    private EngineLoop engineLoop;

    private IWindowSurface? windowSurface;

    public Engine(IPlatform platform, IRuntimeContext runtimeContext)
    {
        this.platform = platform;
        this.runtimeContext = runtimeContext;

        IEventBus eventBus = new EventBus();
        IServiceContainer serviceContainer = new ServiceContainer();
        engineContext = new(eventBus, serviceContainer);

        EngineContext.Instance = engineContext;

        heartBeat = new Heartbeat();
        engineLoop = new EngineLoop(runtimeContext);

        kernel = new Kernel();
    }

    public void Run()
    {
        PreInit();

        windowSurface = platform.CreateWindowSurface();
        windowSurface.OnLoad += OnLoad;
        windowSurface.Initialize();
        windowSurface.OnLoad -= OnLoad;

        Init();

        PostInit();
        // kernel.Init();

        heartBeat.SetTargetFrameRate(165);
        heartBeat.Start(engineLoop, windowSurface);
    }


    private void PreInit()
    {
        EngineLoadingState_PreInit preInit = new EngineLoadingState_PreInit();
        engineContext.EventBus.Publish(preInit);
    }

    private void OnLoad()
    {
        IGraphicsDevice graphicsDevice = platform.CreateGraphicsDevice();
        GraphicsContext graphicsContext = new(windowSurface!, graphicsDevice);
        engineContext.ServiceContainer.Register(graphicsContext);

        graphicsDevice.Submit();
    }

    private void Init()
    {
        EngineLoadingState_Init init = new EngineLoadingState_Init();
        engineContext.EventBus.Publish(init);


        EngineContext.Register(new UniverseManager());
    }

    private void PostInit()
    {
        EngineLoadingState_PostInit postInit = new EngineLoadingState_PostInit();
        engineContext.EventBus.Publish(postInit);
    }

}

public readonly struct EngineLoadingState_PreInit
{
    
}
public readonly struct EngineLoadingState_Init
{
    
}
public readonly struct EngineLoadingState_PostInit
{
    
}