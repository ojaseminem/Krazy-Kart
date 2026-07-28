using System.Collections.Generic;
using Gameplay;
using UnityEditor;
using UnityEngine;

namespace KrazyKartEditor
{
    /// Generates the whole mall as one stacked vertical space.
    ///
    /// Layout, bottom to top: every floor is a hall open along its +X edge onto a ramp shaft.
    /// The player arrives on a landing at the shaft's top, drives inward onto the slab, crosses
    /// the floor wrecking everything, then re-enters the shaft at its low end and climbs to the
    /// next floor. One continuous U per level — park in the basement, escape from the roof.
    ///
    /// Re-runnable: clears and rebuilds the MALL root, so layout tuning is a single menu click.
    public static class MallBuilder
    {
        public const float FloorHeight = 10f;

        // Slab spans X [-HalfX, HalfX], Z [-HalfZ, HalfZ]. The ramp shaft sits just outside +X.
        private const float HalfX = 32f;
        private const float HalfZ = 30f;
        private const float ShaftInner = 32f;
        private const float ShaftOuter = 41f;

        // Ramp runs from its low end at -RampBottomZ up to the landing at +RampTopZ. Both sit
        // inside the slab's Z range so the player never has to pass through the end walls.
        private const float RampBottomZ = -26f;
        private const float RampTopZ = 24f;

        private const float WallHeight = 7f;
        private const float WallThickness = 1f;

        private const string MallRootName = "=== MALL ===";

        [MenuItem("KrazyKart/Rebuild Mall")]
        public static void RebuildMall()
        {
            var existing = GameObject.Find(MallRootName);
            if (existing != null) Object.DestroyImmediate(existing);

            LoadEffectPrefabs();

            var root = new GameObject(MallRootName);
            var themes = MallTheme.All();
            var floors = new List<FloorDefinition>();

            for (int i = 0; i < themes.Length; i++)
                floors.Add(BuildFloor(themes[i], i, i == themes.Length - 1, root.transform));

            WireRunController(floors);
            Debug.Log($"[MallBuilder] Built {floors.Count} floors.");
        }

        private static FloorDefinition BuildFloor(MallTheme theme, int index, bool isTop, Transform parent)
        {
            var go = new GameObject($"Floor_{theme.Id}");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0f, index * FloorHeight, 0f);

            var def = go.AddComponent<FloorDefinition>();

            Material floorMat = MakeMaterial($"M_Floor_{theme.Id}", theme.FloorColor, 0.15f);
            Material wallMat = MakeMaterial($"M_Wall_{theme.Id}", theme.WallColor, 0.1f);
            Material accentMat = MakeMaterial($"M_Accent_{theme.Id}", theme.AccentColor, 0.3f);

            BuildSlab(go.transform, floorMat);
            BuildWalls(go.transform, wallMat);
            BuildPillars(go.transform, wallMat);
            BuildLighting(go.transform, theme);

            Transform entry = BuildEntry(go.transform, index);
            var copPoints = BuildCopSpawnPoints(go.transform);

            // Keep spawn, ramp mouth, landing and the central drift space free of props.
            var keepClear = new List<KeepClearZone>
            {
                new KeepClearZone(entry.localPosition, 11f),
                new KeepClearZone(new Vector3(HalfX - 4f, 0f, RampBottomZ), 12f),
                new KeepClearZone(new Vector3(HalfX - 4f, 0f, RampTopZ), 12f),
                new KeepClearZone(Vector3.zero, 7f)
            };

            PopulateProps(go.transform, theme, index, keepClear);

            if (!isTop) BuildExitRamp(go.transform, accentMat, wallMat);
            else BossFloorBuilder.Build(go.transform, theme, accentMat);

            ConfigureDefinition(def, theme, entry, copPoints);
            return def;
        }

        private static void ConfigureDefinition(FloorDefinition def, MallTheme theme, Transform entry, Transform[] copPoints)
        {
            var so = new SerializedObject(def);
            so.FindProperty("floorId").enumValueIndex = (int)theme.Id;
            so.FindProperty("displayName").stringValue = theme.DisplayName;
            so.FindProperty("entryPoint").objectReferenceValue = entry;
            so.FindProperty("copsEnabled").boolValue = theme.CopsEnabled;
            so.FindProperty("firstCopAt").intValue = theme.FirstCopAt;
            so.FindProperty("secondCopAt").intValue = theme.SecondCopAt;
            so.FindProperty("formationAt").intValue = theme.FormationAt;

            var points = so.FindProperty("copSpawnPoints");
            points.arraySize = copPoints.Length;
            for (int i = 0; i < copPoints.Length; i++)
                points.GetArrayElementAtIndex(i).objectReferenceValue = copPoints[i];

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        #region Geometry

        private static GameObject Box(string name, Transform parent, Vector3 localPos, Vector3 size, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = size;
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
            return go;
        }

        private static void BuildSlab(Transform parent, Material mat)
        {
            Box("Slab", parent, new Vector3(0f, -0.5f, 0f), new Vector3(HalfX * 2f, 1f, HalfZ * 2f), mat)
                .isStatic = true;
        }

        /// Three walls only. The +X edge is deliberately open onto the ramp shaft: it is both the
        /// way out and the way in, and a wall there would trap the player on the landing.
        private static void BuildWalls(Transform parent, Material mat)
        {
            var walls = new GameObject("Walls");
            walls.transform.SetParent(parent, false);

            float h = WallHeight * 0.5f;

            Box("Wall_ZPos", walls.transform, new Vector3(0f, h, HalfZ), new Vector3(HalfX * 2f, WallHeight, WallThickness), mat);
            Box("Wall_ZNeg", walls.transform, new Vector3(0f, h, -HalfZ), new Vector3(HalfX * 2f, WallHeight, WallThickness), mat);
            Box("Wall_XNeg", walls.transform, new Vector3(-HalfX, h, 0f), new Vector3(WallThickness, WallHeight, HalfZ * 2f), mat);
        }

        private static void BuildPillars(Transform parent, Material mat)
        {
            var pillars = new GameObject("Pillars");
            pillars.transform.SetParent(parent, false);

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && z == 0) continue;
                    Box($"Pillar_{x}_{z}", pillars.transform,
                        new Vector3(x * 18f, WallHeight * 0.5f, z * 16f),
                        new Vector3(1.6f, WallHeight, 1.6f), mat);
                }
            }
        }

        private static void BuildLighting(Transform parent, MallTheme theme)
        {
            var lights = new GameObject("Lights");
            lights.transform.SetParent(parent, false);

            // Readable and cheap: wide point lights, no realtime shadows.
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    var go = new GameObject($"Light_{x}_{z}");
                    go.transform.SetParent(lights.transform, false);
                    go.transform.localPosition = new Vector3(x * 20f, 6.5f, z * 15f);

                    var light = go.AddComponent<Light>();
                    light.type = LightType.Point;
                    light.range = 34f;
                    light.intensity = 2.6f;
                    light.color = Color.Lerp(Color.white, theme.AccentColor, 0.22f);
                    light.shadows = LightShadows.None;
                }
            }
        }

        private static Transform BuildEntry(Transform parent, int index)
        {
            var go = new GameObject("EntryPoint");
            go.transform.SetParent(parent, false);

            if (index == 0)
            {
                // B1 starts deep in the parking level, pointed down the hall toward the ramp.
                // Offset from the pillar grid (x and z multiples of 18/16) so the view is clear.
                go.transform.localPosition = new Vector3(-25f, 1.2f, 22f);
                go.transform.localRotation = Quaternion.Euler(0f, 160f, 0f);
                return go.transform;
            }

            // Every other floor is entered just inside the slab, facing across the hall.
            // Deliberately clear of the shaft so a respawn cannot land inside the previous
            // floor's exit trigger and immediately re-fire it.
            go.transform.localPosition = new Vector3(ShaftInner - 6f, 1.2f, RampTopZ - 2f);
            go.transform.localRotation = Quaternion.Euler(0f, 250f, 0f);
            return go.transform;
        }

        private static Transform[] BuildCopSpawnPoints(Transform parent)
        {
            var root = new GameObject("CopSpawnPoints");
            root.transform.SetParent(parent, false);

            var offsets = new[]
            {
                new Vector3(-24f, 1f, -22f), new Vector3(20f, 1f, -22f),
                new Vector3(-24f, 1f, 22f),  new Vector3(20f, 1f, 12f)
            };

            var points = new Transform[offsets.Length];
            for (int i = 0; i < offsets.Length; i++)
            {
                var go = new GameObject($"CopSpawn_{i}");
                go.transform.SetParent(root.transform, false);
                go.transform.localPosition = offsets[i];
                points[i] = go.transform;
            }

            return points;
        }

        /// The ramp shaft along +X: an inclined deck with kerbs, plus a flat landing at the top
        /// that meets the next floor's slab edge exactly.
        private static void BuildExitRamp(Transform parent, Material accentMat, Material wallMat)
        {
            var root = new GameObject("ExitRamp");
            root.transform.SetParent(parent, false);

            float x = (ShaftInner + ShaftOuter) * 0.5f;
            float width = ShaftOuter - ShaftInner;
            float run = RampTopZ - RampBottomZ;
            float slopeDeg = Mathf.Atan2(FloorHeight, run) * Mathf.Rad2Deg;
            float deckLength = Mathf.Sqrt(run * run + FloorHeight * FloorHeight);
            float midZ = (RampTopZ + RampBottomZ) * 0.5f;

            var deck = Box("RampDeck", root.transform,
                new Vector3(x, FloorHeight * 0.5f, midZ),
                new Vector3(width, 0.6f, deckLength), accentMat);
            deck.transform.localRotation = Quaternion.Euler(-slopeDeg, 0f, 0f);

            for (int s = -1; s <= 1; s += 2)
            {
                var kerb = Box($"RampKerb_{s}", root.transform,
                    new Vector3(x + s * width * 0.5f, FloorHeight * 0.5f + 0.7f, midZ),
                    new Vector3(0.5f, 1.8f, deckLength), wallMat);
                kerb.transform.localRotation = Quaternion.Euler(-slopeDeg, 0f, 0f);
            }

            // Flat landing level with the next floor's slab.
            Box("RampLanding", root.transform,
                new Vector3(x, FloorHeight - 0.3f, RampTopZ + 4f),
                new Vector3(width, 0.6f, 9f), accentMat);

            // Outer guard so the landing is not an open ledge.
            Box("LandingGuard", root.transform,
                new Vector3(ShaftOuter + 0.25f, FloorHeight + 1f, RampTopZ + 2f),
                new Vector3(0.5f, 2.4f, 13f), wallMat);

            // Low kerb at the shaft's foot so the player does not overshoot into the void.
            Box("ShaftFootGuard", root.transform,
                new Vector3(x, 0.6f, RampBottomZ - 3.2f),
                new Vector3(width, 1.2f, 0.6f), wallMat);

            BuildExitTrigger(root.transform, x);
        }

        private static void BuildExitTrigger(Transform parent, float x)
        {
            var go = new GameObject("FloorExitTrigger");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(x, FloorHeight + 1.6f, RampTopZ + 4f);

            var box = go.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(9f, 4f, 7f);

            go.AddComponent<FloorExitTrigger>();
        }

        #endregion

        #region Props

        /// A circular patch of floor that must stay empty.
        public struct KeepClearZone
        {
            public Vector3 Centre;
            public float Radius;

            public KeepClearZone(Vector3 centre, float radius)
            {
                Centre = new Vector3(centre.x, 0f, centre.z);
                Radius = radius;
            }

            public bool Blocks(Vector3 localPos)
            {
                var flat = new Vector3(localPos.x, 0f, localPos.z);
                return Vector3.Distance(flat, Centre) < Radius;
            }
        }

        private static bool IsBlocked(List<KeepClearZone> zones, Vector3 localPos)
        {
            for (int i = 0; i < zones.Count; i++)
                if (zones[i].Blocks(localPos)) return true;

            return false;
        }

        private static void PopulateProps(Transform parent, MallTheme theme, int index, List<KeepClearZone> keepClear)
        {
            var root = new GameObject("Props");
            root.transform.SetParent(parent, false);

            // Deterministic per floor so a rebuild reproduces the same layout.
            Random.InitState(9000 + index);

            BuildAisles(root.transform, theme, keepClear);
            BuildScatter(root.transform, theme, keepClear);
        }

        /// Aisles of large props across the player's path — the destruction clusters.
        private static void BuildAisles(Transform parent, MallTheme theme, List<KeepClearZone> keepClear)
        {
            if (theme.LargeProps == null || theme.LargeProps.Length == 0) return;

            int aisleCount = Mathf.RoundToInt(Mathf.Lerp(2f, 5f, theme.Density));
            int perAisle = Mathf.RoundToInt(Mathf.Lerp(3f, 7f, theme.Density));

            for (int a = 0; a < aisleCount; a++)
            {
                float x = Mathf.Lerp(-22f, 16f, aisleCount == 1 ? 0.5f : a / (float)(aisleCount - 1));

                for (int p = 0; p < perAisle; p++)
                {
                    float z = Mathf.Lerp(-20f, 20f, perAisle == 1 ? 0.5f : p / (float)(perAisle - 1));
                    var pos = new Vector3(x, 0f, z);
                    if (IsBlocked(keepClear, pos)) continue;

                    string prop = theme.LargeProps[Random.Range(0, theme.LargeProps.Length)];
                    SpawnProp(prop, parent, pos, Random.Range(0, 4) * 90f, 22, 1.35f, 3.2f);
                }
            }
        }

        /// Loose dressing between the aisles so chains have stepping stones.
        private static void BuildScatter(Transform parent, MallTheme theme, List<KeepClearZone> keepClear)
        {
            if (theme.SmallProps == null || theme.SmallProps.Length == 0) return;

            int count = Mathf.RoundToInt(Mathf.Lerp(10f, 34f, theme.Density));
            for (int i = 0; i < count; i++)
            {
                var pos = new Vector3(Random.Range(-28f, 26f), 0f, Random.Range(-25f, 25f));
                if (IsBlocked(keepClear, pos)) continue;

                string prop = theme.SmallProps[Random.Range(0, theme.SmallProps.Length)];
                SpawnProp(prop, parent, pos, Random.Range(0f, 360f), 8, 0.6f, 2.0f);
            }
        }

        private static void SpawnProp(string propName, Transform parent, Vector3 localPos, float yaw,
            int mcValue, float health, float splashRadius)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(MallTheme.Path(propName));
            if (prefab == null)
            {
                Debug.LogWarning($"[MallBuilder] Missing prop prefab: {propName}");
                return;
            }

            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            EnsureCollider(go);
            AddDestructible(go, mcValue, health, splashRadius);
        }

        /// Shop prefabs ship without colliders, so fit one to the visible bounds.
        private static void EnsureCollider(GameObject go)
        {
            if (go.GetComponentInChildren<Collider>() != null) return;

            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);

            var box = go.AddComponent<BoxCollider>();
            box.center = go.transform.InverseTransformPoint(bounds.center);

            Vector3 size = go.transform.InverseTransformVector(bounds.size);
            // Mirrored scales can produce negative extents, which break collision.
            box.size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
        }

        private static GameObject _breakVfx;
        private static GameObject _debrisChunk;

        /// Cached so a full rebuild does not hit the asset database hundreds of times.
        private static void LoadEffectPrefabs()
        {
            _breakVfx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/VFX/VFX_PropShatter.prefab");
            _debrisChunk = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/VFX/Debris_Chunk.prefab");
        }

        private static void AddDestructible(GameObject go, int mcValue, float health, float splashRadius)
        {
            var prop = go.GetComponent<DestructibleProp>();
            if (prop == null) prop = go.AddComponent<DestructibleProp>();

            var so = new SerializedObject(prop);
            so.FindProperty("mcValue").intValue = mcValue;
            so.FindProperty("health").floatValue = health;
            so.FindProperty("splashRadius").floatValue = splashRadius;

            // Assigned during generation so effects survive every rebuild.
            if (_breakVfx != null) so.FindProperty("breakVfxPrefab").objectReferenceValue = _breakVfx;
            if (_debrisChunk != null) so.FindProperty("debrisPrefab").objectReferenceValue = _debrisChunk;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region Materials

        private static Material MakeMaterial(string name, Color color, float smoothness)
        {
            const string dir = "Assets/Art/Materials";
            if (!AssetDatabase.IsValidFolder("Assets/Art")) AssetDatabase.CreateFolder("Assets", "Art");
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets/Art", "Materials");

            string path = $"{dir}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader) { name = name };
            mat.color = color;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);

            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        #endregion

        private static void WireRunController(List<FloorDefinition> floors)
        {
            var controller = Object.FindFirstObjectByType<RunController>();
            if (controller == null) return;

            var so = new SerializedObject(controller);
            var list = so.FindProperty("floors");
            list.arraySize = floors.Count;
            for (int i = 0; i < floors.Count; i++)
                list.GetArrayElementAtIndex(i).objectReferenceValue = floors[i];

            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
