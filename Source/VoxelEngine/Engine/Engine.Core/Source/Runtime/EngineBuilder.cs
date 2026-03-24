namespace VoxelEngine.Core.Runtime;

public sealed class EngineBuilder
{
    private IEngineProfile? profile;
    private IGame? game;

    public EngineBuilder WithProfile(IEngineProfile platform)
    {
        this.profile = platform;
        return this;
    }
    public EngineBuilder WithGame(IGame runtimeContext)
    {
        this.game = runtimeContext;
        return this;
    }


    internal Engine Build()
    {
        if (profile == null) throw new InvalidOperationException("Platform must be specified.");
        if (game == null) throw new InvalidOperationException("Runtime context must be specified.");

        return new Engine(profile, game);
    }

}
