using System.Collections.Generic;
using XEngine.Engine;
using UnityEditor;
using UnityEngine;


namespace XEngine.Editor
{
    internal class UIBuilderWindow : EditorWindow
    {
        private static readonly List<string> PrefabPaths = new();
        private bool initDone = false;
        private GUIStyle Style;

        [MenuItem("Tools/UI Builder", false, 100)]
        private static void ShowWindow()
        {
            UIBuilderWindow window = GetWindow<UIBuilderWindow>("UI Builder", true);
            window.minSize = new Vector2(500, 500);
            window.Show();
            PrefabPaths.Clear();
            string[] allFilesFromDirectory = GameUtil.GetAllFilesFromDirectory(PathProtocol.AssetsPathRes2BundlePrefabsWindowsDir);
            foreach (string filePath in allFilesFromDirectory)
            {
                if (filePath.EndsWith(".prefab"))
                {
                    PrefabPaths.Add(filePath);
                }
            }
        }

        private void InitStyles()
        {
            initDone = true;
            Style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold,
            };
        }

        private void OnGUI()
        {
            if (!initDone)
                InitStyles();
            GUILayout.Label(PathProtocol.AssetsPathRes2BundlePrefabsWindowsDir);
            if (GUILayout.Button("Generate All", Style))
            {
                foreach (var prefabPath in PrefabPaths)
                {
                    LuaCodeGenerator.GenerateLuaScript(prefabPath);
                }
                // Generator all 之后自动关闭窗口
                Close();
            }
            _ = GUILayout.BeginScrollView(Vector2.zero);
            foreach (var prefabPath in PrefabPaths)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(prefabPath);
                if (GUILayout.Button("Generate", GUILayout.Width(150)))
                {
                    LuaCodeGenerator.GenerateLuaScript(prefabPath);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.Space(10f);
        }
    }
}