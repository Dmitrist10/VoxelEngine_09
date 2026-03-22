namespace VoxelEngine.Core;

/// <summary>
/// Marks a field or property to be visible in the Inspector.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public sealed class InspectAttribute : Attribute
{
    /// <summary>Optional label override shown in the Inspector.</summary>
    public string? Label { get; init; }

    public InspectAttribute(string? label = null)
    {
        Label = label;
    }
}

/// <summary>
/// Makes field not automatically visible in the Inspector.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public sealed class NoInspectAttribute : Attribute
{
    public NoInspectAttribute() { }
}

/// <summary>
/// Makes so that a field or component is only visible in the Inspector, but not editable.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public sealed class OnlyView : Attribute
{
    public OnlyView() { }
}

/// <summary>
/// Adds min and max values to the slider.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public sealed class MinMax : Attribute
{
    public float Min { get; init; }
    public float Max { get; init; }

    public MinMax(float min, float max) { Min = min; Max = max; }
}
