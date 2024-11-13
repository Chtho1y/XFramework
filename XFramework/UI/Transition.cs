// Copyright (C) 2024 ricimi. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at https://unity.com/legal/as-terms.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


namespace XEngine.UI
{
    // This class is responsible for managing the transitions between scenes that are performed
    // in the demo via a classic fade.
    public class Transition : MonoBehaviour
    {
        private static GameObject m_canvas;

        private GameObject m_overlay;

        private void Awake()
        {
            // Create a new, ad-hoc canvas that is not destroyed after loading the new scene
            // to more easily handle the fading code.
            m_canvas = new GameObject("TransitionCanvas");
            var canvas = m_canvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            DontDestroyOnLoad(m_canvas);
        }

        public static void LoadLevel(float duration, Color color, string level = null)
        {
            var fade = new GameObject("Transition");
            fade.AddComponent<Transition>();
            fade.GetComponent<Transition>().StartFade(duration, color, level);
            fade.transform.SetParent(m_canvas.transform, false);
            fade.transform.SetAsLastSibling();
        }

        private void StartFade(float duration, Color fadeColor, string level)
        {
            StartCoroutine(RunFade(duration, fadeColor, level));
        }

        // This coroutine performs the core work of fading out of the current scene
        // and into the new scene.
        private IEnumerator RunFade(float duration, Color fadeColor, string level)
        {
            var bgTex = new Texture2D(1, 1);
            bgTex.SetPixel(0, 0, fadeColor);
            bgTex.Apply();

            m_overlay = new GameObject();
            var image = m_overlay.AddComponent<Image>();
            var rect = new Rect(0, 0, bgTex.width, bgTex.height);
            var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
            image.material.mainTexture = bgTex;
            image.sprite = sprite;
            var newColor = image.color;
            image.color = newColor;
            image.canvasRenderer.SetAlpha(0.0f);

            m_overlay.transform.localScale = new Vector3(1, 1, 1);
            m_overlay.GetComponent<RectTransform>().sizeDelta = m_canvas.GetComponent<RectTransform>().sizeDelta;
            m_overlay.transform.SetParent(m_canvas.transform, false);
            m_overlay.transform.SetAsFirstSibling();

            var time = 0.0f;
            var halfDuration = duration / 2.0f;
            while (time < halfDuration)
            {
                time += Time.deltaTime;
                image.canvasRenderer.SetAlpha(Mathf.InverseLerp(0, 1, time / halfDuration));
                yield return new WaitForEndOfFrame();
            }

            image.canvasRenderer.SetAlpha(1.0f);
            yield return new WaitForEndOfFrame();

            if (!String.IsNullOrEmpty(level)) SceneManager.LoadScene(level);

            time = 0.0f;
            while (time < halfDuration)
            {
                time += Time.deltaTime;
                image.canvasRenderer.SetAlpha(Mathf.InverseLerp(1, 0, time / halfDuration));
                yield return new WaitForEndOfFrame();
            }

            image.canvasRenderer.SetAlpha(0.0f);
            yield return new WaitForEndOfFrame();

            Destroy(m_canvas);
        }
    }
}
