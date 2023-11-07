using System.Collections;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{
    public class KHHUIColorPicker : MonoBehaviour
    {
        [SerializeField] Button[] colorButtons;
        [Space]
        [SerializeField] private Button m_CloseButton;

        public System.Action<Color> OnChangeColor;

        private Color m_Color;

        private void Awake()
        {
            m_CloseButton.onClick.AddListener(OnClickClose);
        }

        public void Show(Color color, System.Action<Color> onChangeColor)
        {
            if (gameObject.activeSelf)
                Close();

            foreach (var b in colorButtons)
                b.onClick.RemoveAllListeners();

            m_Color = color;
            OnChangeColor = onChangeColor;


            gameObject.SetActive(true);
        }

        public void Close()
        {
            OnChangeColor = null;
            gameObject.SetActive(false);
        }

        private void OnColorButtonClick(int idx)
        {
            m_Color = colorButtons[idx].image.color;
            OnChangeColor?.Invoke(m_Color);
        }

        private void OnClickClose()
        {
            this.Close();
        }

        private void OnValidate()
        {
            for (int i = 0; i < colorButtons.Length; i++)
                UnityEventTools.AddIntPersistentListener(colorButtons[i].onClick, OnColorButtonClick, i);
        }
    }
}
