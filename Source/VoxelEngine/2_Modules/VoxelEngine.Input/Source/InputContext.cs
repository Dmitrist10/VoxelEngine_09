using System.Numerics;
using Silk.NET.Input;
using System.Collections.Generic;

using silkInputContext = Silk.NET.Input.IInputContext;
using silksNativeKeyboard = Silk.NET.Input.IKeyboard;
using silkMouse = Silk.NET.Input.IMouse;

namespace VoxelEngine.Input.SilkNet;

public sealed class InputContext : IInputContext
{
    private silkInputContext _silkContext;

    private List<IKeyboard> _keyboards = new();
    private List<IMouse> _mice = new();
    // private List<IGamepad> _gamepads = new();
    // private List<ITouchscreen> _touchscreens = new();
    // private List<IVRController> _vrControllers = new();

    public IReadOnlyList<IKeyboard> Keyboards => _keyboards;
    public IReadOnlyList<IMouse> Mice => _mice;

    private SilkKeyboard _keyboard;
    private SilkMouse _mouse;

    private Dictionary<string, IActionMapping> _keyboardActionMappings = new();
    private Dictionary<string, IMouseActionMapping> _mouseActionMappings = new();
    private Dictionary<string, IAxisMapping> _axisMappings = new();

    // public IReadOnlyList<IGamepad> Gamepads => _gamepads;
    // public IReadOnlyList<ITouchscreen> Touchscreens => _touchscreens;
    // public IReadOnlyList<IVRController> VRControllers => _vrControllers;

    public InputContext(silkInputContext silkContext)
    {
        _silkContext = silkContext;

        // Initialize devices from Silk.NET
        foreach (var k in silkContext.Keyboards) _keyboards.Add(new SilkKeyboard(k));
        foreach (var m in silkContext.Mice) _mice.Add(new SilkMouse(m));
        // foreach (var g in silkContext.Gamepads) _gamepads.Add(new SilkGamepad(g));

        _keyboard = (SilkKeyboard)_keyboards.FirstOrDefault()! ?? throw new Exception("No keyboard found");
        _mouse = (SilkMouse)_mice.FirstOrDefault()! ?? throw new Exception("No mouse found");
    }

    public Vector2 MousePosition => _mouse.Position;
    public Vector2 MouseDelta => _mouse.Delta;
    public Vector2 MouseScrollDelta => _mouse.ScrollDelta;

    public bool IsKeyDown(Key key) => _keyboard.IsKeyDown(key);
    public bool IsKeyPressed(Key key) => _keyboard.IsKeyPressed(key);
    public bool IsKeyReleased(Key key) => _keyboard.IsKeyReleased(key);

    public bool IsMouseButtonPressed(MouseButton button) => _mouse.IsButtonPressed(button);
    public bool IsMouseButtonDown(MouseButton button) => _mouse.IsButtonDown(button);
    public bool IsMouseButtonReleased(MouseButton button) => _mouse.IsButtonReleased(button);

    // --- Action Implementations (Stubs for now) ---
    public bool GetAction(string actionName)
    {
        if (_keyboardActionMappings.TryGetValue(actionName, out var mapping))
        {
            return _keyboard.IsKeyDown(mapping.key);
        }
        if (_mouseActionMappings.TryGetValue(actionName, out var mouseMapping))
        {
            return _mouse.IsButtonPressed(mouseMapping.button);
        }
        return false;
    }
    public bool GetActionDown(string actionName)
    {
        if (_keyboardActionMappings.TryGetValue(actionName, out var mapping))
        {
            return _keyboard.IsKeyDown(mapping.key);
        }
        if (_mouseActionMappings.TryGetValue(actionName, out var mouseMapping))
        {
            return _mouse.IsButtonDown(mouseMapping.button);
        }
        return false;
    }
    public bool GetActionUp(string actionName)
    {
        if (_keyboardActionMappings.TryGetValue(actionName, out var mapping))
        {
            return _keyboard.IsKeyReleased(mapping.key);
        }
        if (_mouseActionMappings.TryGetValue(actionName, out var mouseMapping))
        {
            return _mouse.IsButtonReleased(mouseMapping.button);
        }
        return false;
    }
    public float GetAxis(string actionName)
    {
        if (_axisMappings.TryGetValue(actionName, out var mapping))
        {
            return _keyboard.IsKeyDown(mapping.Posetive) ? 1f : _keyboard.IsKeyDown(mapping.Negative) ? -1f : 0f;
        }
        return 0f;
    }
    public Vector2 GetVector(string actionName)
    {
        if (_axisMappings.TryGetValue(actionName, out var mapping))
        {
            return new Vector2(_keyboard.IsKeyDown(mapping.Posetive) ? 1f : _keyboard.IsKeyDown(mapping.Negative) ? -1f : 0f, _keyboard.IsKeyDown(mapping.Posetive) ? 1f : _keyboard.IsKeyDown(mapping.Negative) ? -1f : 0f);
        }
        return Vector2.Zero;
    }

    public void RegisterActionMapping(string actionName, Key key)
    {
        _keyboardActionMappings.Add(actionName, new ActionMapping(actionName, key));
    }
    public void RegisterActionMapping(string actionName, MouseButton button)
    {
        _mouseActionMappings.Add(actionName, new MouseActionMapping(actionName, button));
    }
    public void RegisterAxisMapping(string actionName, Key positiveKey, Key negativeKey)
    {
        _axisMappings.Add(actionName, new AxisMapping(actionName, positiveKey, negativeKey));
    }

    public void Update()
    {
        foreach (var m in _mice)
        {
            if (m is SilkMouse sm) sm.Update();
        }
        foreach (var k in _keyboards)
        {
            if (k is SilkKeyboard sk) sk.Update();
        }
    }

}

public sealed class ActionMapping : IActionMapping
{
    public string Name { get; }
    public Key key { get; set; }

    public ActionMapping(string name, Key valuekey)
    {
        Name = name;
        key = valuekey;
    }
}
public sealed class MouseActionMapping : IMouseActionMapping
{
    public string Name { get; }
    public MouseButton button { get; set; }

    public MouseActionMapping(string name, MouseButton valuebutton)
    {
        Name = name;
        button = valuebutton;
    }
}
public sealed class AxisMapping : IAxisMapping
{
    public string Name { get; }
    public Key Posetive { get; set; }
    public Key Negative { get; set; }

    public AxisMapping(string name, Key posetiveKey, Key negativeKey)
    {
        Name = name;
        Posetive = posetiveKey;
        Negative = negativeKey;
    }
}

// Internal wrapper classes for Silk.NET devices
internal class SilkKeyboard : IKeyboard
{
    private readonly silksNativeKeyboard _keyboard;
    private readonly HashSet<Silk.NET.Input.Key> _currentlyDown = new();
    private readonly HashSet<Silk.NET.Input.Key> _previouslyDown = new();
    private readonly HashSet<Silk.NET.Input.Key> _pendingDown = new();
    private readonly HashSet<Silk.NET.Input.Key> _pendingUp = new();

    public SilkKeyboard(silksNativeKeyboard keyboard)
    {
        _keyboard = keyboard;
        _keyboard.KeyDown += (k, key, i) => _pendingDown.Add(key);
        _keyboard.KeyUp += (k, key, i) => _pendingUp.Add(key);
        
        // Initialize state
        foreach (var key in _keyboard.SupportedKeys ?? Array.Empty<Silk.NET.Input.Key>())
        {
            if (_keyboard.IsKeyPressed(key)) _currentlyDown.Add(key);
        }
    }

    public void Update()
    {
        _previouslyDown.Clear();
        foreach (var key in _currentlyDown) _previouslyDown.Add(key);

        foreach (var key in _pendingDown) _currentlyDown.Add(key);
        foreach (var key in _pendingUp) _currentlyDown.Remove(key);
        _pendingDown.Clear();
        _pendingUp.Clear();
    }

    public string Name => _keyboard.Name;
    public bool IsConnected => _keyboard.IsConnected;
    public InputDeviceType Type => InputDeviceType.Keyboard;

    // IsKeyDown uses the native silk polling method for maximum reliability
    public bool IsKeyDown(Key key) => _keyboard.IsKeyPressed((Silk.NET.Input.Key)key);
    public bool IsKeyPressed(Key key) => IsKeyDown(key) && !_previouslyDown.Contains((Silk.NET.Input.Key)key);
    public bool IsKeyReleased(Key key) => !IsKeyDown(key) && _previouslyDown.Contains((Silk.NET.Input.Key)key);
}

internal class SilkMouse : IMouse
{
    private readonly silkMouse _mouse;
    private Vector2 _lastPosition;
    private Vector2 _delta;
    private readonly HashSet<Silk.NET.Input.MouseButton> _currentlyDown = new();
    private readonly HashSet<Silk.NET.Input.MouseButton> _previouslyDown = new();
    private readonly HashSet<Silk.NET.Input.MouseButton> _pendingDown = new();
    private readonly HashSet<Silk.NET.Input.MouseButton> _pendingUp = new();

    public SilkMouse(silkMouse mouse)
    {
        _mouse = mouse;
        _lastPosition = mouse.Position;
        _mouse.MouseDown += (m, btn) => _pendingDown.Add(btn);
        _mouse.MouseUp += (m, btn) => _pendingUp.Add(btn);
        
        // Initialize state
        for (int i = 0; i < 12; i++)
        {
            var btn = (Silk.NET.Input.MouseButton)i;
            if (_mouse.IsButtonPressed(btn)) _currentlyDown.Add(btn);
        }
    }

    public void Update()
    {
        Vector2 currentPosition = _mouse.Position;
        _delta = currentPosition - _lastPosition;
        _lastPosition = currentPosition;

        _previouslyDown.Clear();
        foreach (var btn in _currentlyDown) _previouslyDown.Add(btn);

        foreach (var btn in _pendingDown) _currentlyDown.Add(btn);
        foreach (var btn in _pendingUp) _currentlyDown.Remove(btn);
        _pendingDown.Clear();
        _pendingUp.Clear();
    }

    public string Name => _mouse.Name;
    public bool IsConnected => _mouse.IsConnected;
    public InputDeviceType Type => InputDeviceType.Mouse;

    public Vector2 Position => _mouse.Position;
    public Vector2 Delta => _delta;
    public Vector2 ScrollDelta => new Vector2(_mouse.ScrollWheels.FirstOrDefault().X, _mouse.ScrollWheels.FirstOrDefault().Y);

    public bool IsButtonPressed(MouseButton button) => IsButtonDown(button) && !_previouslyDown.Contains((Silk.NET.Input.MouseButton)button);
    public bool IsButtonDown(MouseButton button) => _mouse.IsButtonPressed((Silk.NET.Input.MouseButton)button);
    public bool IsButtonReleased(MouseButton button) => !IsButtonDown(button) && _previouslyDown.Contains((Silk.NET.Input.MouseButton)button);
}

internal class SilkGamepad : VoxelEngine.Input.IGamepad
{
    private readonly Silk.NET.Input.IGamepad _gamepad;
    public SilkGamepad(Silk.NET.Input.IGamepad gamepad) => _gamepad = gamepad;

    public string Name => _gamepad.Name;
    public bool IsConnected => _gamepad.IsConnected;
    public InputDeviceType Type => InputDeviceType.Gamepad;

    public bool IsButtonPressed(GamepadButton button) => _gamepad.Buttons.Any(b => b.Name.ToString() == button.ToString() && b.Pressed);
    public bool IsButtonDown(GamepadButton button) => _gamepad.Buttons.Any(b => b.Name.ToString() == button.ToString() && b.Pressed);
    public bool IsButtonReleased(GamepadButton button) => false;

    public float GetAxis(GamepadAxis axis) => 0f;
    public void SetVibration(float leftMotor, float rightMotor) { }
}
