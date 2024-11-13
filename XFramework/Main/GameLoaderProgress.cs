using UnityEngine;
using UnityEngine.UI;


namespace XEngine.Main
{
    public class GameLoaderProgress : MonoBehaviour
    {
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private Text loadingTip;
        private double progress = 0.0;

        public void UpdateLoadingProgress(long currentBytes, long totalBytes)
        {
            progress = (double)currentBytes / totalBytes;

            double loadedMB = (double)currentBytes / 1024 / 1024;
            double allMB = (double)totalBytes / 1024 / 1024;

            loadingSlider.value = (float)progress;
            loadingTip.text = string.Format("下载资源文件中... {0}%, {1}/{2}M",
                                             (progress * 100).ToString("f0"),
                                             loadedMB.ToString("f2"),
                                             allMB.ToString("f2"));

            if (progress >= 1.0)
            {
                loadingSlider.value = 1f;
                loadingTip.text = "下载完成!";
                End();
            }
        }

        public void Begin()
        {
            loadingSlider.value = 0f;
            gameObject.SetActive(true);
        }

        public void End()
        {
            loadingSlider.value = 1f;
            gameObject.SetActive(false);
        }
    }
}
