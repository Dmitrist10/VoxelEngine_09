using VoxelEngine.Core;
using VoxelEngine.Rendering;

namespace VoxelEngine.Client;

public sealed class ClientFeaturesCollection : IFeatureCollection
{
    public List<IFeature> GetFeatures()
    {
        return new List<IFeature>()
        {
            new RenderingFeature()
        };
    }
}
