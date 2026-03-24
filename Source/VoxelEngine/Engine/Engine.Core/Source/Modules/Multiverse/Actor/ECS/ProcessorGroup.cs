using System;

namespace VoxelEngine.Core;

[Flags]
public enum ProcessorGroup
{
    None = 0,
    Engine = 1 << 0,  // Always runs (Transform, Rendering, Camera, UI)
    Game = 1 << 1,    // Only runs in Play mode (Physics, AI, Gameplay logic)
    Editor = 1 << 2,  // Only runs in Editor mode (Gizmos, Editor UI)
    All = Engine | Game | Editor
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ProcessorGroupAttribute : Attribute
{
    public ProcessorGroup Group { get; }
    public ProcessorGroupAttribute(ProcessorGroup group) => Group = group;
}
