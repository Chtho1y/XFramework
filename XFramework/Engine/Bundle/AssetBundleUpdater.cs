using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


namespace XEngine.Engine
{
    public class AssetBundleUpdater
    {
        public bool NeedsUpdate { get; private set; } = false; // 是否需要更新ab包
        private const int MaxRetryAccessServerTimes = 3; // 访问ab服务器的最大重试次数
        private int retriedAccessServerTimes = 0; // 访问ab服务器的已重试次数

        public async void CheckUpdate(ResourceMode resMode, string bundleServerUrl, Action<UpdateStatus> updateCallback, Action<long, long> updateLoaderProgress = null)
        {
            BundleManager.Instance.SetResourceMode(resMode);
            if (BundleManager.Instance.IsLoadRaw())
            {
                // 本地读取Lua源码文件
                GameManager.Instance.InitLua();
                updateCallback?.Invoke(UpdateStatus.NoNeedUpdate);
                return;
            }

            if (resMode == ResourceMode.LocalStreamingAssetBundle)
            {
                // 从本地的 StreamingAsset 读取文件
                // UnityWebRequest 不能直接在Mac,Linux上用 StreamingAssetPath 读取本地文件, 需要加上 file://
                bundleServerUrl = "file://" + PathProtocol.LocalStreamingAssetBundlePath;
            }

            // 本地缓存检查
            var localVersionFilePath = Path.Combine(PathProtocol.DownloadBundleSaveDir, PathProtocol.VersionFileName);
            VersionInfo cachedVersion = VersionUtil.LoadVersionInfoFromFile(localVersionFilePath);

            // 异步检查并快速返回
            await CheckUpdateAsync(resMode, bundleServerUrl, updateCallback, updateLoaderProgress);
        }


        public async Task CheckUpdateAsync(ResourceMode resMode, string bundleServerUrl, Action<UpdateStatus> updateCallback, Action<long, long> updateLoaderProgress = null)
        {
            BundleManager.Instance.SetResourceMode(resMode);
            if (BundleManager.Instance.IsLoadRaw())
            {
                GameManager.Instance.InitLua();
                updateCallback?.Invoke(UpdateStatus.NoNeedUpdate);
                return;
            }

            if (resMode == ResourceMode.LocalStreamingAssetBundle)
            {
                bundleServerUrl = "file://" + PathProtocol.LocalStreamingAssetBundlePath;
            }

            string versionUrl = bundleServerUrl + PathProtocol.VersionFileName;
            Debug.LogFormat($"CheckUpdateAsync[Retry={retriedAccessServerTimes}]: {versionUrl}");
            if (!Directory.Exists(PathProtocol.DownloadBundleSaveDir))
            {
                Directory.CreateDirectory(PathProtocol.DownloadBundleSaveDir);
            }

            while (!Caching.ready)
            {
                await Task.Yield(); // 异步等待，防止阻塞主线程
            }

            byte[] versionData = null;
            while (retriedAccessServerTimes < MaxRetryAccessServerTimes)
            {
                using (UnityWebRequest req = UnityWebRequest.Get(versionUrl))
                {
                    await req.SendWebRequest();

                    if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
                    {
                        retriedAccessServerTimes++;
                        Debug.LogFormat($"CheckUpdateAsync failed [retry={retriedAccessServerTimes}]: {req.error}]");
                        await Task.Delay(retriedAccessServerTimes * 3000); // 等待时间改为毫秒
                        continue;
                    }

                    if (req.isDone)
                    {
                        versionData = req.downloadHandler.data;
                        break;
                    }
                }
            }

            if (versionData == null)
            {
                Debug.LogError("CheckUpdateAsync failed: " + versionUrl);
                return;
            }

            string versionInfoJson = GameUtil.Bytes2String(versionData);
            var newVersion = JsonUtility.FromJson<VersionInfo>(versionInfoJson);

            UpdateStatus status = UpdateStatus.NoNeedUpdate;
            Queue<BundleInfo> toDownloads = new();
            long sumBytes = 0L;
            long downloadedBytes = 0L;
            var localVersionFilePath = Path.Combine(PathProtocol.DownloadBundleSaveDir, PathProtocol.VersionFileName);
            VersionInfo oldVersion = VersionUtil.LoadVersionInfoFromFile(localVersionFilePath);
            status = VersionUtil.CompareVersionForUpdate(oldVersion, newVersion);

            switch (status)
            {
                case UpdateStatus.NoNeedUpdate:
                    NeedsUpdate = false;
                    break;
                case UpdateStatus.NeedDownloadNewClient:
                    GameManager.Instance.InitLua();
                    updateCallback?.Invoke(status);
                    return;
                case UpdateStatus.FirstTime:
                    NeedsUpdate = true;
                    var firstNewBundleInfo = newVersion.DecodeBundleInfo();
                    foreach (KeyValuePair<string, BundleInfo> info in firstNewBundleInfo)
                    {
                        var filePath = PathProtocol.DownloadBundleSaveDir + info.Key;
                        if (!File.Exists(filePath))
                        {
                            toDownloads.Enqueue(info.Value);
                            sumBytes += info.Value.size;
                            continue;
                        }
                        var md5 = GameUtil.GetFileMD5(filePath);
                        if (md5 != info.Value.md5)
                        {
                            toDownloads.Enqueue(info.Value);
                            sumBytes += info.Value.size;
                        }
                    }
                    break;
                default:
                    NeedsUpdate = true;
                    var newBundleInfo = newVersion.DecodeBundleInfo();
                    var oldBundleInfo = oldVersion.DecodeBundleInfo();
                    foreach (KeyValuePair<string, BundleInfo> old in oldBundleInfo)
                    {
                        if (!newBundleInfo.ContainsKey(old.Key))
                        {
                            var filePath = PathProtocol.DownloadBundleSaveDir + old.Key;
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                    }
                    foreach (KeyValuePair<string, BundleInfo> info in newBundleInfo)
                    {
                        var filePath = PathProtocol.DownloadBundleSaveDir + info.Key;
                        if (!File.Exists(filePath))
                        {
                            toDownloads.Enqueue(info.Value);
                            sumBytes += info.Value.size;
                            continue;
                        }
                        var md5 = GameUtil.GetFileMD5(filePath);
                        if (md5 != info.Value.md5)
                        {
                            toDownloads.Enqueue(info.Value);
                            sumBytes += info.Value.size;
                        }
                    }
                    break;
            }

            while (toDownloads.Count > 0)
            {
                BundleInfo bundleInfo = toDownloads.Dequeue();
                string abName = bundleInfo.name;
                long size = bundleInfo.size;
                int retryCount = 0;
                bool downloadSuccessful = false;
                long existingFileSize = 0;
                string filePath = Path.Combine(PathProtocol.DownloadBundleSaveDir, abName);

                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    existingFileSize = fileInfo.Length;

                    // **MD5验证**
                    var existingFileMD5 = GameUtil.GetFileMD5(filePath);
                    if (existingFileMD5 == bundleInfo.md5)
                    {
                        downloadedBytes += existingFileSize;
                        updateLoaderProgress?.Invoke(downloadedBytes, sumBytes);
                        Debug.LogFormat($"File already exists and verified by MD5: {abName}");
                        continue; // 已下载且MD5验证通过，跳过下载
                    }
                    else
                    {
                        Debug.LogWarning($"MD5 mismatch for {abName}. Will re-download.");
                        File.Delete(filePath); // 删除损坏的文件，重新下载
                        existingFileSize = 0;
                    }
                }

                while (retryCount < MaxRetryAccessServerTimes && !downloadSuccessful)
                {
                    using (UnityWebRequest req2 = UnityWebRequest.Get(bundleServerUrl + abName))
                    {
                        if (existingFileSize > 0)
                        {
                            req2.SetRequestHeader("Range", "bytes=" + existingFileSize + "-");
                        }
                        req2.timeout = 60;
                        await req2.SendWebRequest();

                        if (req2.result == UnityWebRequest.Result.ConnectionError || req2.result == UnityWebRequest.Result.ProtocolError)
                        {
                            retryCount++;
                            Debug.LogFormat($"Download failed for {abName} [retry={retryCount}]: {req2.error}");
                            await Task.Delay(retryCount * 5000); // 重新等待时间改为毫秒
                            continue;
                        }

                        if (req2.isDone)
                        {
                            byte[] data = req2.downloadHandler.data;
                            if (existingFileSize > 0)
                            {
                                using (var fileStream = new FileStream(filePath, FileMode.Append))
                                {
                                    fileStream.Write(data, 0, data.Length);
                                }
                            }
                            else
                            {
                                GameUtil.Write2Disk(filePath, data);
                            }

                            downloadedBytes += data.Length;
                            updateLoaderProgress?.Invoke(downloadedBytes, sumBytes);
                            Debug.LogFormat($"Downloaded: {downloadedBytes}/{sumBytes} Bytes");

                            // **再次进行MD5验证**
                            var downloadedFileMD5 = GameUtil.GetFileMD5(filePath);
                            if (downloadedFileMD5 == bundleInfo.md5)
                            {
                                downloadSuccessful = true;
                            }
                            else
                            {
                                Debug.LogWarning($"MD5 mismatch after download for {abName}. Retrying...");
                                File.Delete(filePath); // 删除损坏的文件，重新下载
                                existingFileSize = 0;
                            }
                        }
                    }
                }

                if (!downloadSuccessful)
                {
                    throw new Exception($"Failed to download bundle {abName} after {MaxRetryAccessServerTimes} attempts.");
                }
            }

            var versionSaveFilePath = Path.Combine(PathProtocol.DownloadBundleSaveDir, PathProtocol.VersionFileName);
            GameUtil.Write2Disk(versionSaveFilePath, versionData);
            await Task.Delay(1000); // 等待1秒（延迟是为了确保所有数据写入磁盘）

            var allBundleSavedPath = Path.Combine(PathProtocol.DownloadBundleSaveDir, "AssetBundle");
            AssetBundle allBundle = AssetBundle.LoadFromFile(allBundleSavedPath);
            var manifest = allBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            BundleManager.Instance.SetManifest(manifest);
            GameManager.Instance.InitLua();
            updateCallback?.Invoke(status);
        }
    }
}
