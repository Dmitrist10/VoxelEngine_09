using Silk.NET.Input;
using Silk.NET.Windowing;

using VoxelEngine.Core;
using VoxelEngine.Windowing;

using silkInputContext = Silk.NET.Input.IInputContext;

namespace VoxelEngine.Input.SilkNet;

public sealed class InputDriver : IInputDriver
{
    public IInputContext GetInputContext()
    {
        IWindow window = (IWindow)EngineContext.Get<IWindowSurface>().NativeWindow;
        silkInputContext input = window.CreateInput();
        EngineContext.Register<Silk.NET.Input.IInputContext>(input);
        
        return new InputContext(input);
    }
}