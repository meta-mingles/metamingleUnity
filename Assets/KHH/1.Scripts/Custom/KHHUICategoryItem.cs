using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KHHUICategoryItem : MonoBehaviour
{
    [SerializeField] private GameObject imageOff;
    [SerializeField] private GameObject imageOn;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI m_CategoryTitle;
    [SerializeField] private TextMeshProUGUI m_EquipedItemTitle;
    [SerializeField] private Button m_Button;

    public System.Action OnClick;

    Color colorOff = new Color32(217, 217, 217, 255);
    Color colorOn = new Color32(18, 24, 29, 255);

    public string Title
    {
        get => m_CategoryTitle.text;
        set => m_CategoryTitle.text = value;
    }

    public string Value
    {
        get => m_EquipedItemTitle.text;
        set => m_EquipedItemTitle.text = value;
    }

    private void Awake()
    {
        m_Button.onClick.AddListener(Callback_OnClick);
    }

    private void Callback_OnClick()
    {
        OnClick?.Invoke();

        imageOff.SetActive(false);
        imageOn.SetActive(true);
        icon.color = colorOn;
        m_CategoryTitle.color = colorOn;
        m_EquipedItemTitle.color = colorOn;
    }

    public void SetOff()
    {
        imageOff.SetActive(true);
        imageOn.SetActive(false);
        icon.color = colorOff;
        m_CategoryTitle.color = colorOff;
        m_EquipedItemTitle.color = colorOff;
    }
}