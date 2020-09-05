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
        [Header("Write Digits")]
        public ComputeShader writeDigitCompute;

        public Texture2D[] digits;
        private ComputeBuffer[] digitsBuffer;

        public int digitWidth;
        public int digitHeight;

        public Color[] colors;
        int GetColorIndex(int last)
        {
            int i = (last + Random.Range(1, colors.Length)) % colors.Length;
            return i;
        }

        public int spacing;

        [Header("Framing")]
        public ComputeShader framingCompute;
        [Space(2f)]
        public int first;
        public int second;
        public int third;
        [Space(2f)]
        public Color firstColor;
        public Color secondColor;
        public Color thirdColor;

        float[] GetDigitToCompute(int i)
        {
            float[] digit = new float[digitWidth * digitHeight];

            int index = 0;

            for(int y=0; y<digitHeight; y++)
                for(int x =0; x<digitWidth; x++)
                {

                    digit[index] = digits[i].GetPixel(x, y).a > 0.5f ? 1 : 0;
                    index++;
                }

            return digit;
        }
        public void SetComputeData()
        {
            int count = digitWidth * digitHeight;
            int stride = sizeof(float);

            digitsBuffer = new ComputeBuffer[10]; 
            for(int i=0; i<10; i++)
            {
                digitsBuffer[i] = new ComputeBuffer(count, stride);
                digitsBuffer[i].SetData(GetDigitToCompute(i));
            }

            writeDigitCompute.SetBuffer(0, "zero", digitsBuffer[0]);
            writeDigitCompute.SetBuffer(0, "one", digitsBuffer[1]);
            writeDigitCompute.SetBuffer(0, "two", digitsBuffer[2]);
            writeDigitCompute.SetBuffer(0, "three", digitsBuffer[3]);
            writeDigitCompute.SetBuffer(0, "four", digitsBuffer[4]);
            writeDigitCompute.SetBuffer(0, "five", digitsBuffer[5]);
            writeDigitCompute.SetBuffer(0, "six", digitsBuffer[6]);
            writeDigitCompute.SetBuffer(0, "seven", digitsBuffer[7]);
            writeDigitCompute.SetBuffer(0, "eight", digitsBuffer[8]);
            writeDigitCompute.SetBuffer(0, "nine", digitsBuffer[9]);

            writeDigitCompute.SetInt("digitWidth", digitWidth);
            writeDigitCompute.SetInt("digitHeight", digitHeight);
        }
        public void ReleaseComputeData()
        {
            for (int i = 0; i < 10; i++)
            {
                digitsBuffer[i].Release();
            }
        }

        public Sprite Write(int Number)
        {
            if (Number < 0)
                Number = -Number;

            string s = Number.ToString();

            int[] digits = new int[s.Length];
            for (int i = 0; i < digits.Length; i++)
                digits[i] = s[i] - 48;

            Color[] digitColors = new Color[s.Length];
            int colorIndex = Random.Range(0, digitColors.Length);
            for (int i = 0; i < s.Length; i++) {
                digitColors[i] = colors[colorIndex];
                colorIndex = GetColorIndex(colorIndex);
            }

            int length = digits.Length;

            int width = length * (digitWidth + spacing) + third + third;
            int height = digitHeight + third + third;

            ComputeBuffer digitsBuffer = new ComputeBuffer(length, sizeof(int));
            ComputeBuffer colorsBuffer = new ComputeBuffer(length, sizeof(float) * 4);

            digitsBuffer.SetData(digits);
            colorsBuffer.SetData(digitColors);

            RenderTexture rtDigits = new RenderTexture(width, height, 32);
            rtDigits.enableRandomWrite = true;
            rtDigits.Create();

            writeDigitCompute.SetInt("width", width);
            writeDigitCompute.SetInt("height", height);
            writeDigitCompute.SetInt("frame", third);
            writeDigitCompute.SetInt("spacing", spacing);
            writeDigitCompute.SetInt("length", digits.Length);
            writeDigitCompute.SetBuffer(0, "digits", digitsBuffer);
            writeDigitCompute.SetBuffer(0, "colors", colorsBuffer);
            writeDigitCompute.SetTexture(0, "Result", rtDigits);

            int X = Mathf.CeilToInt(width / 8f);
            int Y = Mathf.CeilToInt(height / 8f);

            writeDigitCompute.Dispatch(0, X, Y, 1);

            RenderTexture rtFull = new RenderTexture(width, height, 32);
            rtFull.enableRandomWrite = true;
            rtFull.Create();

            framingCompute.SetInt("width", width);
            framingCompute.SetInt("height", height);

            framingCompute.SetFloat("first", first);
            framingCompute.SetFloat("second", second);
            framingCompute.SetFloat("third", third);

            framingCompute.SetVector("firstColor", firstColor);
            framingCompute.SetVector("secondColor", secondColor);
            framingCompute.SetVector("thirdColor", thirdColor);

            framingCompute.SetTexture(0, "Input", rtDigits);
            framingCompute.SetTexture(0, "Result", rtFull);

            framingCompute.Dispatch(0, X, Y, 1);

            RenderTexture active = RenderTexture.active;
            RenderTexture.active = rtFull;
            Texture2D t = new Texture2D(width, height);
            t.ReadPixels(new Rect(0, 0, rtFull.width, rtFull.height), 0, 0);
            t.Apply();
            RenderTexture.active = active;

            digitsBuffer.Release();
            colorsBuffer.Release();
            rtDigits.Release();
            rtFull.Release();

            return Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        }
    }
}
