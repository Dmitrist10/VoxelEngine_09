using System.Numerics;

namespace VoxelEngine.Input;

/* pls dont delte this!
    Vector2 MousePosition { get; }
    Vector2 MouseDelta { get; }
    Vector2 MouseScrollDelta { get; }

    bool IsMouseScrollingThisFrame();

    void IsKeyPressedThisFrame(Key key);
    void IsKeyPressed(Key key);
    void IsKeyDown(Key key);
    void IsKeyUp(Key key);

    void IsMouseButtonPressedThisFrame(MouseButton button);
    void IsMouseButtonPressed(MouseButton button);
    void IsMouseButtonDown(MouseButton button);
    void IsMouseButtonUp(MouseButton button);

    float GetMappingAxis(string axisName);
    bool GetMapping(string axisName);

    void RegisterKeyMapping(string axisName, Key positiveKey, Key negativeKey);
    void RegisterKeyMapping(string axisName, Key key);
 */

/// <summary>
/// Main input management context. Prodives access to specific device groups and handles action-based inputs.
/// </summary>
public interface IInputContext
{
    // // --- Device Access ---
    // IReadOnlyList<IKeyboard> Keyboards { get; }
    // IReadOnlyList<IMouse> Mice { get; }
    // // IReadOnlyList<IGamepad> Gamepads { get; }
    // // IReadOnlyList<ITouchscreen> Touchscreens { get; }
    // // IReadOnlyList<IVRController> VRControllers { get; }


    // IKeyboard? PrimaryKeyboard => Keyboards.FirstOrDefault();
    // IMouse? PrimaryMouse => Mice.FirstOrDefault();
    // // IGamepad? PrimaryGamepad => Gamepads.FirstOrDefault();
    // // ITouchscreen? PrimaryTouchscreen => Touchscreens.FirstOrDefault();
    // // IVRController? PrimaryVRController => VRControllers.FirstOrDefault();

    // bool IsKeyDown(Key key);
    // bool IsKeyPressed(Key key);
    // bool IsKeyReleased(Key key);

    // Vector2 MousePosition { get; }
    // Vector2 MouseDelta { get; }
    // Vector2 MouseScrollDelta { get; }

    // bool IsMouseButtonPressed(MouseButton button);
    // bool IsMouseButtonDown(MouseButton button);
    // bool IsMouseButtonReleased(MouseButton button);

    // // --- Action-Based Input (Easier to extend for VR, Mobile, etc.) ---

    // /// <summary> Checks if an action (e.g., "Jump") is currently down (held). </summary>
    // bool GetAction(string actionName);

    // /// <summary> Checks if an action was started precisely this frame. </summary>
    // bool GetActionDown(string actionName);

    // /// <summary> Checks if an action was released precisely this frame. </summary>
    // bool GetActionUp(string actionName);

    // /// <summary> Gets the value of an axis action (e.g., "MoveForward", "LookHorizontal"). </summary>
    // float GetAxis(string actionName);

    // /// <summary> Gets a 2D vector for composite actions like "Movement" or "Look". </summary>
    // Vector2 GetVector(string actionName);

    // // --- Mapping Configuration ---
    // void RegisterActionMapping(string actionName, Key key);
    // void RegisterActionMapping(string actionName, MouseButton button);
    // // void RegisterActionMapping(string actionName, GamepadButton button);
    // void RegisterAxisMapping(string actionName, Key positiveKey, Key negativeKey);
    // // void RegisterAxisMapping(string actionName, GamepadAxis axis);
}

public interface IActionMapping : IInputMapping
{
    Key key { get; set; }
    // Action? On
}
public interface IMouseActionMapping : IInputMapping
{
    MouseButton button { get; set; }
    // Action? On
}

public interface IAxisMapping : IInputMapping
{
    Key Posetive { get; set; }
    Key Negative { get; set; }

}

public interface IInputMapping
{
    // string Name { get; set; }
    string Name { get; }
}


public interface IKeyboard : IInputDevice
{
    bool IsKeyDown(Key key);
    bool IsKeyPressed(Key key);
    bool IsKeyReleased(Key key);
}

public interface IMouse : IInputDevice
{
    Vector2 Position { get; }
    Vector2 Delta { get; }
    Vector2 ScrollDelta { get; }

    bool IsButtonPressed(MouseButton button);
    bool IsButtonDown(MouseButton button);
    bool IsButtonReleased(MouseButton button);
}

public interface IGamepad : IInputDevice
{
    bool IsButtonPressed(GamepadButton button);
    bool IsButtonDown(GamepadButton button);
    bool IsButtonReleased(GamepadButton button);

    float GetAxis(GamepadAxis axis);
    void SetVibration(float leftMotor, float rightMotor);
}

public interface ITouchscreen : IInputDevice
{
    int TouchCount { get; }
    Vector2 GetTouchPosition(int id);
    bool IsFingerDown(int id);
}

public interface IVRController : IInputDevice
{
    Vector3 Position { get; }
    Quaternion Rotation { get; }

    bool IsButtonPressed(GamepadButton button);
    float GetTriggerStrength();
    void SetHaptics(float intensity, float durationSeconds);
}
