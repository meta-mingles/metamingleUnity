using DG.Tweening;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class J_LoginUIManager : MonoBehaviour
{
    private int page = 0;
    private bool isReady = false;
    private bool canContinue = false;
    private bool isRemember = false;
    EventSystem system;

    [Header("Title")]
    [SerializeField] private CanvasGroup titleCG; //타이틀
    [SerializeField] private Transform titleLogo; //타이틀 로고
    //public TMP_Text introductionText;//소개 text
    [SerializeField] private Button move_startBt; //이동 버튼

    [Header("Window")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup windowCG;
    List<GameObject> pages = new List<GameObject>(); //패널 오브젝트 이름 배열

    [Header("Popup_Login")]
    public GameObject PopUp_Login; //로그인 창
    public Button move_SignUpBt; //이동 버튼
    //public Button close_Bt1; //닫기 버튼

    public Button loginBt; //로그인 버튼
    public TMP_InputField inputId; //로그인 아이디 입력
    public TMP_InputField inputPW; //로그인 패스워드 입력
    public Toggle rememberMeToggle; // 로그인 정보 저장

    [Header("Popup_ SignUp")]
    public GameObject PopUp_signUp; //회원가입 창
    public Button signUpBt; //회원가입 버튼
    public Button prev_LoginBt; //로그인 버튼
    //public Button close_Bt2; //닫기 버튼
    public TMP_InputField inputId2; //로그인 아이디 입력
    public TMP_InputField inputNickName; //로그인 닉네임 입력
    public TMP_InputField inputPW2; //로그인 패스워드 입력

    [Header("LoginFail")]
    public GameObject loginFail;
    public Transform loginFailPopUp;
    public Button loginFailCloseButton; //닫기 버튼

    [Header("CheckSignUp")]
    public GameObject checkSignUp; //회원가입창
    public Transform checkSignUpPopUp;
    public Button checkSignUpCloseButton; //닫기 버튼

    Action onChange;
    //string customizationSceneName = "Customization";
    // Start is called before the first frame update
    void Start()
    {
        Init();
        //이전
        if (prev_LoginBt != null) prev_LoginBt.onClick.AddListener(Click_Prev);
        if (loginFailCloseButton != null) loginFailCloseButton.onClick.AddListener(LoginFailClose);
        if (checkSignUpCloseButton != null) checkSignUpCloseButton.onClick.AddListener(CheckSignUpClose);

        //다음
        if (move_startBt != null) move_startBt.onClick.AddListener(Click_Start);
        if (move_SignUpBt != null) move_SignUpBt.onClick.AddListener(Click_Next);
        if (signUpBt != null) signUpBt.onClick.AddListener(Click_Next);
        foreach (Transform t in windowCG.transform)
        {
            pages.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }
        pages[page].SetActive(true);
        isReady = true;
        system = EventSystem.current;
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

        Login_Bt();
        SigUp_Bt();
        //사운드매니저로 브금 실행
        if (!SceneManager.GetActiveScene().name.Contains("Tool"))
            SoundManager.instance.PlayBGM("Bgm");
    }

    // Update is called once per frame
    void Update()
    {
        ChangeInput();
    }
    void Init()
    {
        titleCG.gameObject.SetActive(true);
        titleCG.transform.DOMoveY(0, 0.5f).SetEase(Ease.Linear);
        titleCG.DOFade(1, 0.5f).SetEase(Ease.Linear).OnComplete(() => titleCG.blocksRaycasts = true);
        titleLogo.DOScale(Vector3.one * 0.7f, 0.1f).SetDelay(0.4f).SetEase(Ease.Linear).OnComplete(() =>
        { titleLogo.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutElastic); });
    }
    void Click_Start()
    {
        panel.SetActive(true);
        windowCG.transform.DOMoveY(0, 0.5f).SetEase(Ease.Linear);
        windowCG.DOFade(1, 0.5f).SetEase(Ease.Linear).OnComplete(() => windowCG.blocksRaycasts = true);
    }
    //이전으로 이동하는 버튼
    public void Click_Prev()
    {
        if (page <= 0 || !isReady) return;
        pages[page].SetActive(false);
        pages[page -= 1].SetActive(true);
    }

    ////2칸 뒤로 이동
    //public void OnChange()
    //{
    //    if (page <= 0 || !isReady) return;
    //    pages[page].SetActive(false);
    //    pages[page -= 2].SetActive(true);
    //}

    void LoginFailClose()
    {
        loginFailPopUp.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            loginFail.SetActive(false);
        });
    }

    void CheckSignUpClose()
    {
        checkSignUpPopUp.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            checkSignUp.SetActive(false);
            Click_Prev();
        });
    }

    //다음으로 이동하는 버튼
    public void Click_Next()
    {
        if (page >= pages.Count - 1) return;
        pages[page].SetActive(false);
        pages[page += 1].SetActive(true);
    }
    //현재 로그인 포스트 통신 함수 
    public void LoginPost()
    {
        HttpInfo info = new HttpInfo();
        info.Set(RequestType.POST, "/member/login", async (DownloadHandler downloadHandler) =>
        {
            PlayerPrefs.SetString("email", inputId.text);
            PlayerPrefs.SetString("pw", inputPW.text);
            //Post 데이터 전송했을 때 서버로부터 응답온다
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
                await KHHUserCustom.LoadData();
                if (KHHUserCustom.HasData)
                {
                    prevSceneName = SceneManager.GetActiveScene().name;
                    nextSceneName = "Main_Platform";
                }
                else
                {
                    prevSceneName = "Main_Platform";    //커스텀 존재 유무 확인 필요
                    nextSceneName = "Customization";    //커스텀 존재 유무 확인 필요
                }
                KHHPhotonInit.instance.Init(prevSceneName, nextSceneName, HttpManager.instance.nickname);
            }
        }, () =>
        {
            loginFail.SetActive(true);
            loginFailPopUp.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
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
            JObject jObject = JObject.Parse(downloadHandler.text);
            JObject data = jObject["data"].ToObject<JObject>();
            HttpManager.instance.email = data["email"].ToObject<string>();
            HttpManager.instance.nickname = data["nickname"].ToObject<string>();

            checkSignUp.SetActive(true);
            checkSignUpPopUp.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
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
        }
    }

    public void OnToggleValueChanged(bool value)
    {
        //toggle버튼 isOn일 때 
        if (value)
        {
            PlayerPrefs.SetInt("IsRemember", 1);
        }
        else
        {
            PlayerPrefs.SetInt("IsRemember", 0);
        }
    }
}
