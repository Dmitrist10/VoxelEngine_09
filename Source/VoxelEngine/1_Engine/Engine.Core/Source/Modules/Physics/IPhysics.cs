namespace VoxelEngine.Physics;

public interface IPhysics
{
    IPhysicsWorld CreatePhysicsWorld();
}

public interface IPhysicsWorld
{
    void Step(float deltaTime);
}

public interface IPhysicsDriver
{
    IPhysics CreatePhysics();
}