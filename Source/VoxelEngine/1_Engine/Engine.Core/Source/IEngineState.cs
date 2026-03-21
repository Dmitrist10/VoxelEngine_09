namespace VoxelEngine.Core;

public interface IEngineState
{
    static abstract string Name { get; }
    void OnInitialize();
}