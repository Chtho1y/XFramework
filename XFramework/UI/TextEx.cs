using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace XEngine.UI
{
    [RequireComponent(typeof(Text)), DisallowMultipleComponent]
    public class TextEx : BaseMeshEffect
    {
        [Header("是否使用渐变色")]
        public bool useGradientColor = false;
        [Header("左上颜色,右上颜色,左下颜色,右下颜色")]
        public Color32 gradientColor1 = Color.white;
        public Color32 gradientColor2 = Color.white;
        public Color32 gradientColor3 = Color.white;
        public Color32 gradientColor4 = Color.white;

        [Header("是否开启对齐")]
        public bool useAlign = false;
        [Header("字间距")]
        public float wordSpace = 0;
        [Header("行间距")]
        public float lineSpace = 0;

        private Text text;
        private UICharInfo[] characters;
        private UILineInfo[] lines;
        private List<UIVertex> stream = new List<UIVertex>();
        private int characterCountVisible;
        private Color[] gradientColors = new Color[6];

        protected override void Awake()
        {
            text = GetComponent<Text>();
            if (text == null) return;
            text.RegisterDirtyMaterialCallback(OnFontMaterialChanged);
        }

#if UNITY_EDITOR
        protected override void OnEnable()
        {
            text = GetComponent<Text>();
            text.RegisterDirtyMaterialCallback(OnFontMaterialChanged);
        }
#endif

        private void OnFontMaterialChanged()
        {
            text.font.RequestCharactersInTexture("*", text.fontSize, text.fontStyle);
        }

        protected override void OnDestroy()
        {
            text.UnregisterDirtyMaterialCallback(OnFontMaterialChanged);
            base.OnDestroy();
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || (!useAlign && !useGradientColor)) return;
            if (text.rectTransform.rect.size.x <= 0 || text.rectTransform.rect.size.y <= 0) return;

            characters = text.cachedTextGenerator.GetCharactersArray();
            lines = text.cachedTextGenerator.GetLinesArray();
            characterCountVisible = text.cachedTextGenerator.characterCountVisible;

            vh.GetUIVertexStream(stream);
            vh.Clear();

            int streamCount = stream.Count;
            int offsetIndex = 0;

            for (int i = 0; i < streamCount; i += 6)
            {
                int charIdx = i / 6;
                float offx = 0f;
                float offy = 0f;

                if (useAlign)
                {
                    int lineIndex = GetCharInLineIndex(charIdx);
                    if (lines[lineIndex].startCharIdx == charIdx)
                    {
                        offsetIndex = 0;
                    }

                    offx = wordSpace * offsetIndex;
                    offy = lineSpace * lineIndex;

                    offsetIndex++;
                }

                for (int j = 0; j < 6; j++)
                {
                    if (useGradientColor)
                    {
                        SetGradientColors(i, j);
                    }

                    if (useAlign)
                    {
                        DoAlign(i, j, offx, offy);
                    }
                }
            }

            vh.AddUIVertexTriangleStream(stream);
        }

        private void SetGradientColors(int i, int j)
        {
            if (j < 6 && (gradientColors[0] != gradientColor1 || gradientColors[1] != gradientColor2 ||
                          gradientColors[2] != gradientColor4 || gradientColors[4] != gradientColor3 ||
                          gradientColors[5] != gradientColor1))
            {
                gradientColors[0] = gradientColor1;
                gradientColors[1] = gradientColor2;
                gradientColors[2] = gradientColor4;
                gradientColors[3] = gradientColor4;
                gradientColors[4] = gradientColor3;
                gradientColors[5] = gradientColor1;
            }

            UIVertex uiv = stream[i + j];
            uiv.color = gradientColors[j];
            stream[i + j] = uiv;
        }

        private void DoAlign(int i, int j, float offx, float offy)
        {
            UIVertex uiv = stream[i + j];
            uiv.position += new Vector3(offx, offy, 0);
            stream[i + j] = uiv;
        }

        private int GetCharInLineIndex(int charIndex)
        {
            int low = 0, high = lines.Length - 1;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                if (lines[mid].startCharIdx == charIndex)
                    return mid;
                else if (lines[mid].startCharIdx < charIndex)
                    low = mid + 1;
                else
                    high = mid - 1;
            }
            return Mathf.Max(low - 1, 0);
        }
    }
}