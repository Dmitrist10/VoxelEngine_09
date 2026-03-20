using VoxelEngine.Core;

namespace VoxelEngine.Server;

public sealed class ServerFeaturesCollection : IFeatureCollection
{
    public List<IFeature> GetFeatures()
    {
        return new List<IFeature>()
        {
            // new RenderingFeature()
        };
    }
}
