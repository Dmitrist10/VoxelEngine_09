using VoxelEngine.Assets;
using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public record struct PipelineAsset(PipelineHandle Handle) : IAsset
{
    // public PipelineHandle Handle;
    // public 
}

public record PipelineData
{
    public readonly ShaderData Shader;
    public readonly PrimitiveTopology Topology;

    public PipelineData(ShaderData shader, PrimitiveTopology topology = PrimitiveTopology.Triangles)
    {
        Shader = shader;
        Topology = topology;
    }
}
