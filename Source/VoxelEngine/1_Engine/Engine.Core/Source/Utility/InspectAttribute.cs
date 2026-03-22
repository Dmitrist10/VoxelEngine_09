namespace VoxelEngine.Core;

/// <summary>
/// Mark a public field on a component struct with [Inspect] to expose it
/// in the runtime ImGui Inspector panel.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public sealed class InspectAttribute : Attribute
{
    /// <summary>Optional label override shown in the Inspector.</summary>
    public string? Label { get; init; }

    public InspectAttribute(string? label = null)
    {
        Label = label;
    }
}
