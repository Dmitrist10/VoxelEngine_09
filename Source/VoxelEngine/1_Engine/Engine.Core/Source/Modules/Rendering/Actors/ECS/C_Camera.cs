using System.Numerics;

namespace VoxelEngine.Core;

public enum CameraProjectionType
{
    Perspective,
    Orthographic
}

public struct C_Camera : IComponent
{
    [Inspect] public Matrix4x4 View { get; private set; }
    [NoInspect] public Matrix4x4 Projection { get; private set; }
    [Inspect] public bool IsMainCamera { get; internal set; }
    [Inspect] public int Priority { get; internal set; }

    [Inspect] public CameraProjectionType CameraType;

    [Inspect] public float OrthographicSize;
    [Inspect] public float FieldOfView;

    [OnlyView] public float AspectRatio;
    [Inspect] public float NearPlane;
    [Inspect] public float FarPlane;

    public C_Camera(CameraProjectionType type = CameraProjectionType.Perspective, float fieldOfView = 0.8f, float aspectRatio = 16f / 9f,
        float nearPlane = 0.1f, float farPlane = 1000f, float orthographicSize = 8f, bool isMainCamera = false,
        int Priority = 0)
    {
        CameraType = type;
        FieldOfView = fieldOfView;
        AspectRatio = aspectRatio;
        NearPlane = nearPlane;
        FarPlane = farPlane;
        OrthographicSize = orthographicSize;
        IsMainCamera = isMainCamera;
        this.Priority = Priority;
        UpdateProjection();
    }

    public void UpdateProjection()
    {
        switch (CameraType)
        {
            case CameraProjectionType.Perspective:
                if (FieldOfView <= 0) FieldOfView = 0.75f;
                Projection = Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
                break;
            case CameraProjectionType.Orthographic:
                Projection = Matrix4x4.CreateOrthographic(OrthographicSize * AspectRatio, OrthographicSize, NearPlane,
                    FarPlane);
                break;
        }
    }

    public void UpdateView(Vector3 cameraPos, Vector3 cameraTarget, Vector3 cameraUp)
    {
        // View = Matrix4x4.CreateLookAt(transform.Position, transform.Position + Transform.Forward, Transform.Up);
        View = Matrix4x4.CreateLookAt(cameraPos, cameraTarget, cameraUp);
    }

    public void UpdateAspectRatio(float aspectRatio)
    {
        AspectRatio = aspectRatio;
        UpdateProjection();
    }
}