namespace VoxelEngine.Input;

public interface IInputDevice
{
    string Name { get; }
    bool IsConnected { get; }
    InputDeviceType Type { get; }
}
