using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class J_LoginUIManager : MonoBehaviour
{
    public List<GameObject> panels = new List<GameObject>(); //패널 오브젝트 이름 배열
    private int page = 0;
    private bool isReady = false;
    private bool canContinue = false;
    EventSystem system;

    [SerializeField] private Transform panelTransform;

    [Header("Title")]
    public TMP_Text introductionText;//소개 text
    public Button move_startBt; //이동 버튼


    [Header("Popup_Login")]
    public GameObject PopUp_Login; //로그인 창
    public Button move_SignUpBt; //이동 버튼
    public Button close_Bt1; //닫기 버튼

    public Button loginBt; //로그인 버튼
    public TMP_InputField inputId; //로그인 아이디 입력
    public TMP_InputField inputPW; //로그인 패스워드 입력
    public Selectable firstInput;

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
        prev_SignUp.onClick.AddListener(OnChange);
        close_Bt1.onClick.AddListener(Click_Prev);
        close_Bt2.onClick.AddListener(Click_Prev);
        close_Bt3.onClick.AddListener(OnChange);

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
        system = EventSystem.current;

        Login_Bt();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeInput();
    }

    public void IntroductionText()
    {
        string IntroText = "메타 밍글은 숏폼, 인터랙티브 무비를 통한 문화 교류 커뮤니티 메타버스 플랫폼입니다.";

        introductionText.GetComponent<TMP_Text>().text = IntroText;
        IntroText = introductionText.text;

        //if (Input.anyKeyDown)
        //{
        //    canContinue = true;
        //    move_startBt.onClick.Invoke();
        //    Debug.Log("Button pressed!");
        //}


    }
    //이전으로 이동하는 버튼
    public void Click_Prev()
    {

        if (page <= 0 || !isReady) return;

        panels[page].SetActive(false);
        panels[page -= 1].SetActive(true);
        CheckControl();
    }

    //2칸 뒤로 이동
    public void OnChange()
    {
        if (page <= 0 || !isReady) return;
        panels[page].SetActive(false);
        panels[page -= 2].SetActive(true);
        CheckControl();
    }
    //자신 비활성화
    public void OnClose()
    {

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
    //현재 로그인 포스트 통신 함수 => 추후 함수 이름 변경
    public void PostTest()
    {
        //string text = " {\r\n    \"apiStatus\": \"SUCCESS\",\r\n    \"message\": \"성공적으로 로그인되었습니다.\",\r\n    \"data\": {\r\n        \"token\": \"eyJhbGciOiJIUzI1NiJ9.eyJyb2x8c\"\r\n    }}";
        //SignInInfo signInInfo = JsonUtility.FromJson<SignInInfo>(text);
        //HttpManager.Get().token = signInInfo.data.token;


        HttpInfo info = new HttpInfo();
        info.Set(RequestType.POST, "/member/login", (DownloadHandler downloadHandler) => {
            //Post 데이터 전송했을 때 서버로부터 응답온다
            Debug.Log("Signup : " + downloadHandler.text);

            SignInInfo signInInfo = JsonUtility.FromJson<SignInInfo>(downloadHandler.text);
            HttpManager.Get().token = signInInfo.data.token;


            //여기서 씬이동
            print("커스터마이징씬이동");
            
        });
        SignUpInfo signUpInfo = new SignUpInfo();
        signUpInfo.email = inputId.text;
        signUpInfo.password = inputPW.text;

        info.body = JsonUtility.ToJson(signUpInfo);

        HttpManager.Get().SendRequest(info);



    }
    //로그인 버튼
    public void Login_Bt()
    {
        if (loginBt.onClick != null)
        {
            loginBt.onClick.AddListener(PostTest);
        }

    }



    //탭키로 인풋필드 이동 및 엔터버튼으로 로그인하기 
    public void ChangeInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            loginBt.onClick.Invoke();
            Debug.Log("Button pressed!");
        }
    }


}
