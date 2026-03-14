namespace VoxelEngine.Common;

public interface IUpdateCallbacksHandler : IDisposable
{
    void OnInitialize();

    void OnUpdate();
    void OnFixedUpdate();
    void OnTick();
    void OnRender();
}

public interface IUpdatable { void OnUpdate(); }
public interface IFixedUpdatable { void OnFixedUpdate(); }
public interface IRenderable { void OnRender(); }
public interface ITickable { void OnTick(); }

// public interface IUpdate
// {
//     void OnUpdate();
// }

// public interface IFixedUpdate
// {
//     void OnFixedUpdate();
// }

// public interface ITick
// {
//     void OnTick();
// }

// public interface IRender
// {
//     void OnRender();
// }
