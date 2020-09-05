using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

namespace UnityEngine.UI.Counter
{
    [CreateAssetMenu(fileName = "New UI Counter Font", menuName = "UI.Counter/Font")]
    public class Font : ScriptableObject
    {
        public Texture2D[] digits;
        Texture2D GetTexture(char c, Color color)
        {
            Texture2D t = digits[c - 48];

            for (int x = 0; x < t.width; x++)
                for (int y = 0; y < t.height; y++)
                {
                    Color col = t.GetPixel(x, y);
                    if (col.a > 0.5f)
                        t.SetPixel(x, y, color);
                }

            return t;
        }
        [Space(5f)]
        public Color[] colors;
        int GetColorIndex(int last)
        {
            int i = (last + Random.Range(1, colors.Length)) % colors.Length;
            return i;
        }
        [Space(5f)]
        public int spacing;
        [Space(5f)]
        public ComputeShader framingCompute;
        [Space(2f)]
        public int first;
        public int second;
        public int third;
        [Space(2f)]
        public Color firstColor;
        public Color secondColor;
        public Color thirdColor;


        public Sprite Write(int number)
        {
            if (number < 0)
                number = -number;

            string s = number.ToString();

            Texture2D[] chars = new Texture2D[s.Length];

            int width = spacing + 2 * third;
            int height = digits[0].height + 2 * third;

            int colorIndex = Random.Range(0, colors.Length);

            for(int i=0; i<s.Length; i++)
            {
                colorIndex = GetColorIndex(colorIndex);
                chars[i] = GetTexture(s[i], colors[colorIndex]);
                width += spacing + chars[i].width;
            }

            Texture2D t = new Texture2D(width, height);
            int x = third;

            Color[] space = new Color[width * height];
            for (int i = 0; i < space.Length; i++)
                space[i] = new Color(0, 0, 0, 0);
            t.SetPixels(space);

            space = new Color[spacing * height];
            for (int i = 0; i < space.Length; i++)
            {
                space[i] = new Color(0, 0, 0, 0);
            }

            t.SetPixels(x, 0, spacing, height, space);
            x += spacing;

            for (int i = 0; i < chars.Length; i++)
            {
                Color[] cs = chars[i].GetPixels();

                t.SetPixels(x, third, chars[i].width, chars[i].height, cs);
                x += chars[i].width;

                t.SetPixels(x, 0, spacing, height, space);
                x += spacing;
            }
            t.Apply();

            RenderTexture rt = new RenderTexture(t.width, t.height, 32);
            rt.enableRandomWrite = true;
            rt.Create();

            framingCompute.SetInt("width", t.width);
            framingCompute.SetInt("height", t.height);

            framingCompute.SetFloat("first", first);
            framingCompute.SetFloat("second", second);
            framingCompute.SetFloat("third", third);

            framingCompute.SetVector("firstColor", firstColor);
            framingCompute.SetVector("secondColor", secondColor);
            framingCompute.SetVector("thirdColor", thirdColor);

            framingCompute.SetTexture(0, "Input", t);
            framingCompute.SetTexture(0, "Result", rt);

            int X = Mathf.CeilToInt(t.width / 8f);
            int Y = Mathf.CeilToInt(t.height / 8f);

            framingCompute.Dispatch(0, X, Y, 1);

            RenderTexture.active = rt;
            t.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            t.Apply();

            return Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        }
    }
}
