using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

public class KHHUIColorPicker : MonoBehaviour
{
    //[SerializeField] Button[] colorButtons;
    [SerializeField] ColorPickerControl colorPicker;
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

        colorPicker.CurrentColor = color;
        colorPicker.onValueChanged.AddListener((c) => OnChangeColor?.Invoke(c));
        //foreach (var b in colorButtons)
        //    b.onClick.RemoveAllListeners();

        m_Color = color;
        OnChangeColor = onChangeColor;

        gameObject.SetActive(true);
    }

    public void Close()
    {
        OnChangeColor = null;
        gameObject.SetActive(false);
    }

    //private void OnColorButtonClick(int idx)
    //{
    //    m_Color = colorButtons[idx].image.color;
    //    OnChangeColor?.Invoke(m_Color);
    //}

    private void OnClickClose()
    {
        this.Close();
    }

    //private void OnValidate()
    //{
    //    for (int i = 0; i < colorButtons.Length; i++)
    //        if (colorButtons[i].onClick.GetPersistentEventCount() == 0)
    //            UnityEventTools.AddIntPersistentListener(colorButtons[i].onClick, OnColorButtonClick, i);
    //}
}
