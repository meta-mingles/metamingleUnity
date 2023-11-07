using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class J_LoginUIManager : MonoBehaviour
{
    public TMP_Text introductionText;//소개 text

    // Start is called before the first frame update
    void Start()
    {
        IntroductionText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void IntroductionText()
    {
        string IntroText = "메타 밍글은 숏폼, 인터랙티브 무비를 통한 문화 교류 커뮤니티 메타버스 플랫폼입니다.";

        introductionText.GetComponent<TMP_Text>().text = IntroText;
        IntroText = introductionText.text;

    }

}
