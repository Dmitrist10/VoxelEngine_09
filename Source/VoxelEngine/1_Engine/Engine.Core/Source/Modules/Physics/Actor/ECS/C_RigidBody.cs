namespace VoxelEngine.Physics;

public record struct C_RigidBody
{
    public float Mass = 1;
    public float Friction = 0.5f;
    public float Restitution = 0.5f;

    public PhysicsBodyType Type;

    public PhysicsHandle Handle;
    public bool isDirty;

    public C_RigidBody()
    {
    }


}
