using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Counter
{
    public class Counter : MonoBehaviour
    {
        public Font font;
        public RectTransform rt;
        public Image image;

        public void Write(int number)
        {
            Sprite s = font.Write(number);
            rt.sizeDelta = new Vector2(rt.sizeDelta.y * s.rect.width / s.rect.height, rt.sizeDelta.y);

            image.sprite = font.Write(number);
        }

        public void Start()
        {
            Write(1234567890);
        }
    }
}
