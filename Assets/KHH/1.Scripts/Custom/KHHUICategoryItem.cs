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
    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] private TextMeshProUGUI m_CategoryTitle;
    [SerializeField] private TextMeshProUGUI m_EquipedItemTitle;
    [SerializeField] private Button m_Button;

    public System.Action OnClick;

    Color colorOff = new Color32(217, 217, 217, 255);
    Color colorOn = new Color32(18, 24, 29, 255);

    public string Title
    {
        get => m_CategoryTitle.text;
        set
        {
            m_CategoryTitle.text = value;
            switch (value)
            {
                case "body":
                    icon.sprite = iconSprites[0];
                    break;
                case "head":
                    icon.sprite = iconSprites[1];
                    break;
                case "hairstyle":
                    icon.sprite = iconSprites[2];
                    break;
                case "top":
                    icon.sprite = iconSprites[3];
                    break;
                case "bottom":
                    icon.sprite = iconSprites[4];
                    break;
                case "shoes":
                    icon.sprite = iconSprites[5];
                    break;
                case "outfit":
                    icon.sprite = iconSprites[6];
                    break;
            }
        }
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