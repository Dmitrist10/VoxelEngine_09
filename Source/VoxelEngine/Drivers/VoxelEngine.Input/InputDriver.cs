using Silk.NET.Windowing;
using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Input;
using VoxelEngine.Windowing;

namespace VoxelEngine.Input.SilkNet;

public class InputDriver : IInputDriver
{
    private readonly InputContext _inputContext;

    public InputDriver()
    {
        IWindow window = EngineContext.Get<IWindow>();
        _inputContext = new InputContext(window);

        EngineContext.Register<IInputContext>(_inputContext);
    }

    public void OnInitialize()
    {
    }
    public void OnRender()
    {
    }
    public void OnFixedUpdate()
    {
    }
    public void OnTick()
    {
    }
    public void OnUpdate()
    {
        _inputContext.Update();
    }


    public void Dispose()
    {
    }

}
