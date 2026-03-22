using Arch.Core;
using VoxelEngine.Common;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace TestingGame;

internal record struct C_Testing
{
    [Inspect] public float Speed;
    [Inspect] public bool IsActive;
    [Inspect("Jump Height")] public float JumpHeight;
}

internal sealed class EP_Testing : EntityProcessor, IUpdatable
{
    private QueryDescription query;

    public EP_Testing()
    {
        Group = ProcessorGroup.Game;
        query = new QueryDescription().WithAll<C_Testing, C_Transform>();
    }

    public void OnUpdate()
    {
        world.Query(query, (ref C_Testing testing, ref C_Transform transform) =>
        {
            transform.LocalPosition += transform.Forward * testing.Speed * Time.DeltaTime;
            transform.MarkDirty();
        });
    }

}