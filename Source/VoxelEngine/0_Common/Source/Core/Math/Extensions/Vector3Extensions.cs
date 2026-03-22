using System.Numerics;

namespace VoxelEngine.Common;

public static class Vector3Extensions
{
    // public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float delta)
    // {

    // }

    extension(Vector3 from)
    {
        public Int3 ToInt3()
        {
            return new Int3((int)from.X, (int)from.Y, (int)from.Z);
        }

        public Quaternion ToQuaternion()
        {
            // return Quaternion.CreateFromYawPitchRoll(from.Y * EMath.Deg2Rad, from.X * EMath.Deg2Rad, from.Z * EMath.Deg2Rad);
            return Quaternion.CreateFromAxisAngle(Vector3.UnitY, from.Y * EMath.Deg2Rad) * 
                   Quaternion.CreateFromAxisAngle(Vector3.UnitX, from.X * EMath.Deg2Rad) * 
                   Quaternion.CreateFromAxisAngle(Vector3.UnitZ, from.Z * EMath.Deg2Rad);
        }

    }

}

