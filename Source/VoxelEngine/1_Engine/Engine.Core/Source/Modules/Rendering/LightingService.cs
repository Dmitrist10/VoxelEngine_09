using System.Numerics;

namespace VoxelEngine.Rendering;

public sealed class LightingService
{
    // Pointing INTO the scene along +Z (since camera looks at +Z objects)
    public Vector3 Direction { get; set; } = Vector3.Normalize(new Vector3(0.5f, -0.5f, 1.0f));
    public Vector3 Color { get; set; } = new Vector3(1.0f, 1.0f, 0.9f);
    public float AmbientIntensity { get; set; } = 2.5f;
    public float SpecularIntensity { get; set; } = 1.0f;
    public float Shininess { get; set; } = 256.0f;
}
