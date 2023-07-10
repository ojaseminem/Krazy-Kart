/*
 ####################################
 ####################################
     Capture Screenshot Tool
                BY
         Turtle Game Works
 ####################################
 ####################################
 */

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TurtleGameWorks.Editor.CaptureScreenshotTool
{
    public static class CaptureScreenshotTool
    {
        public const string KEditorPref = "CaptureScreenshotPath";
        public const string KMenuPath = "TurtleGameWorks/Capture Screenshot";
        
        // You can change the shortcut to your needs
        // Use "%#F11" to use ctrl + F11 for screenshot
        // You can also change the F11 to any key you'd like
        
        [MenuItem(KMenuPath + " _F11")]
        public static void CaptureScreenshotToolMenuItem()
        {
            var path = EditorPrefs.GetString(KEditorPref);
            if (string.IsNullOrWhiteSpace(path))
                path = GetDefaultPath();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filepath = Path.Combine(path, $"{Application.productName}_{DateTime.Now:yyyymmddhhmmss}.png");

            ScreenCapture.CaptureScreenshot(filepath, 1);

            Debug.Log("Screenshot saved to path :: " + filepath);
        }

        public static string GetDefaultPath()
        {
            var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Screenshots");
            return defaultPath;
        }
    }

    public class CaptureScreenshotSettingsProvider : SettingsProvider
    {
        private CaptureScreenshotSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope)
        { }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);

            GUILayout.Space(20f);

            var path = EditorPrefs.GetString(CaptureScreenshotTool.KEditorPref);

            if (string.IsNullOrWhiteSpace(path))
                path = CaptureScreenshotTool.GetDefaultPath();

            var changedPath = EditorGUILayout.TextField("Screenshot Path", path);

            if (string.CompareOrdinal(path, changedPath) != 0)
            {
                EditorPrefs.SetString(CaptureScreenshotTool.KEditorPref, changedPath);
            }
            
            GUILayout.Space(10f);

            if (GUILayout.Button("Reset to Default", GUILayout.Width(150f)))
            {
                EditorPrefs.DeleteKey(CaptureScreenshotTool.KEditorPref);
                Repaint();
            }
        }

       [SettingsProvider]
        public static SettingsProvider CreateCaptureScreenshotSettingsProvider()
        {
            var captureScreenshotSettingsProvider =
                new CaptureScreenshotSettingsProvider(CaptureScreenshotTool.KMenuPath);
            return captureScreenshotSettingsProvider;
        }
    }
}
