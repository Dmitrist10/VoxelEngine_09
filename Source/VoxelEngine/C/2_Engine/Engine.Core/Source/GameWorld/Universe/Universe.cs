namespace VoxelEngine.Core;

public sealed class Universe
{

    public Scene Create()
    {
        Scene s = new();
        return s;
    }

}