using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Arch.Core;

namespace VoxelEngine.Core;

public readonly partial struct Actor
{

    #region Components


    [MethodImpl(AggressiveInlining)]
    public void AddComponent<TComponent>(in TComponent component) where TComponent : struct
    {
        scene.World.Add(entity, component);
    }

    [MethodImpl(AggressiveInlining)]
    public void AddComponent<TComponent>() where TComponent : struct
    {
        scene.World.Add<TComponent>(entity);
    }

    [MethodImpl(AggressiveInlining)]
    public bool TryGetComponent<T>([NotNullWhen(true)] out T component) where T : struct
    {
        return scene.World.TryGet<T>(entity, out component);
    }

    public delegate void ComponentModifier<T>(ref T component);

    [MethodImpl(AggressiveInlining)]
    public void ModifyComponent<T>(ComponentModifier<T> modifier) where T : struct
    {
        modifier(ref scene.World.Get<T>(entity));
    }

    // [MethodImpl(AggressiveInlining)]
    // public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : unmanaged, IComponent
    // {
    //     component = Scene.World.Get<TComponent>(Entity);
    //     return component != null;
    // }
    [MethodImpl(AggressiveInlining)]
    public ref TComponent GetComponent<TComponent>() where TComponent : struct
    {
        return ref scene.World.Get<TComponent>(entity);
    }

    [MethodImpl(AggressiveInlining)]
    public void RemoveComponent<TComponent>() where TComponent : struct
    {
        scene.World.Remove<TComponent>(entity);
    }

    [MethodImpl(AggressiveInlining)]
    public bool HasComponent<TComponent>() where TComponent : struct
    {
        return scene.World.Has<TComponent>(entity);
    }

    #endregion

}
