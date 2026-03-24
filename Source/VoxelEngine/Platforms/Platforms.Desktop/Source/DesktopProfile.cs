using VoxelEngine.Core;
using VoxelEngine.Common;
using VoxelEngine.Core.Runtime;
using VoxelEngine.Diagnostics;
using VoxelEngine.Windowing;
using VoxelEngine.Graphics.OpenGL;
using VoxelEngine.Input.SilkNet;

namespace VoxelEngine.Platforms.Desktop;

public sealed class DesktopProfile : IEngineProfile
{

    public void Initialize()
    {
#if AOT
        // Register Silk.NET platforms for Native AOT compatibility
        Silk.NET.Windowing.Glfw.GlfwWindowing.RegisterPlatform();
        Silk.NET.Input.Glfw.GlfwInput.RegisterPlatform();
#else
        if (ArgumentsParser.HasArg("driver-glfw"))
        {
            Logger.Info("arg: '--drivers-glfw' -> Registering GLFW window & input backend...");
            Silk.NET.Windowing.Glfw.GlfwWindowing.RegisterPlatform();
            Silk.NET.Input.Glfw.GlfwInput.RegisterPlatform();
        }
#endif

    }

    public IEngineSubsystem[] CreateEngineSubSystems()
    {
        return new IEngineSubsystem[]
        {
            new GL_GraphicsDriver(),
            new InputDriver(),
        };
    }

    public IWindowSurface CreateWindowSurface(string title, int width, int height)
    {
        return new DesktopWindow(title, width, height);
    }




}
