namespace VoxelEngine.Graphics;

public record struct PipelineDescription
{
    public required string VertexShaderSource;
    public required string FragmentShaderSource;

    public PipelineDescription(string vertexShaderSource, string fragmentShaderSource) : this()
    {
        VertexShaderSource = vertexShaderSource;
        FragmentShaderSource = fragmentShaderSource;
    }
}
