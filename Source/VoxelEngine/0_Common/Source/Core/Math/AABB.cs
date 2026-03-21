using System.Numerics;

namespace VoxelEngine.Common;

public record struct AABB
{
    public Vector3 Min;
    public Vector3 Max;

    public AABB(Vector3 min, Vector3 max) : this()
    {
        Min = min;
        Max = max;
    }


    /// <summary>
    /// Checks if this AABB intersects with another AABB.
    /// Uses SIMD-accelerated Vector3 operations.
    /// </summary>
    [MethodImpl(AggressiveInlining)]
    public bool Intersects(in AABB other)
    {
        // Standard AABB collision: (a.Min <= b.Max) && (a.Max >= b.Min)
        return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
               (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
               (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
    }

    /// <summary>
    /// Checks if a point is contained within the AABB.
    /// </summary>
    [MethodImpl(AggressiveInlining)]
    public bool Contains(Vector3 point)
    {
        return point.X >= Min.X && point.X <= Max.X &&
               point.Y >= Min.Y && point.Y <= Max.Y &&
               point.Z >= Min.Z && point.Z <= Max.Z;
    }

    /// <summary>
    /// Expands the AABB to include a new point.
    /// </summary>
    public AABB Encapsulate(Vector3 point)
    {
        return new AABB(
            Vector3.Min(Min, point),
            Vector3.Max(Max, point)
        );
    }

    public bool IsInFrustum(ReadOnlySpan<Vector4> planes)
    {
        Vector3 center = (Min + Max) * 0.5f;
        Vector3 extents = (Max - Min) * 0.5f;

        for (int i = 0; i < 6; i++)
        {
            Vector4 plane = planes[i];
            Vector3 normal = new(plane.X, plane.Y, plane.Z);

            // Compute the projection interval radius r
            float r = extents.X * MathF.Abs(normal.X) +
                      extents.Y * MathF.Abs(normal.Y) +
                      extents.Z * MathF.Abs(normal.Z);

            // Compute distance of box center from plane
            float s = Vector3.Dot(normal, center) + plane.W;

            // If the box is completely behind any plane, it's culled
            if (s <= -r) return false;
        }
        return true;
    }

    // Add this to your AABB struct
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOutsidePlane(in Plane plane)
    {
        Vector3 center = (Max + Min) * 0.5f;
        Vector3 extents = (Max - Min) * 0.5f;

        // Projected radius of the AABB onto the plane normal
        float r = extents.X * MathF.Abs(plane.Normal.X) +
                  extents.Y * MathF.Abs(plane.Normal.Y) +
                  extents.Z * MathF.Abs(plane.Normal.Z);

        float distance = Vector3.Dot(plane.Normal, center) + plane.D;

        return distance < -r;
    }


}

public readonly struct Frustum
{
    // Store 6 planes as an array or ReadOnlySpan
    public readonly Plane[] Planes;

    public Frustum(Matrix4x4 vp)
    {
        Planes = new Plane[6];
        // Plane normalization is handled automatically by Normalize()
        Planes[0] = Plane.Normalize(new Plane(vp.M14 + vp.M11, vp.M24 + vp.M21, vp.M34 + vp.M31, vp.M44 + vp.M41)); // Left
        Planes[1] = Plane.Normalize(new Plane(vp.M14 - vp.M11, vp.M24 - vp.M21, vp.M34 - vp.M31, vp.M44 - vp.M41)); // Right
        Planes[2] = Plane.Normalize(new Plane(vp.M14 + vp.M12, vp.M24 + vp.M22, vp.M34 + vp.M32, vp.M44 + vp.M42)); // Bottom
        Planes[3] = Plane.Normalize(new Plane(vp.M14 - vp.M12, vp.M24 - vp.M22, vp.M34 - vp.M32, vp.M44 - vp.M42)); // Top
        Planes[4] = Plane.Normalize(new Plane(vp.M13, vp.M23, vp.M33, vp.M43));                                   // Near
        Planes[5] = Plane.Normalize(new Plane(vp.M14 - vp.M13, vp.M24 - vp.M23, vp.M34 - vp.M33, vp.M44 - vp.M43)); // Far
    }

    public bool Contains(in AABB box)
    {
        for (int i = 0; i < 6; i++)
        {
            // Center-Extents check using built-in Plane.DotPlane
            if (IsOutside(Planes[i], box)) return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsOutside(Plane plane, in AABB box)
    {
        Vector3 center = (box.Max + box.Min) * 0.5f;
        Vector3 extents = (box.Max - box.Min) * 0.5f;

        // Radius of the AABB projected onto the plane's normal
        float r = extents.X * MathF.Abs(plane.Normal.X) +
                  extents.Y * MathF.Abs(plane.Normal.Y) +
                  extents.Z * MathF.Abs(plane.Normal.Z);

        // Distance from center to plane
        float distance = Vector3.Dot(plane.Normal, center) + plane.D;

        return distance < -r;
    }
}


// public record struct Frustum
// {

//     public static Vector4[] ExtractPlanes(Matrix4x4 m)
//     {
//         // Extract planes from a View-Projection matrix (Row-major)
//         var planes = new Vector4[6];
//         planes[0] = Vector4.Normalize(new(m.M14 + m.M11, m.M24 + m.M21, m.M34 + m.M31, m.M44 + m.M41)); // Left
//         planes[1] = Vector4.Normalize(new(m.M14 - m.M11, m.M24 - m.M21, m.M34 - m.M31, m.M44 - m.M41)); // Right
//         planes[2] = Vector4.Normalize(new(m.M14 - m.M12, m.M24 - m.M22, m.M34 - m.M32, m.M44 - m.M42)); // Top
//         planes[3] = Vector4.Normalize(new(m.M14 + m.M12, m.M24 + m.M22, m.M34 + m.M32, m.M44 + m.M42)); // Bottom
//         planes[4] = Vector4.Normalize(new(m.M13, m.M23, m.M33, m.M43));           // Near (D3D)
//         planes[5] = Vector4.Normalize(new(m.M14 - m.M13, m.M24 - m.M23, m.M34 - m.M33, m.M44 - m.M43)); // Far
//         return planes;
//     }


// }


// public readonly struct Frustum
// {
//     // Ordered for early-out: Left, Right, Bottom, Top, Near, Far
//     public readonly Plane[] Planes;

//     public Frustum(Matrix4x4 viewProjection)
//     {
//         Planes = new Plane[6];

//         // Gribb-Hartmann extraction method for Row-Major matrices
//         Planes[0] = new Plane(new(viewProjection.M14 + viewProjection.M11, viewProjection.M24 + viewProjection.M21, viewProjection.M34 + viewProjection.M31, viewProjection.M44 + viewProjection.M41)); // Left
//         Planes[1] = new Plane(new(viewProjection.M14 - viewProjection.M11, viewProjection.M24 - viewProjection.M21, viewProjection.M34 - viewProjection.M31, viewProjection.M44 - viewProjection.M41)); // Right
//         Planes[2] = new Plane(new(viewProjection.M14 + viewProjection.M12, viewProjection.M24 + viewProjection.M22, viewProjection.M34 + viewProjection.M32, viewProjection.M44 + viewProjection.M42)); // Bottom
//         Planes[3] = new Plane(new(viewProjection.M14 - viewProjection.M12, viewProjection.M24 - viewProjection.M22, viewProjection.M34 - viewProjection.M32, viewProjection.M44 - viewProjection.M42)); // Top
//         Planes[4] = new Plane(new(viewProjection.M13, viewProjection.M23, viewProjection.M33, viewProjection.M43)); // Near
//         Planes[5] = new Plane(new(viewProjection.M14 - viewProjection.M13, viewProjection.M24 - viewProjection.M23, viewProjection.M34 - viewProjection.M33, viewProjection.M44 - viewProjection.M43)); // Far
//     }

//     public static Vector4[] ExtractPlanes(Matrix4x4 m)
//     {
//         // Extract planes from a View-Projection matrix (Row-major)
//         var planes = new Vector4[6];
//         planes[0] = Vector4.Normalize(new(m.M14 + m.M11, m.M24 + m.M21, m.M34 + m.M31, m.M44 + m.M41)); // Left
//         planes[1] = Vector4.Normalize(new(m.M14 - m.M11, m.M24 - m.M21, m.M34 - m.M31, m.M44 - m.M41)); // Right
//         planes[2] = Vector4.Normalize(new(m.M14 - m.M12, m.M24 - m.M22, m.M34 - m.M32, m.M44 - m.M42)); // Top
//         planes[3] = Vector4.Normalize(new(m.M14 + m.M12, m.M24 + m.M22, m.M34 + m.M32, m.M44 + m.M42)); // Bottom
//         planes[4] = Vector4.Normalize(new(m.M13, m.M23, m.M33, m.M43));           // Near (D3D)
//         planes[5] = Vector4.Normalize(new(m.M14 - m.M13, m.M24 - m.M23, m.M34 - m.M33, m.M44 - m.M43)); // Far
//         return planes;
//     }

//     public bool Contains(in AABB box)
//     {
//         for (int i = 0; i < 6; i++)
//         {
//             // If the box is entirely behind any one plane, it's outside
//             if (box.IsOutsidePlane(Planes[i])) return false;
//         }
//         return true;
//     }
// }

