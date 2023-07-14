using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TurtleGameWorks.Editor.SceneManagerTool
{
    public class SceneManagerTool : EditorWindow
    {
        protected SceneInput[] SceneInputs;
        private bool _showConfigSection;
        private bool _showSetupSection = true;
        protected bool ShowSceneSection;

        protected Vector2 ScrollPosition;

        private string _saveFilePath = "Assets/Settings/ToolSettings/SceneManagerToolSettings.json";

        private const string DefaultSaveFilePath = "Assets/Settings/ToolSettings/SceneManagerToolSettings.json";
        
        [MenuItem("TurtleGameWorks/Scene Manager Tool")]
        private static void OpenWindow()
        {
            var window = GetWindow<SceneManagerTool>();
            window.titleContent = new GUIContent("Scene Manager Tool");
            window.LoadSettings();
            window.Show();
        }

        private void OnEnable()
        {
            SceneInputs = Array.Empty<SceneInput>();
            _showConfigSection = false;
            LoadSettings();
        }

        protected virtual void OnGUI()
        {
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            
            EditorGUILayout.Space(20f);
            
            if (ShowSceneSection)
            {
                GUILayout.Label("Scene Section", EditorStyles.boldLabel);
                DrawSceneSlots();
            }

            EditorGUILayout.Space(20f);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(20f);
            
            GUILayout.Label("Setup Section", EditorStyles.boldLabel);
            
            EditorGUILayout.Space(20f);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("LoadSettings"))
            {
                LoadSettings();
            }

            if (GUILayout.Button("SaveSettings"))
            {
                SaveSettings();
            }

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(20f);

            _showSetupSection = EditorGUILayout.Foldout(_showSetupSection, "Setup");

            if (_showSetupSection)
            {
                DrawSceneInputs();
                DrawSetupButtons();
            }

            EditorGUILayout.Space(20f);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(20f);
            
            _showConfigSection = EditorGUILayout.Foldout(_showConfigSection, "Configure Tool");

            if (_showConfigSection)
            {
                ResetTool();
            }

            EditorGUILayout.Space(100f);
            
            EditorGUILayout.EndScrollView();
        }

        private void ResetTool()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();
            
            _saveFilePath = EditorGUILayout.TextField("Set Save Data file location: ", _saveFilePath);
            
            EditorGUILayout.Separator();

            if (GUILayout.Button("Reset Everything"))
            {
                bool resetConfirmed = EditorUtility.DisplayDialog("Reset Confirmation", "Are you sure you want to reset Scene Loader Tool?", "Yes", "No");

                if (resetConfirmed)
                {
                    ClearSceneInputs();
                    _saveFilePath = DefaultSaveFilePath;
                    Debug.Log("<color=#ff525d>Successfully Reset Scene Loader Tool</color>");
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawSceneInputs()
        {
            EditorGUILayout.Space(20f);

            for (int i = 0; i < SceneInputs.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                SceneInputs[i].sceneAsset = (SceneAsset)EditorGUILayout.ObjectField("Scene Input " + (i + 1),
                    SceneInputs[i].sceneAsset, typeof(SceneAsset), false);
                SceneInputs[i].customName = EditorGUILayout.TextField("Set Scene Name: " + (i + 1), SceneInputs[i].customName);

                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space(20f);

            if (GUILayout.Button("Add Scene Input"))
            {
                AddSceneInput();
            }
        }

        protected void DrawSetupButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Setup"))
            {
                if (SceneInputs.Length > 0)
                {
                    ShowSceneSection = true;
                    SaveSettings();
                }
                else
                {
                    Debug.Log("Please add at least one scene input.");
                }
            }

            if (GUILayout.Button("Clear"))
            {
                ClearSceneInputs();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSceneSlots()
        {
            for (int i = 0; i < SceneInputs.Length; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                
                SceneInputs[i].loadAdditive = EditorGUILayout.ToggleLeft("Load Additive", SceneInputs[i].loadAdditive);

                
                if(SceneInputs[i].sceneAsset != null) GUILayout.Label(SceneInputs[i].sceneAsset.name);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Remove", GUILayout.Width(70f)))
                {
                    RemoveSceneInput(i);
                    return; // Exit the loop after removing the scene input
                }

                EditorGUILayout.EndHorizontal();

                /*EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Scene Name:");
                GUILayout.Label(_sceneInputs[i].sceneAsset != null ? _sceneInputs[i].sceneAsset.name : "None");
                EditorGUILayout.EndHorizontal();*/

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(SceneInputs[i].customName, GUILayout.MinWidth(70f), GUILayout.ExpandWidth(true), GUILayout.MinHeight(30f)))
                {
                    LoadScene(SceneInputs[i].sceneAsset, SceneInputs[i].loadAdditive);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(); // Add some spacing between scene inputs
            }
        }

        private void AddSceneInput()
        {
            ArrayUtility.Add(ref SceneInputs, new SceneInput());
        }

        protected void RemoveSceneInput(int index)
        {
            if (index < 0 || index >= SceneInputs.Length) return;

            if (SceneInputs.Length == 1)
            {
                ClearSceneInputs();
                return;
            }

            ArrayUtility.RemoveAt(ref SceneInputs, index);
        }

        private void ClearSceneInputs()
        {
            SceneInputs = Array.Empty<SceneInput>();
            ShowSceneSection = false;
        }

        protected void LoadScene(SceneAsset sceneAsset, bool loadAdditive)
        {
            if (sceneAsset == null) return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);

            switch (loadAdditive)
            {
                case true:
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    break;
                case false:
                    EditorSceneManager.OpenScene(scenePath);
                    break;
            }
        }

        private void SaveSettings()
        {
            var settings = new SceneLoaderSettings
            {
                sceneInputs = SceneInputs
            };

            foreach (var sceneInput in settings.sceneInputs)
            {
                if (sceneInput.sceneAsset != null)
                {
                    string scenePath = AssetDatabase.GetAssetPath(sceneInput.sceneAsset);
                    sceneInput.scenePath = scenePath;
                }
            }

            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);

            string folderPath = Path.GetDirectoryName(_saveFilePath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath!);

            File.WriteAllText(_saveFilePath, json);
        }

        private void LoadSettings()
        {
            if (!File.Exists(_saveFilePath)) return;

            string json = File.ReadAllText(_saveFilePath);

            var settings = JsonConvert.DeserializeObject<SceneLoaderSettings>(json, new SceneInputConverter());

            if (settings != null)
            {
                foreach (var sceneInput in settings.sceneInputs)
                {
                    if (sceneInput.scenePath == string.Empty) continue;
                    
                    sceneInput.sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneInput.scenePath);
                }
            }

            SceneInputs = settings?.sceneInputs ?? Array.Empty<SceneInput>();
            ShowSceneSection = SceneInputs.Length > 0;

            Repaint();
        }
    }

    [Serializable]
    public class SceneInput
    {
        [JsonIgnore]
        public SceneAsset sceneAsset;
        public string customName;
        public bool loadAdditive;
        public string scenePath;
    }

    [Serializable]
    public class SceneLoaderSettings
    {
        public SceneInput[] sceneInputs;
    }

    public class SceneInputConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SceneInput);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var sceneInput = new SceneInput();

            foreach (var property in jsonObject.Properties())
            {
                switch (property.Name)
                {
                    case "scenePath":
                    {
                        string scenePath = property.Value.ToString();
                        sceneInput.scenePath = scenePath;
                        
                        sceneInput.sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                        break;
                    }
                    case "customName":
                        sceneInput.customName = property.Value.ToString();
                        break;
                }
            }

            return sceneInput;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
    }

}
