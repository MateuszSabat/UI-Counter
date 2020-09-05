using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Counter
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class Counter : MonoBehaviour
    {
        public Font font;
        private RectTransform rt;
        private Image image;

        public void Start()
        {
            font.SetComputeData();

            rt = GetComponent<RectTransform>();
            image = GetComponent<Image>();

            Write(1234567890);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Write(Random.Range(0, int.MaxValue));
            }
        }

        public void OnApplicationQuit()
        {
            font.ReleaseComputeData();
        }

        public void Write(int number)
        {
            Sprite s = font.Write(number);
            rt.sizeDelta = new Vector2(rt.sizeDelta.y * s.rect.width / s.rect.height, rt.sizeDelta.y);

            image.sprite = font.Write(number);
        }
    }
}
