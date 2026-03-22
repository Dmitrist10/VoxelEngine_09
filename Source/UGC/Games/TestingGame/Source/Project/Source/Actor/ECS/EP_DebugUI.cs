using ImGuiNET;
using Arch.Core;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

using VoxelEngine.Core;
using VoxelEngine.Common;
using VoxelEngine.Input;
using VoxelEngine.Assets;
using VoxelEngine.Graphics;
using VoxelEngine.Rendering;
using VoxelEngine.Diagnostics;

namespace TestingGame;

public record struct C_EditorCamera();
public record struct C_EditorOnly(); // Hides this entity from the hierarchy


public class EP_DebugUI : EntityProcessor, IRenderable, IUpdatable
{
    private Entity _selectedEntity = Entity.Null;
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
        if (input.IsKeyPressed(Key.F5))
        {
            isUIEnabled = !isUIEnabled;
        }

        if (input.IsKeyPressed(Key.F) && _selectedEntity != Entity.Null)
        {
            world.Query(new QueryDescription().WithAll<C_EditorCamera, C_Transform>(), (Entity e, ref C_EditorCamera c, ref C_Transform t) =>
            {
                ref var targetT = ref world.Get<C_Transform>(_selectedEntity);
                t.LocalPosition = targetT.WorldPosition + -targetT.Forward * 2;
                t.MarkDirty();
            });
        }

        // Duplicate
        if (input.IsKeyDown(Key.ControlLeft) && input.IsKeyPressed(Key.D))
        {
            if (_selectedEntity != Entity.Null && world.IsAlive(_selectedEntity))
            {
                DuplicateActor(new Actor(_selectedEntity, scene));
            }
        }

        UpdatePicking();
    }

    private void UpdatePicking()
    {
        var io = ImGui.GetIO();
        if (io.WantCaptureMouse) return;
        if (!input.IsMouseButtonPressed(MouseButton.Left)) return;

        // Find main camera
        C_Camera? mainCam = null;
        world.Query(new QueryDescription().WithAll<C_Camera, C_Transform>(), (ref C_Camera cam) =>
        {
            if (cam.IsMainCamera) mainCam = cam;
        });

        if (mainCam == null) return;

        Ray ray = GetMouseRay(mainCam.Value, io.MousePos, io.DisplaySize.X, io.DisplaySize.Y);

        Entity bestHit = Entity.Null;
        float minT = float.MaxValue;

        world.Query(new QueryDescription().WithAll<C_AABB, C_WorldTransformMatrix>(), (Entity e, ref C_AABB aabb, ref C_WorldTransformMatrix worldMatrix) =>
        {
            Matrix4x4.Invert(worldMatrix.WorldMatrix, out var invWorld);
            Vector3 localOrigin = Vector3.Transform(ray.Origin, invWorld);
            Vector3 localDir = Vector3.Normalize(Vector3.TransformNormal(ray.Direction, invWorld));
            Ray localRay = new Ray(localOrigin, localDir);

            if (aabb.LocalAABB.Intersects(localRay, out float t))
            {
                Vector3 worldHit = Vector3.Transform(localRay.GetPoint(t), worldMatrix.WorldMatrix);
                float dist = Vector3.Distance(ray.Origin, worldHit);

                if (dist < minT)
                {
                    minT = dist;
                    bestHit = e;
                }
            }
        });

        if (bestHit != Entity.Null)
            _selectedEntity = bestHit;
    }

    private Ray GetMouseRay(C_Camera cam, Vector2 mousePos, float sw, float sh)
    {
        float x = (2.0f * mousePos.X) / sw - 1.0f;
        float y = 1.0f - (2.0f * mousePos.Y) / sh;

        Matrix4x4.Invert(cam.View * cam.Projection, out var invVP);

        Vector4 near = Vector4.Transform(new Vector4(x, y, 0.0f, 1.0f), invVP);
        Vector4 far = Vector4.Transform(new Vector4(x, y, 1.0f, 1.0f), invVP);

        Vector3 rayOrigin = new Vector3(near.X / near.W, near.Y / near.W, near.Z / near.W);
        Vector3 rayEnd = new Vector3(far.X / far.W, far.Y / far.W, far.Z / far.W);
        Vector3 rayDir = Vector3.Normalize(rayEnd - rayOrigin);

        return new Ray(rayOrigin, rayDir);
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
                    if (ImGui.DragFloat("Ambient Intensity", ref ambient, 0.025f, 0f, 10f))
                        lighting.AmbientIntensity = ambient;

                    ImGui.Separator();
                    ImGui.TextColored(new Vector4(0.5f, 0.5f, 1, 1), "Surface Settings");

                    float spec = lighting.SpecularIntensity;
                    if (ImGui.DragFloat("Specular Intensity", ref spec, 0.01f, 0f, 2f))
                        lighting.SpecularIntensity = spec;

                    float gloss = lighting.Shininess;
                    if (ImGui.DragFloat("Shininess", ref gloss, 1f, 1f, 512f))
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
        if (actor.HasComponent<C_EditorOnly>()) return;

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

        // Context menu
        if (ImGui.BeginPopupContextItem($"ActorContext{actor.entity.Id}"))
        {
            if (ImGui.MenuItem("Duplicate")) DuplicateActor(actor);
            ImGui.EndPopup();
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

            // Name & Actions
            string name = actor.Name ?? "";
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Name");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 100);
            if (ImGui.InputText("##Name", ref name, 128)) actor.Name = name;
            ImGui.SameLine();
            if (ImGui.Button("Duplicate"))
            {
                DuplicateActor(actor);
            }
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
        if (LabeledControl("Position", () => ImGui.DragFloat3("##Position", ref pos, 0.1f), d => { pos += new Vector3(d * 0.1f); return true; }))
            actor.LocalPosition = pos;

        Quaternion rot = actor.LocalRotation;
        if (DrawRotationField("Rotation", ref rot))
            actor.LocalRotation = rot;

        Vector3 scale = actor.LocalScale;
        if (LabeledControl("Scale", () => ImGui.DragFloat3("##Scale", ref scale, 0.1f), d => { scale *= (1.0f + d * 0.01f); return true; }))
            actor.LocalScale = scale;
    }

    private bool DrawRotationField(string label, ref Quaternion q)
    {
        Vector3 euler = q.QuaternionToEuler();

        uint id = ImGui.GetID(label);
        var storage = ImGui.GetStateStorage();

        // If we were active in the previous frame, use the stored euler to stay stable
        if (storage.GetBool(id + 3))
        {
            euler.X = storage.GetFloat(id + 0);
            euler.Y = storage.GetFloat(id + 1);
            euler.Z = storage.GetFloat(id + 2);
        }

        bool changed = LabeledControl(label, () => ImGui.DragFloat3("##" + label, ref euler, 1.0f), d => { euler += new Vector3(d); return true; });
        if (changed)
        {
            q = euler.EulerToQuaternion();
            storage.SetFloat(id + 0, euler.X);
            storage.SetFloat(id + 1, euler.Y);
            storage.SetFloat(id + 2, euler.Z);
        }

        bool isActive = storage.GetBool(id + 3) || ImGui.IsItemActive(); // Approximation for LabeledControl interaction
                                                                         // Note: LabeledControl handles its own internal active state for the label, 
                                                                         // but we sync storage here for the stability logic.

        // Better: Sync isActive correctly if needed. For now, DragFloat3 activity is enough.

        storage.SetBool(id + 3, ImGui.IsItemActive());

        // Sync storage when not active to handle external changes
        if (!ImGui.IsItemActive() && !changed)
        {
            storage.SetFloat(id + 0, euler.X);
            storage.SetFloat(id + 1, euler.Y);
            storage.SetFloat(id + 2, euler.Z);
        }
        return changed;
    }

    private void DrawCamera(Actor actor)
    {
        if (!world.Has<C_Camera>(_selectedEntity)) return;
        if (!ImGui.CollapsingHeader("Camera", ImGuiTreeNodeFlags.DefaultOpen)) return;

        ref C_Camera cam = ref world.Get<C_Camera>(_selectedEntity);

        float fov = cam.FieldOfView;
        if (LabeledControl("Field of View", () => ImGui.DragFloat("##FOV", ref fov, 0.1f))) cam.FieldOfView = fov;

        float orthoSize = cam.OrthographicSize;
        if (LabeledControl("Orthographic Size", () => ImGui.DragFloat("##Ortho", ref orthoSize, 0.1f))) cam.OrthographicSize = orthoSize;

        float near = cam.NearPlane;
        if (LabeledControl("Near Clip", () => ImGui.DragFloat("##Near", ref near, 0.1f))) cam.NearPlane = near;

        float far = cam.FarPlane;
        if (LabeledControl("Far Clip", () => ImGui.DragFloat("##Far", ref far, 0.1f))) cam.FarPlane = far;

        int camTypeIdx = (int)cam.CameraType;
        string[] typeNames = Enum.GetNames(typeof(CameraProjectionType));
        if (LabeledControl("Projection", () => ImGui.Combo("##Projection", ref camTypeIdx, typeNames, typeNames.Length)))
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
            if (LabeledControl("AO", () => ImGui.DragFloat("##AO", ref ao, 0.01f, 0f, 1f)))
            {
                pbr.Properties.AO = ao;
                pbr.ApplyChanges();
            }

            float metallic = pbr.Properties.Metallic;
            if (LabeledControl("Metallic", () => ImGui.DragFloat("##Metallic", ref metallic, 0.01f, 0f, 1f)))
            {
                pbr.Properties.Metallic = metallic;
                pbr.ApplyChanges();
            }

            float roughness = pbr.Properties.Roughness;
            if (LabeledControl("Roughness", () => ImGui.DragFloat("##Roughness", ref roughness, 0.01f, 0f, 1f)))
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

    private void DrawInspectableComponents(Actor actor)
    {
        object[] components = world.GetAllComponents(actor.entity)!;

        foreach (var component in components)
        {
            var type = component.GetType();
            if (type.GetCustomAttribute<NoInspectAttribute>() != null) continue;
            // if (type == typeof(C_Transform) || type == typeof(C_WorldTransformMatrix) ||
            //             type == typeof(C_Camera) || type == typeof(C_Actor) ||
            //             type == typeof(C_ID) || type == typeof(C_Hierarchy) ||
            //             type == typeof(C_Mesh))
            //     continue;

            if (ImGui.CollapsingHeader(component.GetType().Name))
            {
                DrawComponentFields(type, component, actor.entity);
            }
        }

        void DrawComponentFields(Type type, object component, Entity entity)
        {
            ImGui.PushID(type.FullName);
            foreach (var field in type.GetFields())
            {
                if (field.GetCustomAttribute<NoInspectAttribute>() != null) continue;

                var onlyViewAttr = field.GetCustomAttribute<OnlyView>();
                var attr = field.GetCustomAttribute<InspectAttribute>();
                string label = attr?.Label ?? field.Name;

                if (DrawField(label, field, ref component, onlyViewAttr != null))
                {
                    world.Set(entity, component); // Push changes back to ECS
                }
            }
            ImGui.PopID();
        }
    }

    private bool DrawField(string label, FieldInfo field, ref object comp, bool onlyView)
    {
        Type ft = field.FieldType;

        if (ft == typeof(float))
        {
            float v = (float)field.GetValue(comp)!;
            if (!LabeledControl(label, () => ImGui.DragFloat("##" + label, ref v, 0.1f))) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(int))
        {
            int v = (int)field.GetValue(comp)!;
            if (!LabeledControl(label, () => ImGui.DragInt("##" + label, ref v))) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(bool))
        {
            bool v = (bool)field.GetValue(comp)!;
            if (!LabeledControl(label, () => ImGui.Checkbox("##" + label, ref v))) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(Vector2))
        {
            Vector2 v = (Vector2)field.GetValue(comp)!;
            if (!LabeledControl(label, () => ImGui.DragFloat2("##" + label, ref v, 0.1f), d => { v += new Vector2(d * 0.1f); return true; })) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(Vector3))
        {
            Vector3 v = (Vector3)field.GetValue(comp)!;
            float dragFactor = label.Contains("Scale") ? 0.01f : 0.1f;
            if (!LabeledControl(label, () => ImGui.DragFloat3("##" + label, ref v, 0.1f), d =>
            {
                if (label.Contains("Scale")) v *= (1.0f + d * dragFactor);
                else v += new Vector3(d * dragFactor);
                return true;
            })) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(Vector4))
        {
            Vector4 v = (Vector4)field.GetValue(comp)!;
            if (!LabeledControl(label, () => ImGui.DragFloat4("##" + label, ref v, 0.1f), d => { v += new Vector4(d * 0.1f); return true; })) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(Color))
        {
            // Color stores 0-1 floats — cast through Vector4
            Vector4 v = (Vector4)(Color)field.GetValue(comp)!;
            if (!LabeledControl(label, () => ImGui.ColorEdit4("##" + label, ref v))) return false;
            field.SetValue(comp, (Color)v); return true;
        }
        if (ft == typeof(string))
        {
            string v = (string?)field.GetValue(comp) ?? "";
            if (!LabeledControl(label, () => ImGui.InputText("##" + label, ref v, 256))) return false;
            field.SetValue(comp, v); return true;
        }
        if (ft == typeof(Quaternion))
        {
            Quaternion v = (Quaternion)field.GetValue(comp)!;
            if (DrawRotationField(label, ref v))
            {
                field.SetValue(comp, v);
                return true;
            }
            return false;
        }
        if (ft.IsEnum)
        {
            int val = Convert.ToInt32(field.GetValue(comp)!);
            string[] typeNames = Enum.GetNames(ft);
            if (LabeledControl(label, () => ImGui.Combo("##" + label, ref val, typeNames, typeNames.Length)))
            {
                field.SetValue(comp, Enum.ToObject(ft, val));
                return true;
            }
        }

        // Unrecognised type: show a read-only label
        ImGui.TextDisabled($"{label}: <{ft.Name}>");
        return false;
    }

    private bool LabeledControl(string label, Func<bool> drawControl, Func<float, bool>? onLabelDrag = null)
    {
        ImGui.Columns(2, "##" + label, false);
        ImGui.SetColumnWidth(0, 120);

        ImGui.AlignTextToFramePadding();

        // Use a Selectable for the label to make it interactable for dragging
        bool selected = false;
        ImGui.Selectable(label, ref selected, ImGuiSelectableFlags.None);

        bool labelChanged = false;
        if (onLabelDrag != null && ImGui.IsItemActive() && ImGui.IsMouseDragging(ImGuiMouseButton.Left))
        {
            float delta = ImGui.GetIO().MouseDelta.X;
            if (delta != 0)
            {
                labelChanged = onLabelDrag(delta);
            }
        }

        ImGui.NextColumn();
        ImGui.SetNextItemWidth(-1);
        bool controlChanged = drawControl();

        ImGui.Columns(1);
        return controlChanged || labelChanged;
    }

    private void DuplicateActor(Actor source)
    {
        if (!source.IsValid) return;

        // 1. Create a new actor with standard components
        Actor newActor = scene.CreateActor();
        newActor.Name = source.Name + " (Copy)";

        // 2. Copy all other components from source
        object?[] components = world.GetAllComponents(source.entity)!;
        foreach (var comp in components)
        {
            if (comp == null) continue;
            var type = comp.GetType();

            // Skip core components already handled by CreateActor or unique to an entity
            if (newActor.IsCoreComponent(type))
                continue;

            // Add the component (assuming world.Add has a boxed/generic overload available)
            world.Add(newActor.entity, comp);
        }

        // 3. Sync Transform
        newActor.LocalPosition = source.LocalPosition;
        newActor.LocalRotation = source.LocalRotation;
        newActor.LocalScale = source.LocalScale;

        // 4. Handle Parent
        if (source.Parent != Actor.Null)
        {
            newActor.SetParent(source.Parent.entity, false);
        }

        _selectedEntity = newActor.entity;
    }

}
