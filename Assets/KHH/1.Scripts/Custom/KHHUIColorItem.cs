using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KHHUIColorItem : MonoBehaviour
{
    [SerializeField] private Image m_RightImage;
    [SerializeField] private Button m_RightButton;

    public System.Action OnClickLight;

    public int Index { get; set; }

    public Color LightColor
    {
        get => m_RightImage.color;
        set => m_RightImage.color = value;
    }

    private void Awake()
    {
        m_RightButton.onClick.AddListener(_OnClickLight);
    }

    private void _OnClickLight()
    {
        OnClickLight?.Invoke();
    }
}
