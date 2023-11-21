using Newtonsoft.Json.Linq;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class J_LoginUIManager : MonoBehaviour
{
    public List<GameObject> panels = new List<GameObject>(); //패널 오브젝트 이름 배열
    private int page = 0;
    private bool isReady = false;
    private bool canContinue = false;
    EventSystem system;

    private bool isRemember = false;
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
    public Toggle rememberMeToggle; // 로그인 정보 저장

    [Header("Popup_ SignUp")]
    public GameObject PopUp_signUp; //회원가입 창
    public Button prev_LoginBt; //로그인 버튼
    public Button signUpBt; //회원가입 버튼
    public Button close_Bt2; //닫기 버튼
    public TMP_InputField inputId2; //로그인 아이디 입력
    public TMP_InputField inputPW2; //로그인 패스워드 입력
    public TMP_InputField inputNickName; //로그인 닉네임 입력


    [Header("Popup_ CheckSignUp")]
    public GameObject PopUp_checkSignUp; //회원가입창
    public Button prev_SignUp;
    public Button close_Bt4; //닫기 버튼
    public TMP_Text signUpMessage; //회원가입 상태
    // Start is called before the first frame update
    void Start()
    {
        IntroductionText();
        //이전
        if (prev_LoginBt != null) prev_LoginBt.onClick.AddListener(Click_Prev);
        if (prev_SignUp != null) prev_SignUp.onClick.AddListener(OnChange);
        if (close_Bt1 != null) close_Bt1.onClick.AddListener(Click_Prev);
        if (close_Bt2 != null) close_Bt2.onClick.AddListener(Click_Prev);
        if (close_Bt4 != null) close_Bt4.onClick.AddListener(OnChange);

        //다음
        if (move_startBt != null) move_startBt.onClick.AddListener(Click_Next);
        if (move_SignUpBt != null) move_SignUpBt.onClick.AddListener(Click_Next);
        if (signUpBt != null) signUpBt.onClick.AddListener(Click_Next);
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
        SigUp_Bt();


        //설정은 언제하냐 toggle bool 값이 체크 될때

        ////설정
        //PlayerPrefs.SetString("email", inputId2.text);
        //PlayerPrefs.SetString("pw", inputPW2.text);
        ////접근
        //PlayerPrefs.GetString("email");
        //PlayerPrefs.GetString("pw");




        isRemember = PlayerPrefs.GetInt("IsRemember", 0) == 1;
        rememberMeToggle.isOn = isRemember;

        rememberMeToggle.onValueChanged.AddListener(OnToggleValueChanged);



        //ison일때 playerprefs의 데이터 값을 가져온다
        if (rememberMeToggle.isOn)
        {
            //로드
           inputId.text = PlayerPrefs.GetString("email");
           inputPW.text = PlayerPrefs.GetString("pw");
        }
        else
        {
            inputId.text = "";
            inputPW.text = "";
        }


        //사운드매니저로 브금 실행
        if (!SceneManager.GetActiveScene().name.Contains("Tool"))
            SoundManager.instance.PlayBGM("Bgm");
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
        if (prev_LoginBt != null) prev_LoginBt.gameObject.SetActive(page > 0);
        if (prev_SignUp != null) prev_SignUp.gameObject.SetActive(page > 0);
        if (close_Bt1 != null) close_Bt1.gameObject.SetActive(page > 0);
        if (close_Bt2 != null) close_Bt2.gameObject.SetActive(page > 0);
        if (close_Bt4 != null) close_Bt4.gameObject.SetActive(page > 0);
        //다음
        if (move_startBt != null) move_startBt.gameObject.SetActive(page < panels.Count - 1);
        if (move_SignUpBt != null) move_SignUpBt.gameObject.SetActive(page < panels.Count - 1);
        if (signUpBt != null) signUpBt.gameObject.SetActive(page < panels.Count - 1);
    }
    //현재 로그인 포스트 통신 함수 
    public void LoginPost()
    {
        HttpInfo info = new HttpInfo();
        info.Set(RequestType.POST, "/member/login", (DownloadHandler downloadHandler) =>
        {

            PlayerPrefs.SetString("email", inputId.text);
            PlayerPrefs.SetString("pw", inputPW.text);

            //Post 데이터 전송했을 때 서버로부터 응답온다
            Debug.Log("Login : " + downloadHandler.text);
            //Netownjson
            JObject jObject = JObject.Parse(downloadHandler.text);
            JObject data = jObject["data"].ToObject<JObject>();
            HttpManager.instance.token = data["token"].ToObject<string>();
            HttpManager.instance.nickname = data["nickname"].ToObject<string>();

            string prevSceneName, nextSceneName;
            if (SceneManager.GetActiveScene().name.Contains("Tool")) //tool
            {
                prevSceneName = SceneManager.GetActiveScene().name;
                nextSceneName = "ToolSelect";

                

                //씬이동
                GlobalValue.PrevSceneName = prevSceneName;
                GlobalValue.CurSceneName = nextSceneName;
                SceneManager.LoadScene(nextSceneName);
            }
            else //platform
            {
                prevSceneName = "Main_Platform";    //커스텀 존재 유무 확인 필요
                nextSceneName = "Customization";    //커스텀 존재 유무 확인 필요
                KHHPhotonInit.instance.Init(prevSceneName, nextSceneName, HttpManager.instance.nickname);
            }

            //여기서 씬이동
            print("씬이동");
        });



        JObject jObject = new JObject();
        jObject["email"] = inputId.text;
        jObject["password"] = inputPW.text;

        info.body = jObject.ToString();

        HttpManager.instance.SendRequest(info);
    }

    //로그인 버튼
    public void Login_Bt()
    {
        if (loginBt.onClick != null)
        {
            loginBt.onClick.AddListener(LoginPost);
        }

    }
    //회원가입 포스트
    public void SignUpPost()
    {
        HttpInfo info = new HttpInfo();
        info.Set(RequestType.POST, "/member/signup", (DownloadHandler downloadHandler) =>
        {
            //Post 데이터 전송했을 때 서버로부터 응답온다
            Debug.Log("Signup : " + downloadHandler.text);


            JObject jObject = JObject.Parse(downloadHandler.text);
            JObject data = jObject["data"].ToObject<JObject>();
            HttpManager.instance.email = data["email"].ToObject<string>();
            HttpManager.instance.nickname = data["nickname"].ToObject<string>();
        });

        JObject jObject = new JObject();
        jObject["email"] = inputId2.text;
        jObject["password"] = inputPW2.text;
        jObject["nickname"] = inputNickName.text;

        info.body = jObject.ToString();
        HttpManager.instance.SendRequest(info);
    }
    //회원가입 버튼
    public void SigUp_Bt()
    {
        if (signUpBt != null)
        {
            signUpBt.onClick.AddListener(SignUpPost);
        }
    }

    public void SceneChange(string prevSceneName, string nextSceneName)
    {
    }

    //#if UNITY_EDITOR
    //    private void OnValidate()
    //    {
    //        if (loginBt.onClick.GetPersistentEventCount() == 0)
    //            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(loginBt.onClick, SceneChange, customizationSceneName);

    //    }
    //#endif



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
    public void OnToggleValueChanged(bool value)
    {
        //toggle버튼 isOn일 때 
        if (value)
        {
            PlayerPrefs.SetInt("IsRemember",1);
            //설정
        }
        else
        {
            PlayerPrefs.SetInt("IsRemember",0);

        }


    }

}
