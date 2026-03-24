// using Arch.Core;
// using VoxelEngine.Common;
// using VoxelEngine.Core;

// namespace VoxelEngine.Physics;

// internal sealed class EP_Physics : EntityProcessor, IFixedUpdatable
// {
//     private readonly QueryDescription query;
//     private readonly IPhysics physics;
//     private readonly IPhysicsWorld physicsWorld;

//     public EP_Physics()
//     {
//         query = new QueryDescription().WithAll<C_RigidBody>();

//         physics = EngineContext.Get<IPhysics>();
//         physicsWorld = physics.CreatePhysicsWorld();
//     }

//     public void OnFixedUpdate()
//     {
//         world.Query(query, (ref C_RigidBody rigidBody) =>
//         {
//             // your logic here
//         }); 

//         physicsWorld.Step(Time.FixedDeltaTime);

//     }

// }