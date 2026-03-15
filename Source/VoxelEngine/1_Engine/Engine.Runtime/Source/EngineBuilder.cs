
namespace VoxelEngine.Core.Runtime;

public sealed class EngineBuilder
{

    private IPlatform? _platform;
    private IRuntimeContext? _context;

    public EngineBuilder()
    {

    }

    public EngineBuilder WithPlatform(IPlatform platform)
    {
        _platform = platform;
        return this;
    }

    public EngineBuilder WithRuntimeContext(IRuntimeContext state)
    {
        _context = state;
        return this;
    }

    public Engine Build()
    {
        if (_platform == null || _context == null)
            throw new ArgumentException("IEngineState and IPlatform cannot be null.");

        Engine engine = new Engine(_platform, _context);

        return engine;
    }

}
