using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XEngine.Engine;


namespace XEngine.Editor
{
    internal class AssetBundleBuilder
    {
        public static void Build(string abOutputDir, BuildTarget targetPlatform, string version)
        {
            // 执行 LuaProject 下的资源打包到 Res2Bundle 目录下
            Res2BundleBuilder.BuildRes2Bundle();
            // 如果输出目录不存在，则创建
            if (!Directory.Exists(abOutputDir))
            {
                Directory.CreateDirectory(abOutputDir);
            }
            // 分析 Res2Bundle 目录下的资源，准备打包列表和忽略列表
            AnalysisRes2Bundle(out var toBuildBundleArray, out var ignoredList);
            // 输出忽略的资源列表到控制台
            foreach (string item in ignoredList)
            {
                Debug.LogError("The asset will not be packed in bundle: " + item);
            }
            // 使用 BuildPipeline 打包 Asset Bundles
            BuildPipeline.BuildAssetBundles(abOutputDir, toBuildBundleArray, BuildAssetBundleOptions.ChunkBasedCompression, targetPlatform);
            // 刷新 Asset 数据库
            AssetDatabase.Refresh();
            // 构建版本信息字典
            var bundleName2BundleInfo = new Dictionary<string, BundleInfo>();
            string[] files = Directory.GetFiles(abOutputDir);
            foreach (var filePath in files)
            {
                // 排除 manifest 文件、meta 文件和版本文件
                if (filePath.EndsWith(".manifest") || filePath.EndsWith(".meta") || filePath.EndsWith(PathProtocol.VersionFileName))
                {
                    continue;
                }
                // 获取文件的 MD5 值和大小
                var fileMD5 = GameUtil.GetFileMD5(filePath);
                long fileSize = GameUtil.GetFileSize(filePath);
                var fileName = Path.GetFileName(filePath);
                var bundleInfo = new BundleInfo()
                {
                    name = fileName,
                    md5 = fileMD5,
                    size = fileSize
                };
                bundleName2BundleInfo[bundleInfo.name] = bundleInfo;
            }
            // 构建版本信息
            var versionInfo = VersionInfo.BuildVersionInfo(version, bundleName2BundleInfo);
            var verInfoJson = JsonUtility.ToJson(versionInfo);
            Debug.Log($"New build version info json is : {verInfoJson}");
            // 将版本信息写入文件
            File.WriteAllText(Path.Combine(abOutputDir, PathProtocol.VersionFileName), verInfoJson);
            // 刷新 Asset 数据库
            AssetDatabase.Refresh();
            // 删除不必要的 manifest 文件
            files = Directory.GetFiles(abOutputDir);
            foreach (var filePath in files)
            {
                if (filePath.EndsWith(".manifest") || filePath.EndsWith(".manifest.meta"))
                {
                    File.Delete(filePath);
                }
            }
            // 再次刷新 Asset 数据库
            AssetDatabase.Refresh();
        }

        // 不支持的文件扩展名列表
        private static readonly string[] abExtensionBlacklists = new string[]
        {
            ".dll",
            ".cs",
            ".gitignore",
            ".js",
            ".boo",
        };

        // 判断是否是支持的资源文件
        private static bool IsSupportedAsset(string file)
        {
            var mainAssetTypeAtPath = AssetDatabase.GetMainAssetTypeAtPath(file);
            // 排除 LightingDataAsset 和 DefaultAsset 类型的资源
            if (mainAssetTypeAtPath == typeof(LightingDataAsset) || mainAssetTypeAtPath == typeof(DefaultAsset))
            {
                return false;
            }
            // 检查不支持的扩展名
            foreach (var extension in abExtensionBlacklists)
            {
                if (file.EndsWith(extension))
                {
                    return false;
                }
            }
            return true;
        }

        // 分析 Res2Bundle 目录下的资源，准备打包列表和忽略列表
        private static void AnalysisRes2Bundle(out AssetBundleBuild[] toBuildBundleArray, out List<string> ignoredList)
        {
            var bundleName2Files = new Dictionary<string, List<string>>();

            // 检查 Res2Bundle 目录下的所有文件（不包括子文件夹），排除 .meta 文件
            ignoredList = new();
            string[] files = Directory.GetFiles(PathProtocol.Res2BundleDirPath);
            foreach (var filePath in files)
            {
                if (!filePath.EndsWith(".meta"))
                {
                    ignoredList.Add(Path.GetFileName(filePath));
                }
            }

            // 遍历 Res2Bundle 目录下的每个子文件夹，将每个子文件夹打包成一个 Asset Bundle
            string[] subDirPaths = Directory.GetDirectories(PathProtocol.Res2BundleDirPath);
            foreach (var subDirPath in subDirPaths)
            {
                var subDirName = Path.GetRelativePath(PathProtocol.Res2BundleDirPath, subDirPath);
                files = GameUtil.GetAllFilesFromDirectory(subDirPath);
                // 使用子文件夹名作为 bundle 名字
                var bundleName = subDirName.ToLower();
                if (!bundleName2Files.ContainsKey(bundleName))
                {
                    bundleName2Files.Add(bundleName, new List<string>());
                }
                for (var k = 0; k < files.Length; k++)
                {
                    if (files[k].EndsWith(".meta"))
                    {
                        continue;
                    }
                    string filePath = files[k].Replace('\\', '/');
                    string fileRelativePath = Path.GetRelativePath(subDirPath, filePath);
                    string assetFilePath = Path.Combine(PathProtocol.AssetsPathRes2BundleDir, subDirName, fileRelativePath);
                    if (IsSupportedAsset(assetFilePath))
                    {
                        var assetName = assetFilePath.ToLower(); // 使用相对路径作为资源名字, 在加载ab包的时候, 也使用相对路径加载
                        Debug.LogFormat("Asset name is {0}", assetName);
                        bundleName2Files[bundleName].Add(assetName);

                        var progress = (k + 1f) / files.Length;
                        EditorUtility.DisplayCancelableProgressBar("Collect Bundle Assets ..", assetFilePath, progress);
                    }
                    else
                    {
                        ignoredList.Add(assetFilePath);
                    }
                }
            }
            // 构建 AssetBundleBuild 数组
            toBuildBundleArray = bundleName2Files.Select(x => new AssetBundleBuild
            {
                assetBundleName = x.Key,
                assetNames = x.Value.ToArray()
            }).ToArray();
            // 清除进度条
            EditorUtility.ClearProgressBar();
        }

    }

}