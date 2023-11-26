using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using WebSocketSharp;

public class PhotonChatItem : MonoBehaviour
{
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI myText;
    public TextMeshProUGUI outputText;
    public Button translationBt;

    private bool isTraslate = false;
    RectTransform rt;

    public Test_deepl translator;

    // Start is called before the first frame update
    void Awake()
    {
        translator = FindObjectOfType<Test_deepl>();
        if (translator == null)
        {
            Debug.LogError("Test_deepl instance not found in the scene.");
        }
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
    async void Translate()
    {
        //원본언어상태
        if (!isTraslate)
        {
            //번역을 한적이 없다
            if (string.IsNullOrEmpty(outputText.text))
            {
                if (Test_deepl.Instance != null)
                {
                    isTraslate = true;
                    string currentLanguage = DetermineLanguage(myText.text);

                    if (currentLanguage == "ko")
                    {
                        string translatedText = await translator.StartTranslation(myText.text, "EN-GB");
                        outputText.text = translatedText;
                        myText.gameObject.SetActive(false);
                        outputText.gameObject.SetActive(true);

                    }
                    else if (currentLanguage == "en")
                    {
                        string translatedText = await translator.StartTranslation(myText.text, "ko");
                        outputText.text = translatedText;
                        myText.gameObject.SetActive(false);
                        outputText.gameObject.SetActive(true);
                    }
                }
                else
                {
                    Debug.Log("번역 실행안됌");
                }
            }
            //번역을 한적이 있다.
            else
            {
                myText.gameObject.SetActive(false);
                outputText.gameObject.SetActive(true);
                isTraslate = true;
            }
        }
        //번역 언어 상태
        else
        {
            //번역 이전의 값으로 돌아간다
            myText.gameObject.SetActive(true);
            outputText.gameObject.SetActive(false);
            isTraslate = false;
        }
    }
    private void ReturnTranslate()
    {

    }


    private string DetermineLanguage(string text)
    {
        foreach(char c in text)
        {
            if(char.GetUnicodeCategory(c) == UnicodeCategory.OtherLetter)
            {
                return "ko";
            }

        }
        return "en";
    }
}
