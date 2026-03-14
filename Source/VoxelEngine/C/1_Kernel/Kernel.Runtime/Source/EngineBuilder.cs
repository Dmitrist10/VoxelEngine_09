
namespace VoxelEngine.Kernel.Runtime;

public sealed class EngineBuilder
{

    private IPlatform? _platform;
    private IEngineState? _state;

    public EngineBuilder()
    {

    }

    public EngineBuilder WithPlatform(IPlatform platform)
    {
        _platform = platform;
        return this;
    }

    public EngineBuilder WithEngineState(IEngineState state)
    {
        _state = state;
        return this;
    }

    public Engine Build()
    {
        if (_platform == null || _state == null)
            throw new ArgumentException("IEngineState and IPlatform cannot be null.");

        Engine engine = new Engine(_platform, _state);

        return engine;
    }

}
