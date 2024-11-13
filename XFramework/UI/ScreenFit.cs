using UnityEngine;


namespace XEngine.UI
{
    public class ScreenFit : MonoBehaviour
    {
        public bool ReverseFit = false;
        public bool BottomFit = false;
        [Tooltip("适配系数默认为3/4")] public float FitCoefficient = 0.75f;

        void Awake()
        {
            SetScreenFit(GetComponent<RectTransform>());
        }

        public void SetScreenFit(RectTransform rect)
        {
            Vector2 offsetMax = rect.offsetMax;
            Vector2 offsetMin = rect.offsetMin;

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    AdjustForAndroid(ref offsetMax, ref offsetMin);
                    break;

                case RuntimePlatform.IPhonePlayer:
                    AdjustForIPhone(ref offsetMax, ref offsetMin);
                    break;
            }

            rect.offsetMax = offsetMax;
            if (BottomFit)
                rect.offsetMin = offsetMin;
        }

        private void AdjustForAndroid(ref Vector2 offsetMax, ref Vector2 offsetMin)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (AndroidJavaObject decorView = activity.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView"))
                    {
                        using (AndroidJavaObject windowInsets = decorView.Call<AndroidJavaObject>("getRootWindowInsets"))
                        {
                            if (windowInsets != null)
                            {
                                using (AndroidJavaObject displayCutout = windowInsets.Call<AndroidJavaObject>("getDisplayCutout"))
                                {
                                    if (displayCutout != null)
                                    {
                                        int safeInsetTop = displayCutout.Call<int>("getSafeInsetTop");
                                        int safeInsetBottom = displayCutout.Call<int>("getSafeInsetBottom");
                                        int safeInsetLeft = displayCutout.Call<int>("getSafeInsetLeft");
                                        int safeInsetRight = displayCutout.Call<int>("getSafeInsetRight");

                                        offsetMax.y = -(safeInsetTop * FitCoefficient);
                                        offsetMin.y = safeInsetBottom * FitCoefficient;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (ReverseFit)
            {
                offsetMax.y = -offsetMax.y;
                if (BottomFit)
                    offsetMin.y = -offsetMin.y;
            }
        }

        private void AdjustForIPhone(ref Vector2 offsetMax, ref Vector2 offsetMin)
        {
            float safeInsetTop = Screen.safeArea.yMin;
            float safeInsetBottom = Screen.safeArea.yMax - Screen.safeArea.yMin;
            float safeInsetLeft = Screen.safeArea.xMin;
            float safeInsetRight = Screen.safeArea.xMax - Screen.safeArea.xMin;

            offsetMax.y = -(safeInsetTop * FitCoefficient);
            offsetMin.y = safeInsetBottom * FitCoefficient;

            if (ReverseFit)
            {
                offsetMax.y = -offsetMax.y;
                if (BottomFit)
                    offsetMin.y = -offsetMin.y;
            }
        }
    }
}