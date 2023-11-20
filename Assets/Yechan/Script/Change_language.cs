using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Change_language : MonoBehaviour
{

    public Button myButton; // 버튼 변수
    public TextMeshProUGUI buttontext;

    // 버튼 클릭 시 호출되는 함수
    public bool iskorea=true;
    string selected_language;
    public void OnButtonClick()
    {
        iskorea = !iskorea;
        Debug.Log(iskorea);

        if (iskorea)
        {
            selected_language = "한국어";
        }
        else
        {
            selected_language = "english";
        }

        // 버튼 내부의 Text 컴포넌트 가져오기

        
        buttontext = myButton.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(buttontext);
        // Text 컴포넌트가 유효한지 확인하고 텍스트 값을 변경합니다.
        if (buttontext != null)
        {
            buttontext.text = $"[{selected_language}]로 변경";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
