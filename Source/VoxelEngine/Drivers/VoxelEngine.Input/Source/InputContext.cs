using Silk.NET.Input;
using Silk.NET.Windowing;

namespace VoxelEngine.Input.SilkNet;

internal class InputContext : IInputContext, IDisposable
{
    private readonly Silk.NET.Input.IInputContext _inputContext;

    public InputContext(IWindow window)
    {
        _inputContext = window.CreateInput();
    }

    public void Update()
    {
        // _inputContext.Keyboards[0].BeginInput();
    }


    public void Dispose()
    {
        _inputContext.Dispose();
    }

}