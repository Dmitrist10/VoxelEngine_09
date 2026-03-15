using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Arch.Core;

namespace VoxelEngine.Core;

public readonly partial struct Actor
{

    #region Behavior

    public T? GetBehavior<T>() where T : Behavior
    {
        return scene.GetBehavior<T>(entity);
    }

    public bool TryGetBehavior<T>(out T behavior) where T : Behavior
    {
        var result = scene.GetBehavior<T>(entity);
        if (result != null)
        {
            behavior = result;
            return true;
        }

        behavior = null!;
        return false;
    }

    public void RemoveBehavior<T>() where T : Behavior
    {
        scene.RemoveBehavior<T>(entity);
    }

    public bool HasBehavior<T>() where T : Behavior
    {
        return scene.HasBehavior<T>(entity);
    }

    public T AddBehavior<T>(T instance) where T : Behavior
    {
        return scene.AddBehavior<T>(entity, instance);
    }

    public T AddBehavior<T>() where T : Behavior, new()
    {
        return scene.AddBehavior<T>(entity);
    }

    #endregion



}
