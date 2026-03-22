namespace VoxelEngine.Rendering;

/// <summary>
/// Defines exactly WHEN a pass should be injected into the renderer timeline.
/// </summary>
public enum RenderPassEvent
{
    BeforeShadows = 0,
    AfterShadows = 100,
    
    BeforeOpaque = 200,
    AfterOpaque = 300,
    
    BeforeTransparent = 400,
    AfterTransparent = 500,
    
    BeforePostProcess = 600,
    AfterPostProcess = 700,
    
    BeforeUI = 800,
    AfterUI = 900
}

public interface IRenderPass
{
    string Name { get; }
    
    bool IsActive { get; set; }
    
    /// <summary>
    /// Tells the Renderer where to inject this pass in the timeline.
    /// User generated passes can use this to easily run before or after engine passes
    /// without messy manual sorting!
    /// </summary>
    RenderPassEvent InjectionPoint { get; }

    void Execute(RenderContext context);
}
