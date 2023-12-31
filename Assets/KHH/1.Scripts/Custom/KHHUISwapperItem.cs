﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KHHUISwapperItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private Button m_LeftButton;
    [SerializeField] private Button m_RightButton;

    public string Text
    {
        get => m_Text.text;
        set => m_Text.text = value;
    }
    public System.Action OnClickLeft, OnClickRight;

    private void Awake()
    {
        m_LeftButton.onClick.AddListener(_OnClickLeftButton);
        m_RightButton.onClick.AddListener(_OnClickRightButton);
    }

    private void _OnClickLeftButton()
    {
        OnClickLeft?.Invoke();
    }

    private void _OnClickRightButton()
    {
        OnClickRight?.Invoke();
    }
}
