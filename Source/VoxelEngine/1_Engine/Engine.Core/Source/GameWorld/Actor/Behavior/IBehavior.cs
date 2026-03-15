namespace VoxelEngine.Core;

internal interface IBehavior
{
    void OnAwake();
    void OnStart();
    void OnDestroy();

    /// <summary>
    /// Called internally by the registry after OnDestroy.
    /// Invalidates the behavior so stale references fail obviously.
    /// </summary>
    void Invalidate();
}