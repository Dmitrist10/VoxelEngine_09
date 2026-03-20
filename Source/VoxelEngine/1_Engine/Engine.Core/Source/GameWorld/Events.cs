namespace VoxelEngine.Core;

public record Event_OnSceneCreated(Scene scene);
public record Event_OnSceneDestroyed(Scene scene);

public record Event_OnUniverseCreated(Universe universe);
public record Event_OnUniverseDestroyed(Universe universe);
