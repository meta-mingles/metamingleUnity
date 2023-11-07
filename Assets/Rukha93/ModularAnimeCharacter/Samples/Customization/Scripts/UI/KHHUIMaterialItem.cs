
using Rukha93.ModularAnimeCharacter.Customization.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{
    public class KHHUIMaterialItem : MonoBehaviour
    {
        [SerializeField] private Text m_Title;
        [SerializeField] private LayoutGroup m_ColorPanel;
        [SerializeField] private KHHUIColorItem m_ColorItemPrefab;

        private List<KHHUIColorItem> m_ColorItems = new List<KHHUIColorItem>();

        public KHHUIColorPicker ColorPicker { get; set; }

        public string Title
        {
            get => m_Title.text;
            set => m_Title.text = value;
        }

        private void Awake()
        {
            m_ColorItemPrefab.gameObject.SetActive(false);
        }

        public void ResetColors()
        {
            for (int i = 0; i < m_ColorItems.Count; i++)
                m_ColorItems[i].gameObject.SetActive(false);
        }

        public void SetColor(Color color, System.Action<Color> onChangeColor)
        {
            color.a = 1;

            KHHUIColorItem item = null;
            for (int i = 0; i < m_ColorItems.Count; i++)
                if (m_ColorItems[i].gameObject.activeSelf == false)
                    item = m_ColorItems[i];
            if (item == null)
            {
                item = Instantiate(m_ColorItemPrefab, m_ColorPanel.transform);
                m_ColorItems.Add(item);
            }
            item.transform.SetAsLastSibling();
            item.gameObject.SetActive(true);

            item.LightColor = color;
            item.OnClickLight = () =>
            {
                var pos = ColorPicker.transform.position;
                pos.y = item.transform.position.y;
                ColorPicker.transform.position = pos;

                ColorPicker.Show(
                item.LightColor,
                (c) =>
                {
                    item.LightColor = c;
                    onChangeColor?.Invoke(c);
                });
            };
        }
    }
}