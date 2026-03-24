// using System.Numerics;
// using Arch.Core;
// using Silk.NET.Input;
// using Silk.NET.Windowing;
// using VoxelEngine.Common;
// using VoxelEngine.Core;
// using VoxelEngine.Windowing;
// using VoxelEngine.Diagnostics;

// namespace TestingGame;

// public sealed class EP_FlyCamera : EntityProcessor, IUpdatable
// {
//     private IInputContext _input;
//     private IKeyboard _keyboard;
//     private IMouse _mouse;

//     private float _moveSpeed = 5.0f;
//     private float _lookSensitivity = 0.1f;

//     private Vector2 _lastMousePos;
//     private Vector3 _rotation; // pitch, yaw, roll

//     public override void OnInitialize()
//     {
//     }

//     private void EnsureInput()
//     {
//         if (_input != null) return;

//         try
//         {
//             _input = EngineContext.Get<IInputContext>();
//             if (_input.Keyboards.Count > 0) _keyboard = _input.Keyboards[0];
//             if (_input.Mice.Count > 0) _mouse = _input.Mice[0];
//         }
//         catch (Exception ex)
//         {
//             Logger.Error($"Failed to initialize FlyCamera input: {ex.Message}");
//         }
//     }

//     public override void OnShutDown()
//     {
//         _input?.Dispose();
//     }

//     public void OnUpdate()
//     {
//         EnsureInput();
//         if (_keyboard == null || _mouse == null) return;

//         float dt = Time.DeltaTime;

//         world.Query(new QueryDescription().WithAll<C_Transform, C_Camera>(), (ref C_Transform transform, ref C_Camera camera) =>
//         {
//             // Only move if it's the main camera
//             if (!camera.IsMainCamera) return;

//             bool moved = false;

//             // Rotation
//             var mousePos = _mouse.Position;
//             if (_mouse.IsButtonPressed(MouseButton.Right))
//             {
//                 var delta = mousePos - _lastMousePos;
//                 _rotation.Y -= delta.X * _lookSensitivity; // Yaw
//                 _rotation.X -= delta.Y * _lookSensitivity; // Pitch
//                 _rotation.X = Math.Clamp(_rotation.X, -89f, 89f);

//                 transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(
//                     _rotation.Y * (float)Math.PI / 180f,
//                     _rotation.X * (float)Math.PI / 180f,
//                     0);
//                 moved = true;
//             }
//             _lastMousePos = mousePos;

//             // Movement
//             Vector3 movement = Vector3.Zero;
//             if (_keyboard.IsKeyPressed(Key.W)) movement += transform.Forward;
//             if (_keyboard.IsKeyPressed(Key.S)) movement -= transform.Forward;
//             if (_keyboard.IsKeyPressed(Key.A)) movement -= transform.Right;
//             if (_keyboard.IsKeyPressed(Key.D)) movement += transform.Right;
//             if (_keyboard.IsKeyPressed(Key.E)) movement += Vector3.UnitY;
//             if (_keyboard.IsKeyPressed(Key.Q)) movement -= Vector3.UnitY;

//             if (movement != Vector3.Zero)
//             {
//                 transform.LocalPosition += Vector3.Normalize(movement) * _moveSpeed * dt;
//                 moved = true;
//             }

//             if (moved) transform.IsDirty = true;
//         });
//     }
// }
