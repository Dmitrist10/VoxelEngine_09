using VoxelEngine.Common;
using VoxelEngine.Core.UGC;

namespace VoxelEngine.Core.Runtime;

public sealed class EngineBuilder
{
    private IPlatform? platform;
    private IRuntimeContext? runtimeContext;
    private GameBase? game;
    // private readonly List<IFeatureCollection> featureCollections = new();

    public EngineBuilder WithPlatform(IPlatform platform)
    {
        this.platform = platform;
        return this;
    }
    public EngineBuilder WithRuntimeContext(IRuntimeContext runtimeContext)
    {
        this.runtimeContext = runtimeContext;
        return this;
    }

    // public EngineBuilder AddFeatureCollection(IFeatureCollection collection)
    // {
    //     featureCollections.Add(collection);
    //     return this;
    // }
    // public EngineBuilder AddFeatureCollectionsList(List<IFeatureCollection> collections)
    // {
    //     featureCollections.AddRange(collections);
    //     return this;
    // }

    public Engine Build()
    {
        if (platform == null)       throw new InvalidOperationException("Platform must be specified.");
        if (runtimeContext == null) throw new InvalidOperationException("Runtime context must be specified.");
        if (game == null)           throw new InvalidOperationException("Game must be specified.");

        // return new Engine(platform, runtimeContext, featureCollections);
        return new Engine(platform, runtimeContext, game);
    }

    public EngineBuilder WithGame(GameBase game)
    {
        this.game = game;
        return this;
    }
}
