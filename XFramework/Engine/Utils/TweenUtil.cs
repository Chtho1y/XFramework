using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;


namespace XEngine.Engine
{
    public static class TweenUtil
    {
        public static Tweener DoMove(GameObject node, Vector3 target, float duration, bool snapping = false)
        {
            return node.transform.DOMove(target, duration, snapping);
        }

        public static Tweener DoLocalMove(GameObject node, Vector3 target, float duration, bool snapping = false)
        {
            return node.transform.DOLocalMove(target, duration, snapping);
        }

        public static Tweener DoRotate(GameObject node, Vector3 target, float duration, RotateMode mode = RotateMode.Fast)
        {
            return node.transform.DORotate(target, duration, mode);
        }

        public static Tweener DoLocalRotate(GameObject node, Vector3 target, float duration, RotateMode mode = RotateMode.Fast)
        {
            return node.transform.DOLocalRotate(target, duration, mode);
        }

        public static Tweener DoScale(GameObject node, Vector3 target, float duration)
        {
            return node.transform.DOScale(target, duration);
        }

        public static Tweener DoFade(Graphic node, float target, float duration)
        {
            return DOTween.ToAlpha(() => node.color, x => node.color = x, target, duration).SetTarget(node);
        }

        public static Tweener DoFade(CanvasGroup node, float target, float duration)
        {
            return DOTween.To(() => node.alpha, x => node.alpha = x, target, duration).SetTarget(node);
        }

        public static Tweener DoColor(Graphic node, Color target, float duration)
        {
            return DOTween.To(() => node.color, x => node.color = x, target, duration).SetTarget(node);
        }

        public static Tweener DoComplete(Tweener tweener, TweenCallback handler)
        {
            return tweener.OnComplete(handler);
        }

        public static Tweener DoFill(Image node, float target, float duration)
        {
            if (target > 1)
                target = 1;
            else if (target < 0)
                target = 0;
            return DOTween.To(() => node.fillAmount, x => node.fillAmount = x, target, duration).SetTarget(node);
        }

        public static Tweener DoText(Text node, string target, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
        {
            return DOTween.To(() => node.text, x => node.text = x, target, duration).SetOptions(richTextEnabled, scrambleMode, scrambleChars).SetTarget(node);
        }

        public static Tweener DOValue(Slider node, float target, float duration, bool snapping = false)
        {
            return DOTween.To(() => node.value, x => node.value = x, target, duration)
                .SetOptions(snapping).SetTarget(node);
        }
    }
}