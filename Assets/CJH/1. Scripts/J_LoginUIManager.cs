using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class J_LoginUIManager : MonoBehaviour
{
    public List<GameObject> panels = new List<GameObject>(); //패널 오브젝트 이름 배열
    private int page = 0;
    private bool isReady = false;
    [SerializeField] private Transform panelTransform;

    [Header("Title")]
    public TMP_Text introductionText;//소개 text
    public Button move_startBt; //이동 버튼


    [Header("Popup_Login")]
    public GameObject PopUp_Login; //로그인 창
    public Button move_SignUpBt; //이동 버튼
    public Button close_Bt1; //닫기 버튼

    [Header("Popup_ SignUp")]
    public GameObject PopUp_signUp; //회원가입 창
    public Button prev_LoginBt; //로그인 버튼
    public Button signUpBt; //회원가입 버튼
    public Button close_Bt2; //닫기 버튼


    [Header("Popup_ CheckSignUp")]
    public GameObject PopUp_checkSignUp; //회원가입 성공 창
    public Button prev_SignUp;
    public Button close_Bt3; //닫기 버튼

    Action onChange;
    // Start is called before the first frame update
    void Start()
    {
        IntroductionText();
        //이전
        prev_LoginBt.onClick.AddListener(Click_Prev);
        prev_SignUp.onClick.AddListener(Click_Prev);
        close_Bt1.onClick.AddListener(Click_Prev);
        close_Bt2.onClick.AddListener(Click_Prev);
        close_Bt3.onClick.AddListener(Click_Prev);

        //다음
        move_startBt.onClick.AddListener(Click_Next);
        move_SignUpBt.onClick.AddListener(Click_Next);
        signUpBt.onClick.AddListener(Click_Next);
        foreach (Transform t in panelTransform)
        {
            panels.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }

        panels[page].SetActive(true);
        isReady = true;
        CheckControl();
    }

    // Update is called once per frame
    void Update()
    {
        //if(onChange != null)
        //{
        //    IntroductionText();
        //}
    }
    
    public void IntroductionText()
    {
        string IntroText = "메타 밍글은 숏폼, 인터랙티브 무비를 통한 문화 교류 커뮤니티 메타버스 플랫폼입니다.";

        introductionText.GetComponent<TMP_Text>().text = IntroText;
        IntroText = introductionText.text;

    }
    //이전으로 이동하는 버튼
    public void Click_Prev()
    {

        if (page <= 0 || !isReady) return;

        panels[page].SetActive(false);
        panels[page -= 1].SetActive(true);
        CheckControl();
    }


    //다음으로 이동하는 버튼
    public void Click_Next()
    {
        if (page >= panels.Count - 1) return;
        panels[page].SetActive(false);
        panels[page += 1].SetActive(true);
        CheckControl();
    }

    private void CheckControl()
    {
        SetArrowActive();
    }

    private void SetArrowActive()
    {
        //이전
        prev_LoginBt.gameObject.SetActive(page > 0);
        prev_SignUp.gameObject.SetActive(page > 0);
        close_Bt1.gameObject.SetActive(page > 0);
        close_Bt2.gameObject.SetActive(page > 0);
        close_Bt3.gameObject.SetActive(page > 0);
        //다음
        move_startBt.gameObject.SetActive(page < panels.Count - 1);
        move_SignUpBt.gameObject.SetActive(page < panels.Count - 1);
        signUpBt.gameObject.SetActive(page < panels.Count - 1);
    }
}
