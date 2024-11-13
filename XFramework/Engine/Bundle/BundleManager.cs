using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;


namespace XEngine.Engine
{
    public class BundleManager : Singleton<BundleManager>
    {
        private readonly Dictionary<string, AssetBundle> CachedBundles = new();
        private ResourceMode resourceMode;
        private AssetBundleManifest Manifest;

        public BundleManager()
        {
        }

        public override void Init()
        {
        }

        public override void Dispose()
        {
        }

        public void SetManifest(AssetBundleManifest manifest)
        {
            Manifest = manifest;
        }

        public void SetResourceMode(ResourceMode mode)
        {
            resourceMode = mode;
        }

        public void UnloadUnusedAssets(bool withGC = false)
        {
            Resources.UnloadUnusedAssets();
            if (withGC)
            {
                GC.Collect();
            }
        }

        public void UnloadBundle(string bundleName, bool unloadAllLoadedInBundle)
        {
            if (CachedBundles != null)
            {
                bundleName = bundleName.ToLower();
                CachedBundles.TryGetValue(bundleName, out var assetBundle);
                if (assetBundle != null)
                {
                    assetBundle.Unload(unloadAllLoadedInBundle);
                    CachedBundles.Remove(bundleName);
                }
            }
        }

        public bool IsLoadRaw()
        {
            return resourceMode == ResourceMode.Raw && Application.isEditor;
        }

        private T LoadRaw<T>(string path) where T : UnityEngine.Object
        {
            return (T)LoadRaw(path, typeof(T));
        }

        private UnityEngine.Object LoadRaw(string path, Type type)
        {
#if UNITY_EDITOR
            string localAssetPath = PathProtocol.AssetsPathRes2BundleDir + path;

            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(localAssetPath, type);
            return asset;
#else
			UnityEngine.Debug.LogError("Load Raw Resource is only support in UnityEditor");
			return null;
#endif
        }

        public AssetBundle LoadBundle(string bundleName)
        {
            if (CachedBundles == null)
            {
                return null;
            }
            bundleName = bundleName.ToLower();
            bool loaded = CachedBundles.TryGetValue(bundleName, out AssetBundle assetBundle);
            if (assetBundle == null)
            {
                assetBundle = AssetBundle.LoadFromFile(Path.Combine(PathProtocol.DownloadBundleSaveDir, bundleName));
                if (assetBundle == null)
                {
                    Debug.LogError("Load Bundle Error : " + bundleName);
                    return null;
                }
            }
            if (!loaded)
            {
                CachedBundles.Add(bundleName, assetBundle);
            }
            return assetBundle;
        }

        public void LoadAllDependencies(string bundleName)
        {
            string[] allDependencies = Manifest.GetAllDependencies(bundleName);
            foreach (var d in allDependencies)
            {
                _ = LoadBundle(d);
                Debug.Log("Load dependency:" + d);
            }
        }

        public T Load<T>(string path) where T : UnityEngine.Object
        {
            return (T)Load(path, typeof(T));
        }

        public UnityEngine.Object Load(string path, Type type)
        {
            if (IsLoadRaw())
            {
                return LoadRaw(path, type);
            }
            string[] array = path.Split('/');
            string bundleName = array[0];
            string assetName = PathProtocol.AssetsPathRes2BundleDir + path;
            AssetBundle bundle = LoadBundle(bundleName);
            if (bundle == null)
            {
                return null;
            }
            LoadAllDependencies(bundleName);
            return bundle.LoadAsset(assetName, type);
        }

        internal byte[] LoadLua(string path)
        {
            if (IsLoadRaw())
            {
                // 直接读取LuaProject里的源码文件
                var luaBytes = GameUtil.File2UTF8(Path.Combine(PathProtocol.LuaProjectDir, path));
                if (luaBytes == null)
                {
                    Debug.LogError("Lua Editor Load Error : " + path);
                }
                return luaBytes;
            }

            // 从ab包中读取加密的lua文件
            var encryptedLuaBytes = Load<TextAsset>(path + ".txt");
            if (encryptedLuaBytes == null)
            {
                Debug.LogError("Lua Bundle Load Error : " + path);
                return null;
            }
            return CryptoTool.Decrypt(encryptedLuaBytes.bytes);
        }

        internal byte[] LoadProto(string path)
        {
            if (IsLoadRaw())
            {
                var protoBytes = GameUtil.File2UTF8(Path.Combine(PathProtocol.LuaProjectDir, path));
                if (protoBytes == null)
                {
                    Debug.LogError("Proto Load Error : " + path);
                }
                return protoBytes;
            }
            // 从ab包中读取加密的Proto文件
            var encryptedProtoBytes = Load<TextAsset>(path + ".txt");
            if (encryptedProtoBytes == null)
            {
                Debug.LogError("Proto Bundle Load Error : " + path);
                return null;
            }
            return CryptoTool.Decrypt(encryptedProtoBytes.bytes);
        }

        public IEnumerator LoadBundleAsync(string bundleName, Action<AssetBundle> callback)
        {
            if (CachedBundles == null)
            {
                Debug.LogError("Not supported resource type in bundle : " + bundleName);
                callback?.Invoke(null);
                yield break;
            }
            bundleName = bundleName.ToLower();
            CachedBundles.TryGetValue(bundleName, out AssetBundle bundle);
            if (bundle != null)
            {
                callback?.Invoke(bundle);
                yield break;
            }
            AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathProtocol.DownloadBundleSaveDir + bundleName);
            yield return req;
            bundle = req.assetBundle;
            if (bundle == null)
            {
                Debug.LogError("Load Bundle Async Error : " + bundleName);
                callback?.Invoke(null);
            }
            else
            {
                CachedBundles.Add(bundleName, bundle);
                callback?.Invoke(bundle);
            }
        }

        public IEnumerator LoadAsync<T>(string path, Action<UnityEngine.Object> handler) where T : UnityEngine.Object
        {
            yield return LoadAsync(path, typeof(T), handler);
        }

        public IEnumerator LoadAsync(string path, Type type, Action<UnityEngine.Object> handler)
        {
            UnityEngine.Object asset = null;
            if (IsLoadRaw())
            {
                asset = LoadRaw(path, type);
                handler?.Invoke(asset);
                yield break;
            }
            string[] array = path.Split('/');
            string bundleName = array[0];
            string assetName = array[^1];

            void cb(AssetBundle bundle)
            {
                if (bundle == null)
                {
                    Debug.LogError("Load Asset Async Error : " + path);
                }
                else
                {
                    LoadAllDependencies(bundleName);
                    asset = bundle.LoadAsset(assetName, type);
                }
                handler?.Invoke(asset);
            }
            yield return LoadBundleAsync(bundleName, cb);
        }

        public IEnumerator LoadSceneAsync(string sceneName, Action onLoaded, Action<float> onLoading)
        {
            AsyncOperation asy = SceneManager.LoadSceneAsync(sceneName);
            asy.allowSceneActivation = false;
            float progress = 0f;
            onLoading?.Invoke(0f);

            while (asy.progress < 0.9f)
            {
                onLoading?.Invoke(asy.progress);
                yield return null;
            }

            while (progress < 1f)
            {
                progress += 0.01f;
                if (progress > 1f)
                {
                    progress = 1f;
                }
                onLoading?.Invoke(progress);
                yield return new WaitForEndOfFrame();
            }

            UnloadUnusedAssets(withGC: true);
            yield return new WaitForEndOfFrame();

            asy.allowSceneActivation = true;

            while (!asy.isDone)
            {
                yield return null;
            }

            onLoading?.Invoke(1f);
            onLoaded?.Invoke();
        }
    }
}