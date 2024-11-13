using UnityEditor;
using UnityEngine;
using XEngine.Engine;


namespace XEngine.Editor
{
    internal class AssetBundleBuilderOptions
    {
        public string Version;
        public BuildTarget Platform;
        public string AssetBundleOutputDirectory;

        internal static class EditorPrefSaveKey
        {
            public static readonly string Version = Application.productName + "_Version";

            public static readonly string Platform = Application.productName + "_Platform";

            public static readonly string AssetBundleOutputDirectory = Application.productName + "_AssetBundleOutputDirectory";

        }

        public static AssetBundleBuilderOptions DefaultOption()
        {
            return new AssetBundleBuilderOptions
            {
                Version = DefaultVersion,
                AssetBundleOutputDirectory = DefaultAssetBundleOutputDirectory(),
                Platform = DefaultBuildTarget()
            };
        }

        private const string DefaultVersion = "1.0.0.0";

        private static BuildTarget DefaultBuildTarget()
        {
#if UNITY_IOS
            return BuildTarget.iOS;
#elif UNITY_ANDROID
            return BuildTarget.Android;
#elif UNITY_STANDALONE_OSX
            return BuildTarget.StandaloneOSX;
#elif UNITY_STANDALONE_WIN
            return BuildTarget.StandaloneWindows64;
#else
            return BuildTarget.NoTarget
#endif
        }

        private static string DefaultAssetBundleOutputDirectory()
        {
            return PathProtocol.StreamingAssetsAssetBundleDir;
        }


        public void SaveToEditorPref()
        {
            EditorPrefs.SetString(EditorPrefSaveKey.Version, Version);
            EditorPrefs.SetInt(EditorPrefSaveKey.Platform, (int)Platform);
            EditorPrefs.SetString(EditorPrefSaveKey.AssetBundleOutputDirectory, AssetBundleOutputDirectory);
        }

        public static AssetBundleBuilderOptions LoadFromEditorPref()
        {
            return new AssetBundleBuilderOptions
            {
                Version = EditorPrefs.GetString(EditorPrefSaveKey.Version, DefaultVersion),
                Platform = (BuildTarget)EditorPrefs.GetInt(EditorPrefSaveKey.Platform, (int)DefaultBuildTarget()),
                AssetBundleOutputDirectory = EditorPrefs.GetString(EditorPrefSaveKey.AssetBundleOutputDirectory, DefaultAssetBundleOutputDirectory()),
            };
        }
    }
}
