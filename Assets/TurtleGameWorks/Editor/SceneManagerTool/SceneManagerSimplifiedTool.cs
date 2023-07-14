using UnityEditor;
using UnityEngine;

namespace TurtleGameWorks.Editor.SceneManagerTool
{
    public class SceneManagerSimplifiedTool : SceneManagerTool
    {
        [MenuItem("TurtleGameWorks/Scene Manager Simplified")]
        private static void OpenWindow()
        {
            var window = GetWindow<SceneManagerSimplifiedTool>();
            window.titleContent = new GUIContent("Scene Manager Simplified");
            window.Show();
        }

        protected override void OnGUI()
        {
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

            EditorGUILayout.Space(20f);

            if (ShowSceneSection)
            {
                DrawSceneButtons();
            }

            EditorGUILayout.Space(20f);

            EditorGUILayout.EndScrollView();
        }

        private void DrawSceneButtons()
        {
            foreach (var i in SceneInputs)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();

                i.loadAdditive = EditorGUILayout.ToggleLeft("Load Additive", i.loadAdditive);
                
                GUILayout.Label(i.sceneAsset.name);
                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();

                var customNameStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 16
                };

                if (GUILayout.Button(i.customName, customNameStyle, GUILayout.MinWidth(100f), GUILayout.MinHeight(50f)))
                {
                    LoadScene(i.sceneAsset, i.loadAdditive);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(); // Add some spacing between scene inputs
            }
        }
    }
}
