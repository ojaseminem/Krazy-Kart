using System.Collections.Generic;
using Gameplay;
using UnityEditor;
using UnityEngine;

namespace KrazyKartEditor
{
    /// Builds the manager's office: the four-phase escape at the top of the mall.
    ///
    /// Phase hardware is authored here and handed to BossSequence, which animates it as the
    /// escape clock burns down. The glass runway at the end is the run's payoff shot.
    public static class BossFloorBuilder
    {
        public static void Build(Transform floorRoot, MallTheme theme, Material accentMat)
        {
            var root = new GameObject("BossHardware");
            root.transform.SetParent(floorRoot, false);

            var shutters = BuildShutters(root.transform, accentMat);
            var bollards = BuildBollards(root.transform, accentMat);
            var robots = BuildCleaningRobots(root.transform, accentMat);
            var runway = BuildGlassRunway(root.transform, out var glassPanels, out var exitTrigger);
            var moodLights = CollectMoodLights(floorRoot);

            var sequence = floorRoot.gameObject.AddComponent<BossSequence>();
            var so = new SerializedObject(sequence);

            AssignArray(so, "shutters", shutters);
            AssignArray(so, "bollards", bollards);
            AssignArray(so, "cleaningRobots", robots);
            AssignArray(so, "moodLights", moodLights);
            so.FindProperty("glassRunway").objectReferenceValue = runway;
            so.ApplyModifiedPropertiesWithoutUndo();

            // The glass wall reports back into the sequence to fire the slow-motion exit.
            var glass = exitTrigger.GetComponent<GlassExitTrigger>();
            var glassSo = new SerializedObject(glass);

            var shatterVfx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/VFX/VFX_GlassShatter.prefab");
            if (shatterVfx != null) glassSo.FindProperty("shatterVfx").objectReferenceValue = shatterVfx;

            var panels = glassSo.FindProperty("glassPanels");
            panels.arraySize = glassPanels.Count;
            for (int i = 0; i < glassPanels.Count; i++)
                panels.GetArrayElementAtIndex(i).objectReferenceValue = glassPanels[i];
            glassSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignArray(SerializedObject so, string propertyName, IList<Object> values)
        {
            var prop = so.FindProperty(propertyName);
            prop.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        private static GameObject Box(string name, Transform parent, Vector3 pos, Vector3 size, Material mat, bool collider = true)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = size;
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!collider) Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        /// Phase 2: shutters slam down across the mid-floor openings.
        private static List<Object> BuildShutters(Transform parent, Material mat)
        {
            var list = new List<Object>();
            var root = new GameObject("Shutters");
            root.transform.SetParent(parent, false);

            for (int i = -1; i <= 1; i++)
            {
                // Parked above the opening; BossSequence drops them by shutterDropHeight.
                var shutter = Box($"Shutter_{i}", root.transform,
                    new Vector3(i * 18f, 9.4f, 6f), new Vector3(14f, 5f, 0.6f), mat);
                list.Add(shutter.transform);
            }

            return list;
        }

        /// Phase 2: bollards rise out of the floor to break the racing line.
        private static List<Object> BuildBollards(Transform parent, Material mat)
        {
            var list = new List<Object>();
            var root = new GameObject("Bollards");
            root.transform.SetParent(parent, false);

            for (int i = 0; i < 8; i++)
            {
                float x = Mathf.Lerp(-24f, 24f, i / 7f);
                var bollard = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bollard.name = $"Bollard_{i}";
                bollard.transform.SetParent(root.transform, false);
                // Sunk below the slab until phase 2 raises them.
                bollard.transform.localPosition = new Vector3(x, -1.2f, -8f);
                bollard.transform.localScale = new Vector3(0.9f, 1.2f, 0.9f);
                bollard.GetComponent<Renderer>().sharedMaterial = mat;
                list.Add(bollard.transform);
            }

            return list;
        }

        /// Phase 3: cleaning robots patrol the floor. Disabled until the phase fires.
        private static List<Object> BuildCleaningRobots(Transform parent, Material mat)
        {
            var list = new List<Object>();
            var root = new GameObject("CleaningRobots");
            root.transform.SetParent(parent, false);

            for (int i = 0; i < 4; i++)
            {
                var robot = new GameObject($"CleaningRobot_{i}");
                robot.transform.SetParent(root.transform, false);
                robot.transform.localPosition = new Vector3(Mathf.Lerp(-20f, 20f, i / 3f), 0.6f, Random.Range(-14f, 10f));

                var body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                body.name = "Body";
                body.transform.SetParent(robot.transform, false);
                body.transform.localScale = new Vector3(2.4f, 0.45f, 2.4f);
                body.GetComponent<Renderer>().sharedMaterial = mat;

                var patrol = robot.AddComponent<BossCleanerPatrol>();
                var so = new SerializedObject(patrol);
                so.FindProperty("patrolWidth").floatValue = Random.Range(10f, 20f);
                so.FindProperty("speed").floatValue = Random.Range(3.5f, 6f);
                so.ApplyModifiedPropertiesWithoutUndo();

                robot.SetActive(false);
                list.Add(robot);
            }

            return list;
        }

        /// Phase 4: the glass runway opens, ending in the wall the player launches through.
        private static GameObject BuildGlassRunway(Transform parent, out List<Object> glassPanels, out GameObject exitTrigger)
        {
            var runway = new GameObject("GlassRunway");
            runway.transform.SetParent(parent, false);

            var glassMat = MakeGlassMaterial();
            glassPanels = new List<Object>();

            // Boost strip leading to the wall.
            var strip = Box("BoostStrip", runway.transform, new Vector3(0f, 0.05f, 18f),
                new Vector3(12f, 0.1f, 20f), glassMat, collider: false);
            strip.GetComponent<Renderer>().sharedMaterial = glassMat;

            // The wall itself: panels the player smashes through.
            var wallRoot = new GameObject("GlassWall");
            wallRoot.transform.SetParent(runway.transform, false);

            for (int i = -2; i <= 2; i++)
            {
                var panel = Box($"GlassPanel_{i}", wallRoot.transform,
                    new Vector3(i * 3.1f, 3f, 29f), new Vector3(3f, 6f, 0.2f), glassMat, collider: false);
                glassPanels.Add(panel.GetComponent<Renderer>());
            }

            exitTrigger = new GameObject("GlassExitTrigger");
            exitTrigger.transform.SetParent(runway.transform, false);
            exitTrigger.transform.localPosition = new Vector3(0f, 3f, 29f);

            var box = exitTrigger.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(16f, 6f, 2f);
            exitTrigger.AddComponent<GlassExitTrigger>();

            runway.SetActive(false);
            return runway;
        }

        private static List<Object> CollectMoodLights(Transform floorRoot)
        {
            var list = new List<Object>();
            foreach (var light in floorRoot.GetComponentsInChildren<Light>(true))
                list.Add(light);

            return list;
        }

        private static Material MakeGlassMaterial()
        {
            const string dir = "Assets/Art/Materials";
            if (!AssetDatabase.IsValidFolder("Assets/Art")) AssetDatabase.CreateFolder("Assets", "Art");
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets/Art", "Materials");

            const string path = dir + "/M_BossGlass.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader) { name = "M_BossGlass" };
            var tint = new Color(0.65f, 0.9f, 1f, 0.35f);
            mat.color = tint;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", tint);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.9f);

            // URP transparent surface setup.
            if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }
    }
}
