using VoxelEngine.Assets;
using VoxelEngine.Core.Features;
using VoxelEngine.Graphics;
using VoxelEngine.IO;

namespace VoxelEngine.Core.Runtime;

public sealed class CoreFeaturesCollection : IFeatureCollection
{
    public List<IFeature> GetFeatures()
    {
        return new List<IFeature>()
        {
            new CoreFeature(),
            new Feature_IO(),
            new GraphicsFeature(),
            new Features_Assets()
        };
    }
}
