using VoxelEngine.Graphics;

namespace VoxelEngine.Kernel.Runtime;

public sealed class Engine
{
    private IPlatform platform;
    private IEngineState state;

    private Heartbeat heartBeat;
    private ApplicationHost application;

    private Kernel kernel;

    public Engine(IPlatform platform, IEngineState state)
    {
        this.platform = platform;
        this.state = state;

        heartBeat = new Heartbeat();
        application = new ApplicationHost();

        kernel = new Kernel();
    }

    public void Run()
    {
        IWindowSurface windowSurface = platform.CreateWindowSurface();

        kernel.Init();

        heartBeat.Start(application, windowSurface);
    }

}
