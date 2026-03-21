namespace VoxelEngine.Rendering;

[Flags]
public enum CameraClearFlags
{
    None = 0,
    Color = 1 << 0,
    Depth = 1 << 1,
    All = Color | Depth
}

[Flags]
public enum RenderLayers
{
    None = 0,
    Default = 1 << 0,
    Transparent = 1 << 1,
    UI = 1 << 2,
    Custom01 = 1 << 3,
    Custom02 = 1 << 4,
    Custom03 = 1 << 5,
    Custom04 = 1 << 6,
    Custom05 = 1 << 7,
    Custom06 = 1 << 8,
    Custom07 = 1 << 9,
    Custom08 = 1 << 10,
    Custom09 = 1 << 11,
    Custom10 = 1 << 12,
    Custom11 = 1 << 13,
    Custom12 = 1 << 14,
    Custom13 = 1 << 15,
    Custom14 = 1 << 16,
    Custom15 = 1 << 17,
    Custom16 = 1 << 18,
    Custom17 = 1 << 19,
    Custom18 = 1 << 20,
    Custom19 = 1 << 21,
    Custom20 = 1 << 22,
    Custom21 = 1 << 23,
    Custom22 = 1 << 24,
    Custom23 = 1 << 25,
    Custom24 = 1 << 26,
    Custom25 = 1 << 27,
    Custom26 = 1 << 28,
    Custom27 = 1 << 29,
    Custom28 = 1 << 30,
    Custom29 = 1 << 31,
    All = Default | Transparent | UI | Custom01 | Custom02 | Custom03 | Custom04 | Custom05 | Custom06 | Custom07 | Custom08 | Custom09 | Custom10 | Custom11 | Custom12 | Custom13 | Custom14 | Custom15 | Custom16 | Custom17 | Custom18 | Custom19 | Custom20 | Custom21 | Custom22 | Custom23 | Custom24 | Custom25 | Custom26 | Custom27 | Custom28 | Custom29
    // ALL = int.MaxValue,
}
