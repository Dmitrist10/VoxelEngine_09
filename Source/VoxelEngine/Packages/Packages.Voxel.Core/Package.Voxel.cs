using VoxelEngine.Core;
using VoxelEngine.Core.Runtime;

namespace VoxelEngine.Packages.Voxel;

public sealed class VoxelPackage : IEnginePackage
{
    public string Name => "Voxel Core Package";

    public void OnGameStarted()
    {

    }

    public void OnInitialize()
    {
    }

    public void OnSceneCreated(Scene scene)
    {
        SEntityProcessorGroup processorGroup = scene.CreateEPGroup();
        processorGroup.Add<EP_VoxelWorld>();

        scene.AddProcessor(processorGroup);
    }

    public void OnUniverseCreated(Universe universe)
    {
    }
}
