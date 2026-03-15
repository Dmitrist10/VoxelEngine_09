using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Arch.Core;

namespace VoxelEngine.Core;

public readonly partial struct Actor
{
    public static readonly Actor Null = default;

    public readonly Entity entity;
    public readonly Scene scene;
    // public readonly World World;
    public readonly World world => scene.World;

    public Actor(Entity entity, Scene scene)
    {
        this.entity = entity;
        this.scene = scene;
        // World = scene.World;
    }

    /// <summary>
    /// Checks if this actor is valid (entity exists and is alive).
    /// </summary>
    public bool IsValid => scene != null && scene.World.IsAlive(entity);

    #region Local Transform

    public Vector3 LocalPosition
    {
        [MethodImpl(AggressiveInlining)]
        get => world.Get<C_Transform>(entity).LocalPosition;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var transform = ref world.Get<C_Transform>(entity);
            transform.LocalPosition = value;
            transform.IsDirty = true;
        }
    }

    public Quaternion LocalRotation
    {
        [MethodImpl(AggressiveInlining)]
        get => world.Get<C_Transform>(entity).LocalRotation;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var transform = ref world.Get<C_Transform>(entity);
            transform.LocalRotation = value;
            transform.IsDirty = true;
        }
    }

    public Vector3 LocalScale
    {
        [MethodImpl(AggressiveInlining)]
        get => world.Get<C_Transform>(entity).LocalScale;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var transform = ref world.Get<C_Transform>(entity);
            transform.LocalScale = value;
            transform.IsDirty = true;
        }
    }

    #endregion

    #region World Transform

    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation);
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);
    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 Position
    {
        [MethodImpl(AggressiveInlining)]
        get => world.Get<C_Transform>(entity).WorldPosition;
        [MethodImpl(AggressiveInlining)]
        set
        {
            // Convert World position to local space
            ref var hierarchy = ref world.Get<C_Hierarchy>(entity);
            if (hierarchy.Parent == Entity.Null)
            {
                // Root actor: local = World
                LocalPosition = value;
            }
            else
            {
                // Has parent: calculate local from World
                ref var parentTransform = ref world.Get<C_Transform>(hierarchy.Parent);
                Vector3 localPos = value - parentTransform.WorldPosition;
                localPos = Vector3.Transform(localPos, Quaternion.Inverse(parentTransform.WorldRotation));
                localPos /= parentTransform.WorldScale;
                LocalPosition = localPos;
                // parentTransform.IsDirty = true;
            }
        }
    }

    public Quaternion Rotation
    {
        [MethodImpl(AggressiveInlining)]
        get => world.Get<C_Transform>(entity).WorldRotation;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var hierarchy = ref world.Get<C_Hierarchy>(entity);
            if (hierarchy.Parent == Entity.Null)
            {
                LocalRotation = value;
            }
            else
            {
                ref var parentTransform = ref world.Get<C_Transform>(hierarchy.Parent);
                LocalRotation = Quaternion.Inverse(parentTransform.WorldRotation) * value;
            }
        }
    }

    public Vector3 Scale
    {
        [MethodImpl(AggressiveInlining)]
        get => world.Get<C_Transform>(entity).WorldScale;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var hierarchy = ref world.Get<C_Hierarchy>(entity);
            if (hierarchy.Parent == Entity.Null)
            {
                LocalScale = value;
            }
            else
            {
                ref var parentTransform = ref world.Get<C_Transform>(hierarchy.Parent);
                LocalScale = value / parentTransform.WorldScale;
            }
        }
    }

    public Matrix4x4 WorldMatrix => world.Get<C_WorldTransformMatrix>(entity).WorldMatrix;

    #endregion


    #region Hierarchy

    public Actor Parent
    {
        get
        {
            var parentEntity = HierarchyOperations.GetParent(world, entity);
            if (parentEntity == Entity.Null)
                return Actor.Null;
            return new Actor(parentEntity, scene);
        }
        set
        {
            if (!value.IsValid)
            {
                DetachFromParent();
            }
            else
            {
                SetParent(value.entity);
            }
        }
    }

    public int ChildCount => world.Get<C_Hierarchy>(entity).ChildCount;

    public bool IsRoot => world.Get<C_Hierarchy>(entity).IsRoot;

    public int Depth => world.Get<C_Hierarchy>(entity).Depth;

    public void SetParent(Entity parentEntity, bool worldPositionStays = true)
    {
        HierarchyOperations.SetParent(world, entity, parentEntity, worldPositionStays);
    }

    public void DetachFromParent(bool worldPositionStays = true)
    {
        HierarchyOperations.DetachFromParent(world, entity, worldPositionStays);
    }

    public Actor[] GetChildren()
    {
        var childEntities = HierarchyOperations.GetChildren(world, entity);
        var children = new Actor[childEntities.Count];

        for (int i = 0; i < childEntities.Count; i++)
        {
            children[i] = new Actor(childEntities[i], scene);
        }

        return children;
    }

    public List<Entity> GetChildrenEntities()
    {
        return HierarchyOperations.GetChildren(world, entity);
    }

    public Actor GetChild(int index)
    {
        var child = HierarchyOperations.GetChild(world, entity, index);
        if (child == Entity.Null) return Actor.Null;
        return new Actor(child, scene);
    }

    public Actor FindChild(string name)
    {
        var child = HierarchyOperations.FindChild(world, entity, name);
        if (child == Entity.Null) return Actor.Null;
        return new Actor(child, scene);
    }

    #endregion


    public string Name
    {
        get => world.Get<C_Actor>(entity).Name;
        set
        {
            ref var actor = ref world.Get<C_Actor>(entity);
            actor.Name = value;
        }
    }
    public Guid ID
    {
        get => world.Get<C_ID>(entity).ID;
        set
        {
            ref var id = ref world.Get<C_ID>(entity);
            id.ID = value;
        }
    }



    #region Safety Operations

    /// <summary>
    /// Executes an action only if the actor is valid.
    /// </summary>
    public void IfValid(Action<Actor> action)
    {
        if (IsValid)
            action(this);
    }

    /// <summary>
    /// Executes a function only if the actor is valid, otherwise returns default.
    /// </summary>
    public T IfValid<T>(Func<Actor, T> func, T defaultValue = default!)
    {
        return IsValid ? func(this) : defaultValue;
    }

    #endregion

    #region Extra

    public int Version => entity.Version;

    public override int GetHashCode() => entity.GetHashCode();

    public override bool Equals(object? obj) => obj is Actor other && Equals(other);

    // public bool Equals(Actor other) => scene == other.scene && entity.Id == other.entity.Id;
    public bool Equals(Actor other) => entity.Id == other.entity.Id;

    public override string ToString() => $"(Actor: {Name}, ID: {ID} , Entity: {entity} , Scene: {scene.Name})";

    public static bool operator ==(Actor left, Actor right) => left.Equals(right);
    public static bool operator !=(Actor left, Actor right) => !left.Equals(right);

    #endregion



}
