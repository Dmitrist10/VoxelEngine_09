namespace VoxelEngine.Packages;

public interface IPackage
{
    public string Name { get; }
    public Version Version { get; }

    void OnInitialize();
    void OnTick();
    void OnUpdate();
    void OnCleanUp();

    // void OnUniverseCreated(Universe universe);
    // void OnSceneCreated(Scene scene);

}