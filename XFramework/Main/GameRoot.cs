using System.Globalization;
using XEngine.Engine;
using UnityEngine;


namespace XEngine.Main
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }

        [SerializeField]
        private GameLoaderProgress loaderProgress;

        public GameLoaderProgress LoaderProgress => loaderProgress;

        public bool IsFocused { get; private set; }
        public bool IsPaused { get; private set; }

        public bool IsHangup
        {
            get
            {
                return !IsFocused || IsPaused;
            }
        }

        [Header("Load from Raw, StreamingAsset or Server")]
        public ResourceMode resMode = ResourceMode.Raw; // Raw在Editor下直接读取源文件, 在其他环境下是读取ab包

        private void Awake()
        {
            Instance = this;

            // 设置所有新线程的默认语言偏好为英国英语
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en-GB");

            // 设置当前主线程的语言偏好为英国英语
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-GB");

            Application.runInBackground = true; // 允许应用后台运行
            Application.targetFrameRate = 60; // 锁60帧
            Input.multiTouchEnabled = false; // 禁用多点触控
        }

        private void Start()
        {
            LoaderProgress.Begin();
            GameUtil.GetOrAddComponent<GameManager>(this.gameObject);

            var assetBundleUpdater = new AssetBundleUpdater();
            assetBundleUpdater.CheckUpdate(resMode,
                                            BundleServerConfig.ServerUrl,
                                            OnCheckUpdateFinished,
                                            LoaderProgress.UpdateLoadingProgress);
        }

        private void OnCheckUpdateFinished(UpdateStatus status)
        {
            if (status == UpdateStatus.NeedDownloadNewClient)
            {
                Debug.LogError("Please download the latest client version!");
            }
            else
            {
                LoaderProgress.End();
                GameManager.Instance.OnCheckUpdateFinished();
            }
        }

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
