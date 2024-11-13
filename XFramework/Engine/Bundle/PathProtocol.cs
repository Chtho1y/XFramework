using System.IO;
using UnityEngine;


namespace XEngine.Engine
{
	/// <summary>
	/// 项目的资源目录约定
	/// </summary>
	public static class PathProtocol
	{
		#region Lua Source Code
		public const string LuaProjectDir = "LuaProject/";

		public const string LuaSourceCodeDir = LuaProjectDir + "Luas/";
		public const string ProtoSourceCodeDir = LuaProjectDir + "Protos/";
		public const string LuaSourceCodeWindowsDir = LuaSourceCodeDir + "Window/";
		#endregion

		#region Res2Bundle and AssetBundle
		public const string VersionFileName = "Version.json";
		public static readonly string StreamingAssetsAssetBundleDir = Application.streamingAssetsPath + "/AssetBundle/";
		public static readonly string Res2BundleDirPath = Application.dataPath + "/Res2Bundle/";
		public static readonly string Res2BundleLuasDirPath = Application.dataPath + "/Res2Bundle/Luas/";
		public static readonly string Res2BundleProtosDirPath = Application.dataPath + "/Res2Bundle/Protos/";
		public const string AssetsPathRes2BundleDir = "Assets/Res2Bundle/";
		public const string AssetsPathRes2BundleLuasDir = "Assets/Res2Bundle/Luas/";
		public const string AssetsPathRes2BundlePrefabsWindowsDir = "Assets/Res2Bundle/Prefabs/Window/";

		public static readonly string DownloadBundleSaveDir = Application.isEditor ? Path.Combine(Application.dataPath, "AssetBundle") : Path.Combine(Application.persistentDataPath, "AssetBundle");
		public static readonly string LocalStreamingAssetBundlePath = Application.streamingAssetsPath + "/AssetBundle/";
		#endregion

		public const string InitScenePath = "Assets/Scenes/Init.unity";
	}
}