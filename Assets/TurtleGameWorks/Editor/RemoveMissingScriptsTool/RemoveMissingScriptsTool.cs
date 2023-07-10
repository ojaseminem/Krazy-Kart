/*
 ####################################
 ####################################
     Remove Missing Scripts Tool
                BY
         Turtle Game Works
 ####################################
 ####################################
 */

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TurtleGameWorks.Editor.RemoveMissingScriptsTool
{
    public class RemoveMissingScriptsTool : EditorWindow
    {
        private GameObject[] m_Prefabs;

        [MenuItem("Tools/TurtleGameWorks/Remove Missing Scripts")]
        public static void ShowWindow()
        {
            var window = GetWindow<RemoveMissingScriptsTool>();
            window.titleContent = new GUIContent("Missing Scripts Remover");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Prefab List", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Drag and drop prefabs here:");

            var dragArea = GUILayoutUtility.GetRect(0, 35, GUILayout.ExpandWidth(true));
            GUI.Box(dragArea, "Drag and drop prefabs here", EditorStyles.helpBox);

            var evt = Event.current;
            if (evt.type is EventType.DragUpdated or EventType.DragPerform)
            {
                if (dragArea.Contains(evt.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            var prefab = draggedObject as GameObject;
                            if (prefab == null) continue;
                            
                            System.Array.Resize(ref m_Prefabs, m_Prefabs != null ? m_Prefabs.Length + 1 : 1);
                            m_Prefabs[^1] = prefab;
                        }
                    }

                    Event.current.Use();
                }
            }

            EditorGUILayout.Space();

            if (m_Prefabs is { Length: > 0 })
            {
                EditorGUILayout.LabelField("Prefabs:", EditorStyles.boldLabel);

                for (int i = 0; i < m_Prefabs.Length; i++)
                {
                    EditorGUILayout.ObjectField("Prefab " + i, m_Prefabs[i], typeof(GameObject), false);
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            if (GUILayout.Button("Remove Missing Scripts"))
            {
                RemoveMissingScripts();
            }
            
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.Space();

            if (GUILayout.Button("Remove All Missing Scripts from Scene"))
            {
                RemoveAllMissingScriptsFromScene();
            }
        }

        private void RemoveMissingScripts()
        {
            if (m_Prefabs == null || m_Prefabs.Length == 0)
            {
                Debug.LogWarning("No prefabs selected.");
                return;
            }

            foreach (var prefab in m_Prefabs)
            {
                if (prefab == null)
                {
                    Debug.LogWarning("Prefab is null.");
                    continue;
                }

                var components = prefab.GetComponentsInChildren<Component>(true);

                foreach (var component in components)
                {
                    if (component == null)
                    {
                        Debug.LogWarning($"Missing script found in prefab '{prefab.name}'. Removing it.");
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefab);
                        break;
                    }
                }
            }

            Debug.Log("Missing scripts removed from selected prefabs.");
        }
        
        private void RemoveAllMissingScriptsFromScene()
        {
            var gameObjects = FindObjectsOfType<GameObject>();

            foreach (var gameObject in gameObjects)
            {
                var components = gameObject.GetComponents<Component>();

                if (components.All(component => component != null)) continue;
                
                Debug.LogWarning($"Missing script found in scene object '{gameObject.name}'. Removing it.");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
            }

            Debug.Log("Missing scripts removed from scene objects.");
        }
    }
}
