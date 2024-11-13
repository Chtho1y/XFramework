using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using XEngine.UI;


namespace XEngine.Engine
{
    public static class HttpUtil
    {
        public static string HttpGet(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "text/html;charset=UTF-8";
            string result;
            try
            {
                Stream responseStream = ((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                result = text;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 跳转外部URL
        /// </summary>
        /// <param name="URL"></param>
        public static void Jump2Browser(string URL)
        {
            var webRequest = new UnityWebRequest(URL)
            {
                certificateHandler = new UnityWebRequestCertify()
            };
            Application.OpenURL(webRequest.url);
        }

        /// <summary>
        /// 从URL加载一张图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="img"></param>
        /// <param name="fitSize"></param>
        public static void DownloadAndSaveTextureThenLoadImg(string url, Image img, string fileName, bool fitSize = false)
        {
            if (url == String.Empty) return;
            GameManager.Instance.StartCoroutine(HttpUtilHelper.Instance.DownloadAndSaveTextureThenLoadImg(url, img, fileName, fitSize));
        }

        /// <summary>
        /// 下载并存储一张图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        public static void DownloadAndSaveTexture(string url, string fileName)
        {
            GameManager.Instance.StartCoroutine(HttpUtilHelper.Instance.DownloadAndSaveTexture(url, fileName));
        }
    }

    public class HttpUtilHelper : Singleton<HttpUtilHelper>
    {
        public IEnumerator DownloadAndSaveTexture(string url, string fileName)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);

            // 如果文件已经存在，就不下载
            if (File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.Log("File already exists at: " + path);
#endif
                yield break;
            }

            using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
            webRequest.certificateHandler = new UnityWebRequestCertify();
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                // 将纹理保存为 PNG or JPG
                byte[] bytes = texture.EncodeToPNG(); // .EncodeToJPG()
                File.WriteAllBytes(path, bytes);
            }
        }

        public IEnumerator DownloadAndSaveTextureThenLoadImg(string url, Image img, string fileName, bool fitSize)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);

            // 如果文件已经存在，就不下载
            if (File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.Log("File already exists at: " + path);
#endif
                // 直接从文件加载
                Texture2D texture = UIUtil.LoadTextureFromPersistentData(fileName);

                UIUtil.ImgFitSize(img, texture, fitSize);
                UnityEngine.Object.Destroy(texture);
                yield break;
            }

            using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
            webRequest.certificateHandler = new UnityWebRequestCertify();
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(path, bytes);

                UIUtil.ImgFitSize(img, texture, fitSize);

                UnityEngine.Object.Destroy(texture);
            }
        }
    }

    public class UnityWebRequestCertify : UnityEngine.Networking.CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //return base.ValidateCertificate(certificateData);
            return true;
        }
    }
}