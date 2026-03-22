using VoxelEngine.Core;
using ImGuiNET;
using VoxelEngine.Common;
using Arch.Core;
using System.Numerics;
using System;
using System.Reflection;
using System.Collections.Generic;
using VoxelEngine.Assets;
using VoxelEngine.Graphics;
using VoxelEngine.Rendering;
using VoxelEngine.Input;

namespace TestingGame;

public record struct C_EditorCamera();


public class EP_DebugUI : EntityProcessor, IRenderable, IUpdatable
{
    private Entity _selectedEntity = Entity.Null;
    private Dictionary<int, Vector3> _eulerCache = new();

    private IInputContext input;

    private float _hierarchyWidth = 350f;
    private float _inspectorWidth = 350f;

    private bool isUIEnabled = true;

    public EP_DebugUI()
    {
        input = EngineContext.Get<IInputContext>();
    }

    public override void OnInitialize()
    {
        Group = ProcessorGroup.Engine;
        ImGui.StyleColorsDark();
    }


    public void OnUpdate()
    {
        if (input.IsKeyDown(Key.F5))
        {
            isUIEnabled = !isUIEnabled;
        }

        if (input.IsKeyDown(Key.F) && _selectedEntity != Entity.Null)
        {
            world.Query(new QueryDescription().WithAll<C_EditorCamera, C_Transform>(), (Entity e, ref C_EditorCamera c, ref C_Transform t) =>
            {
                ref var targetT = ref world.Get<C_Transform>(_selectedEntity);
                t.WorldPosition = targetT.WorldPosition + -targetT.Forward * 2;
            });
        }

    }

    public void OnRender()
    {
        if (!isUIEnabled) return;

        var io = ImGui.GetIO();
        float screenW = io.DisplaySize.X;
        float screenH = io.DisplaySize.Y;

        // --- Debug overlay (freely floating) ---
        if (ImGui.Begin("Debug Window", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Text($"FPS: {Time.FPS:0.0}");
            ImGui.Text($"Frame (Unscaled): {Time.RawDeltaTime * 1000:0.00} ms");
            ImGui.Text($"Total Engine Time: {Time.TotalTime:0.0} s");
            ImGui.Separator();
            ImGui.Text($"Entities: {world.CountEntities(QueryDescription.Null)}");
        }
        ImGui.End();

        DrawHierarchy(screenW, screenH);
        DrawInspector(screenW, screenH);
    }

    // ──────────────────────────── HIERARCHY ────────────────────────────

    private void DrawHierarchy(float screenW, float screenH)
    {
        ImGui.SetNextWindowPos(new Vector2(0, 0), ImGuiCond.Always);
        ImGui.SetNextWindowSizeConstraints(new Vector2(100, screenH), new Vector2(screenW / 4f, screenH));
        ImGui.SetNextWindowSize(new Vector2(_hierarchyWidth, screenH), ImGuiCond.FirstUseEver);

        if (ImGui.Begin("Hierarchy", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse))
        {
            _hierarchyWidth = ImGui.GetWindowWidth();

            if (ImGui.BeginTabBar("HierarchyTabs"))
            {
                if (ImGui.BeginTabItem("Actors"))
                {
                    if (ImGui.Button("Add Cube Actor"))
                    {
                        var assets = EngineContext.Get<IAssetsManager>();
                        var mat = assets.LoadPBRMaterial(@"resources://Shaders/PBRShader.glsl");
                        Actor na = scene.CreateActor();
                        na.Name = $"Cube {na.ID.ToString().Substring(0, 4)}";
                        C_Mesh mesh = new C_Mesh { Mesh = assets.LoadCube(), Material = mat };
                        na.AddComponent(mesh);
                    }
                    ImGui.Separator();

                    var q = new QueryDescription().WithAll<C_Actor, C_Hierarchy>();
                    world.Query(in q, (Entity e, ref C_Actor _, ref C_Hierarchy h) =>
                    {
                        if (h.IsRoot) DrawActorNode(new Actor(e, scene));
                    });
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("World"))
                {
                    var lighting = EngineContext.Get<LightingService>();

                    ImGui.TextColored(new Vector4(1, 1, 0, 1), "Sun Settings");

                    Vector3 dir = lighting.Direction;
                    if (ImGui.DragFloat3("Sun Direction", ref dir, 0.01f))
                        lighting.Direction = Vector3.Normalize(dir);

                    Vector3 lightCol = lighting.Color;
                    if (ImGui.ColorEdit3("Sun Color", ref lightCol))
                        lighting.Color = lightCol;

                    float ambient = lighting.AmbientIntensity;
                    if (ImGui.DragFloat("Ambient Intensity", ref ambient, 0.01f, 0f, 3f))
                        lighting.AmbientIntensity = ambient;

                    ImGui.Separator();
                    ImGui.TextColored(new Vector4(0.5f, 0.5f, 1, 1), "Surface Settings");

                    float spec = lighting.SpecularIntensity;
                    if (ImGui.DragFloat("Specular Intensity", ref spec, 0.01f, 0f, 1f))
                        lighting.SpecularIntensity = spec;

                    float gloss = lighting.Shininess;
                    if (ImGui.DragFloat("Shininess", ref gloss, 1f, 1f, 256f))
                        lighting.Shininess = gloss;

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Game"))
                {
                    ImGui.TextColored(new Vector4(1, 1, 0, 1), "Game Settings");

                    if (EngineContext.IsPlaying)
                    {
                        if (ImGui.Button("Stop Game"))
                        {
                            EngineContext.IsPlaying = false;
                            EngineContext.ActiveGroups = ProcessorGroup.Engine | ProcessorGroup.Editor;
                        }
                    }
                    else
                    {
                        if (ImGui.Button("Start Game"))
                        {
                            EngineContext.IsPlaying = true;
                            EngineContext.ActiveGroups = ProcessorGroup.Engine | ProcessorGroup.Game;
                        }
                    }

                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
        ImGui.End();
    }

    private void DrawActorNode(Actor actor)
    {
        string name = string.IsNullOrEmpty(actor.Name)
            ? $"Actor {actor.entity.Id}" : actor.Name;

        var nodeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick
                      | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.DefaultOpen;
        if (actor.ChildCount == 0) nodeFlags |= ImGuiTreeNodeFlags.Leaf;
        if (_selectedEntity == actor.entity) nodeFlags |= ImGuiTreeNodeFlags.Selected;

        bool open = ImGui.TreeNodeEx($"{name}##{actor.entity.Id}", nodeFlags);

        if (ImGui.IsItemClicked())
        {
            if (_selectedEntity != actor.entity)
            {
                _selectedEntity = actor.entity;
                _eulerCache.Remove(actor.entity.Id);
            }
        }

        // Drag source
        if (ImGui.BeginDragDropSource())
        {
            int id = actor.entity.Id;
            unsafe { ImGui.SetDragDropPayload("ACTOR_DND", (IntPtr)(&id), sizeof(int)); }
            ImGui.Text($"Move: {name}");
            ImGui.EndDragDropSource();
        }

        // Drop target → re-parent
        if (ImGui.BeginDragDropTarget())
        {
            unsafe
            {
                var payload = ImGui.AcceptDragDropPayload("ACTOR_DND");
                if (payload.NativePtr != null)
                {
                    int droppedId = *(int*)payload.Data;
                    Actor dropped = Actor.Null;
                    world.Query(new QueryDescription().WithAll<C_Actor>(), (Entity se) =>
                    {
                        if (se.Id == droppedId) dropped = new Actor(se, scene);
                    });
                    if (dropped.IsValid && dropped != actor)
                        dropped.Parent = actor;
                }
            }
            ImGui.EndDragDropTarget();
        }

        if (open)
        {
            foreach (var child in actor.GetChildren()) DrawActorNode(child);
            ImGui.TreePop();
        }
    }

    // ──────────────────────────── INSPECTOR ────────────────────────────

    private void DrawInspector(float screenW, float screenH)
    {
        ImGui.SetNextWindowPos(new Vector2(screenW - _inspectorWidth, 0), ImGuiCond.Always);
        ImGui.SetNextWindowSizeConstraints(new Vector2(100, screenH), new Vector2(screenW / 4f, screenH));
        ImGui.SetNextWindowSize(new Vector2(_inspectorWidth, screenH), ImGuiCond.FirstUseEver);

        if (ImGui.Begin("Inspector", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse))
        {
            _inspectorWidth = ImGui.GetWindowWidth();

            if (_selectedEntity == Entity.Null || !world.IsAlive(_selectedEntity))
            {
                ImGui.Text("No actor selected.");
                ImGui.End();
                return;
            }

            Actor actor = new Actor(_selectedEntity, scene);

            // Name
            string name = actor.Name ?? "";
            if (ImGui.InputText("Name", ref name, 128)) actor.Name = name;
            ImGui.Separator();

            DrawTransform(actor);
            DrawCamera(actor);
            DrawMeshRenderer(actor);
            DrawInspectableComponents(actor);
        }
        ImGui.End();
    }

    // ──────────── Built-in component sections ────────────

    private void DrawTransform(Actor actor)
    {
        if (!world.Has<C_Transform>(_selectedEntity)) return;
        if (!ImGui.CollapsingHeader("Transform", ImGuiTreeNodeFlags.DefaultOpen)) return;

        Vector3 pos = actor.LocalPosition;
        if (ImGui.DragFloat3("Position", ref pos, 0.1f))
            actor.LocalPosition = pos;

        // Cached Euler angles — prevents gimbal feedback each frame
        if (!_eulerCache.TryGetValue(_selectedEntity.Id, out Vector3 rot))
        {
            rot = QuaternionToEuler(actor.LocalRotation);
            _eulerCache[_selectedEntity.Id] = rot;
        }
        if (ImGui.DragFloat3("Rotation", ref rot, 1.0f))
        {
            _eulerCache[_selectedEntity.Id] = rot;
            actor.LocalRotation = EulerToQuaternion(rot);
        }

        Vector3 scale = actor.LocalScale;
        if (ImGui.DragFloat3("Scale", ref scale, 0.1f))
            actor.LocalScale = scale;
    }

    private void DrawCamera(Actor actor)
    {
        if (!world.Has<C_Camera>(_selectedEntity)) return;
        if (!ImGui.CollapsingHeader("Camera", ImGuiTreeNodeFlags.DefaultOpen)) return;

        ref C_Camera cam = ref world.Get<C_Camera>(_selectedEntity);

        float fov = cam.FieldOfView;
        if (ImGui.DragFloat("Field of View", ref fov, 0.1f)) cam.FieldOfView = fov;

        float orthoSize = cam.OrthographicSize;
        if (ImGui.DragFloat("Orthographic Size", ref orthoSize, 0.1f)) cam.OrthographicSize = orthoSize;

        float near = cam.NearPlane;
        if (ImGui.DragFloat("Near Clip", ref near, 0.1f)) cam.NearPlane = near;

        float far = cam.FarPlane;
        if (ImGui.DragFloat("Far Clip", ref far, 0.1f)) cam.FarPlane = far;

        int camTypeIdx = (int)cam.CameraType;
        string[] typeNames = Enum.GetNames(typeof(CameraProjectionType));
        if (ImGui.Combo("Projection", ref camTypeIdx, typeNames, typeNames.Length))
        {
            cam.CameraType = (CameraProjectionType)camTypeIdx;
            cam.UpdateProjection();
        }
    }

    private void DrawMeshRenderer(Actor actor)
    {
        if (!world.Has<C_Mesh>(_selectedEntity)) return;
        if (!ImGui.CollapsingHeader("Mesh Renderer", ImGuiTreeNodeFlags.DefaultOpen)) return;

        ref C_Mesh cm = ref world.Get<C_Mesh>(_selectedEntity);
        ImGui.Text($"Vertices: {cm.Mesh.VertexCount}   Indices: {cm.Mesh.IndexCount}");

        if (cm.Material is PBRMaterial pbr)
        {
            // Color is already 0-1 floats, cast directly to Vector4
            Vector4 col = (Vector4)pbr.Properties.Color;
            if (ImGui.ColorEdit4("Albedo Color", ref col))
            {
                // Implicit Color(Vector4) conversion preserves 0-1 range
                pbr.Properties.Color = (Color)col;
                pbr.ApplyChanges();
            }

            float ao = pbr.Properties.AO;
            if (ImGui.DragFloat("AO", ref ao, 0.01f, 0f, 1f))
            {
                pbr.Properties.AO = ao;
                pbr.ApplyChanges();
            }

            float metallic = pbr.Properties.Metallic;
            if (ImGui.DragFloat("Metallic", ref metallic, 0.01f, 0f, 1f))
            {
                pbr.Properties.Metallic = metallic;
                pbr.ApplyChanges();
            }

            float roughness = pbr.Properties.Roughness;
            if (ImGui.DragFloat("Roughness", ref roughness, 0.01f, 0f, 1f))
            {
                pbr.Properties.Roughness = roughness;
                pbr.ApplyChanges();
            }

            // float emissive = pbr.Properties.Emissive;
            // if (ImGui.DragFloat("Emissive", ref emissive, 0.01f, 0f, 1f))
            // {
            //     pbr.Properties.Emissive = emissive;
            //     pbr.ApplyChanges();
            // }
        }
        else
        {
            ImGui.TextDisabled($"Material: {cm.Material?.Name ?? "None"}");
        }
    }

    // ──────────── Generic [Inspect] reflection drawer ────────────

    /// <summary>
    /// Draws editable controls for every public field on a component struct that
    /// (a) has at least one [Inspect] attribute, OR
    /// (b) is a struct in an unknown assembly — editing all public fields.
    ///
    /// We use Arch's World.Get/Set via reflection so we never need to know the
    /// concrete type at compile time.
    /// </summary>
    private void DrawInspectableComponents(Actor actor)
    {
        // Use the entity's archetype to enumerate component types safely
        var archetype = world.GetArchetype(actor.entity);

        // GetType() info for all public generic methods on World
        var worldType = world.GetType();
        var getMethod = worldType.GetMethod("Get", 1, new[] { typeof(Entity) });
        var setMethod = worldType.GetMethod("Set", 2, new[] { typeof(Entity), Type.MakeGenericMethodParameter(0) });

        // Walk every component type in this archetype via its internal Types array
        // The Arch library exposes component types through the Archetype object
        // We use reflection on Archetype to avoid coupling to a specific property name
        object archetypeBox = archetype;
        ComponentType[] componentTypes = GetArchetypeComponentTypes(archetypeBox);

        foreach (var compType in componentTypes)
        {
            Type t = compType.Type;

            // Skip the ones we already handle explicitly
            if (t == typeof(C_Transform) || t == typeof(C_WorldTransformMatrix) ||
                t == typeof(C_Camera) || t == typeof(C_Actor) ||
                t == typeof(C_ID) || t == typeof(C_Hierarchy) ||
                t == typeof(C_Mesh))
                continue;

            // Collect fields to show: those marked [Inspect], or all public fields if none tagged
            var allPublicFields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var taggedFields = Array.FindAll(allPublicFields, f => f.IsDefined(typeof(InspectAttribute), true));
            var fieldsToShow = taggedFields.Length > 0 ? taggedFields : allPublicFields;

            if (fieldsToShow.Length == 0) continue;

            if (!ImGui.CollapsingHeader(t.Name, ImGuiTreeNodeFlags.DefaultOpen)) continue;

            // Box → edit → write back via Set<T>
            var getGeneric = getMethod?.MakeGenericMethod(t);
            var setGeneric = setMethod?.MakeGenericMethod(t);
            if (getGeneric == null) continue;

            object comp = getGeneric.Invoke(world, new object[] { actor.entity })!;
            bool changed = false;

            foreach (var field in fieldsToShow)
            {
                var attr = field.GetCustomAttribute<InspectAttribute>();
                string label = attr?.Label ?? field.Name;

                if (DrawField(label, field, ref comp))
                    changed = true;
            }

            if (changed && setGeneric != null)
                setGeneric.Invoke(world, new object[] { actor.entity, comp });
        }
    }

    /// <summary>Draws a single field control. Returns true if value was changed.</summary>
    private bool DrawField(string label, FieldInfo field, ref object comp)
    {
        Type ft = field.FieldType;

        if (ft == typeof(float))
        {
            float v = (float)field.GetValue(comp)!;
            if (!ImGui.DragFloat(label, ref v, 0.1f)) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(int))
        {
            int v = (int)field.GetValue(comp)!;
            if (!ImGui.DragInt(label, ref v)) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(bool))
        {
            bool v = (bool)field.GetValue(comp)!;
            if (!ImGui.Checkbox(label, ref v)) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(Vector2))
        {
            Vector2 v = (Vector2)field.GetValue(comp)!;
            if (!ImGui.DragFloat2(label, ref v, 0.1f)) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(Vector3))
        {
            Vector3 v = (Vector3)field.GetValue(comp)!;
            if (!ImGui.DragFloat3(label, ref v, 0.1f)) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(Vector4))
        {
            Vector4 v = (Vector4)field.GetValue(comp)!;
            if (!ImGui.DragFloat4(label, ref v, 0.1f)) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(Color))
        {
            // Color stores 0-1 floats — cast through Vector4
            Vector4 v = (Vector4)(Color)field.GetValue(comp)!;
            if (!ImGui.ColorEdit4(label, ref v)) return false;
            field.SetValue(comp, (Color)v); return true;
        }
        if (ft == typeof(string))
        {
            string v = (string?)field.GetValue(comp) ?? "";
            if (!ImGui.InputText(label, ref v, 256)) return false;
            field.SetValue(comp, v); return true;
        }

        // Unrecognised type: show a read-only label
        ImGui.TextDisabled($"{label}: <{ft.Name}>");
        return false;
    }

    // ──────────────────────────── Helpers ────────────────────────────

    /// <summary>Pulls ComponentType[] from an Arch Archetype without knowing the exact property name.</summary>
    private static ComponentType[] GetArchetypeComponentTypes(object archetype)
    {
        Type at = archetype.GetType();

        // Try common property names Arch uses across versions
        foreach (string candidate in new[] { "Types", "ComponentTypes", "Components" })
        {
            var prop = at.GetProperty(candidate, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null && prop.PropertyType == typeof(ComponentType[]))
                return (ComponentType[])prop.GetValue(archetype)!;

            var field = at.GetField(candidate, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(ComponentType[]))
                return (ComponentType[])field.GetValue(archetype)!;
        }
        return Array.Empty<ComponentType>();
    }

    private static Vector3 QuaternionToEuler(Quaternion q)
    {
        float sinr = 2 * (q.W * q.X + q.Y * q.Z);
        float cosr = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        float x = MathF.Atan2(sinr, cosr);

        float sinp = 2 * (q.W * q.Y - q.Z * q.X);
        float y = MathF.Abs(sinp) >= 1
            ? MathF.CopySign(MathF.PI / 2, sinp) : MathF.Asin(sinp);

        float siny = 2 * (q.W * q.Z + q.X * q.Y);
        float cosy = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        float z = MathF.Atan2(siny, cosy);

        return new Vector3(x, y, z) * (180f / MathF.PI);
    }

    private static Quaternion EulerToQuaternion(Vector3 deg)
    {
        Vector3 r = deg * (MathF.PI / 180f);
        return Quaternion.CreateFromYawPitchRoll(r.Y, r.X, r.Z);
    }


}
