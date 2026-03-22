using System.ComponentModel.Design;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using VoxelEngine.Core;
using VoxelEngine.Core.Runtime;
using VoxelEngine.Rendering;
using VoxelEngine.Windowing;

namespace VoxelEngine.Graphics.OpenGL;

public sealed class GL_GuiRenderer : IGuiRenderer
{
    private readonly ImGuiController _imGuiController;

    public GL_GuiRenderer(GL gl, IWindowSurface windowSurface)
    {
        IWindow window = (IWindow)windowSurface.NativeWindow;
        Silk.NET.Input.IInputContext inputContext = EngineContext.Get<Silk.NET.Input.IInputContext>();

        _imGuiController = new ImGuiController(gl, window, inputContext);
    }

    public void Update(float deltaTime) => _imGuiController.Update(deltaTime);
    public void Render() => _imGuiController.Render();
    public void Dispose() => _imGuiController.Dispose();
}
