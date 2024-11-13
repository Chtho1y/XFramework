using System;
using UnityEngine;


namespace XEngine.Engine
{
    public static class UniWebViewUtil
    {
        private static UniWebView webView;

        public static void InitializeWebView(RectTransform webViewRect, string urlOnStart = null, bool showOnStart = false, bool fullScreen = false, bool useToolbar = false, UniWebViewToolbarPosition toolbarPosition = UniWebViewToolbarPosition.Top, bool useEmbeddedToolbar = false, UniWebViewToolbarPosition embeddedToolbarPosition = UniWebViewToolbarPosition.Top)
        {
            webView = webViewRect.gameObject.AddComponent<UniWebView>();
            webView.ReferenceRectTransform = webViewRect;
            webView.OnPageStarted += OnPageStarted;
            webView.OnPageFinished += OnPageFinished;
            webView.OnLoadingErrorReceived += OnLoadingErrorReceived;

            if (!string.IsNullOrEmpty(urlOnStart))
            {
                webView.urlOnStart = urlOnStart;
            }
            webView.showOnStart = showOnStart;
            webView.fullScreen = fullScreen;
#pragma warning disable CS0618
            webView.useToolbar = useToolbar;
            webView.toolbarPosition = toolbarPosition;
#pragma warning restore CS0618
            webView.useEmbeddedToolbar = useEmbeddedToolbar;
            webView.embeddedToolbarPosition = embeddedToolbarPosition;
        }

        public static void LoadUrl(string url)
        {
            webView.Load(url);
        }

        public static void ShowWebView()
        {
            webView.Show();
        }

        public static void HideWebView()
        {
            webView.Hide();
        }

        private static void OnPageStarted(UniWebView webView, string url)
        {
            Debug.Log("Page Started: " + url);
        }

        private static void OnPageFinished(UniWebView webView, int statusCode, string url)
        {
            if (statusCode == 200)
            {
                ShowWebView();
            }
            else
            {
                Debug.LogError("Failed to load URL: " + url + ", Status Code: " + statusCode);
            }
        }

        private static void OnLoadingErrorReceived(UniWebView webView, int errorCode, string errorMessage, UniWebViewNativeResultPayload payload)
        {
            Debug.LogError("Page Error with Status Code: " + errorCode + " Error: " + errorMessage);
        }
    }
}