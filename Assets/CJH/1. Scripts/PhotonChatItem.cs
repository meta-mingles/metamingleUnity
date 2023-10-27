using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonChatItem : MonoBehaviour
{
    TMP_Text myText;
    RectTransform rt;
    // Start is called before the first frame update
    void Awake()
    {
        myText = GetComponent<TMP_Text>();
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetText(string chat, Color color)
    {
        myText.text = chat;

        myText.color = color;

        //text의 height를 적정크기로 설정
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, myText.preferredHeight);
    }
}
