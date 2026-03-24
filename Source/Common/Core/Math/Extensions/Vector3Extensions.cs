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

        public Quaternion EulerToQuaternion()
        {
            Vector3 r = from * (MathF.PI / 180f);
            return Quaternion.CreateFromYawPitchRoll(r.Y, r.X, r.Z);
        }

    }
    extension(Quaternion from)
    {

        public Vector3 QuaternionToEuler()
        {
            // Extraction for System.Numerics order (Roll, then Pitch, then Yaw)
            // Yaw = Y, Pitch = X, Roll = Z
            float s = 2 * (from.W * from.X - from.Y * from.Z);
            float x = MathF.Asin(s > 1f ? 1f : (s < -1f ? -1f : s));

            float y, z;
            if (MathF.Abs(x) < MathF.PI / 2 - 0.001f)
            {
                y = MathF.Atan2(2 * (from.W * from.Y + from.X * from.Z), 1 - 2 * (from.X * from.X + from.Y * from.Y));
                z = MathF.Atan2(2 * (from.W * from.Z + from.X * from.Y), 1 - 2 * (from.X * from.X + from.Z * from.Z));
            }
            else
            {
                // Gimbal lock
                y = MathF.Atan2(2 * (from.Y * from.Z - from.W * from.X), 2 * (from.W * from.W + from.Z * from.Z) - 1);
                z = 0;
            }

            return new Vector3(x, y, z) * (180f / MathF.PI);
        }

    }

}

