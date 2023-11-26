using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonChatItem : MonoBehaviour
{
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI myText;
    public Button translationBt;
    RectTransform rt;
    // Start is called before the first frame update
    void Awake()
    {
        translationBt.onClick.AddListener(Translate);
        rt = GetComponent<RectTransform>();
    }
    public void SetText(string nickname, string chat, Color color)
    {
        nicknameText.text = nickname;
        myText.text = chat;
        myText.color = color;

        //text의 height를 적정크기로 설정
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, myText.preferredHeight);
    }
    public async void Translate()
    {
        string currentLanguage = await TranslationManager.instance.DetectLanguage(myText.text);
        string targetLanguage = currentLanguage == "EN" ? "KO" : "EN";
        string translatedText = await TranslationManager.instance.TranslateText(myText.text, targetLanguage);
        myText.text = translatedText;
    }

}
