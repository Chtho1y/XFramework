using UnityEditor;
using UnityEngine;
using XEngine.Engine;


namespace XEngine.Editor
{
    internal class AssetBundleBuilderWindow : EditorWindow
    {
        private static AssetBundleBuilderOptions buildBundleOptions;
        private bool initDone = false;
        private GUIStyle Style;

        [MenuItem("Tools/AssetBundle Builder", false, 100)]
        private static void ShowWindow()
        {
            AssetBundleBuilderWindow window = GetWindow<AssetBundleBuilderWindow>("AssetBundle Builder", true);
            window.minSize = new Vector2(500, 500);
            window.Show();
        }

        private void OnEnable()
        {
            buildBundleOptions = AssetBundleBuilderOptions.LoadFromEditorPref();
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
            GUILayout.BeginVertical();

            // version
            GUILayout.BeginHorizontal();
            GUILayout.Label("Version:");
            buildBundleOptions.Version = GUILayout.TextField(buildBundleOptions.Version);
            if (GUILayout.Button("Reset"))
            {
                buildBundleOptions = AssetBundleBuilderOptions.DefaultOption();
                buildBundleOptions.SaveToEditorPref();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(20);

            // output path
            GUILayout.Label("AssetBundle Output: " + buildBundleOptions.AssetBundleOutputDirectory);
            if (GUILayout.Button("Select AssetBundle Output Directory"))
            {
                buildBundleOptions.AssetBundleOutputDirectory = EditorUtility.OpenFolderPanel("Select AssetBundle Output Directory", buildBundleOptions.AssetBundleOutputDirectory, "");
                if (string.IsNullOrEmpty(buildBundleOptions.AssetBundleOutputDirectory))
                {
                    buildBundleOptions.AssetBundleOutputDirectory = PathProtocol.StreamingAssetsAssetBundleDir;
                }
            }
            EditorGUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Target Platform:");
            buildBundleOptions.Platform = (BuildTarget)EditorGUILayout.EnumPopup(buildBundleOptions.Platform);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(20);

            // build bundle
            if (GUILayout.Button("Build AssetBundle", Style) && EditorUtility.DisplayDialog("Tip", "Start Build ?", "Yes", "No"))
            {
                Close();
                AssetBundleBuilder.Build(buildBundleOptions.AssetBundleOutputDirectory, buildBundleOptions.Platform, buildBundleOptions.Version);
                buildBundleOptions.Version = VersionUtil.NextVersion(buildBundleOptions.Version);
                buildBundleOptions.SaveToEditorPref();
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.Space(20);

            GUILayout.EndVertical();
        }
    }

}