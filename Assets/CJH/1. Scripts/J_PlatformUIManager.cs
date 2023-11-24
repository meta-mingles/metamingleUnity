using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine.Audio;
using Photon.Pun;

public class J_PlatformUIManager : MonoBehaviour
{
    public static J_PlatformUIManager Instance;

    //db로 변환하는 함수
    private float LinearToDecibel(float linear)
    {
        return linear != 0 ? 20f * Mathf.Log10(linear) : -144.0f;
    }



    private void Awake()
    {
        Instance = this;
        //초기 언어설정
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            GlobalValue.myLanguage = SystemLanguage.Korean;
        }
        else
        {
            GlobalValue.myLanguage = SystemLanguage.English;
        }
        //언어설정 초기값
        koreanBt.GetComponent<Image>().sprite = languageBox;
        englishBt.GetComponent<Image>().sprite = alpha;
        //사운드 키기
        SoundManager.instance.BGMVolume = LinearToDecibel(0.3f); // 30 %로 시작
        SoundManager.instance.PlayBGM("PlatformBGM");

        soundSlider.onValueChanged.AddListener(HandleVolumeChange);
        soundSlider.value = 0.5f; //기본값 설정
    }
    //사운드 설정
    private void HandleVolumeChange(float volume)
    {
        SoundManager.instance.BGMVolume = LinearToDecibel(volume);

    }

    [Header("Quiz")]
    public TMP_Text questionKorean; //한국질문
    public TMP_Text questionEnglish;//영어질문
    [Header("Platform")]
    public Button videoBt; //영상보러가기 버튼
    public Button customizeBt; //아바타 커스터마이징 가기 버튼
    public Button chatBt; //채팅 버튼
    [Header("Setting")]
    public Button settingBt; //설정 버튼
    public GameObject settingTab; //설정창
    public Button closeBt; //닫기 버튼
    public Slider soundSlider; //사운드 슬라이더
    string videoSceneName = "VideoScene";
    string customizationSceneName = "Customization";
    [Header("Language")]
    public GameObject languageBt;
    public Button koreanBt;
    public Button englishBt;
    public Sprite languageBox;
    public Sprite alpha;
    [Header("Enter")]
    public GameObject billboard; //전광판
    public GameObject enterTab; // 입장 UI
    public Button enterBt;//입장 버튼
    private float Distance; // 거리
    public float constDist = 3f; //일정거리

    private void Start()
    {
        //언어
        koreanBt.onClick.AddListener(() => ChangeLanguage("Korean", "?language=kr"));
        englishBt.onClick.AddListener(() => ChangeLanguage("English","?language=eng"));

        //퀴즈
        GetQuiz();
    }

    private void ChangeLanguage(string language, string subTitleVideoUrl)
    {
        //한국어 버튼일때
        if(language == "Korean")
        {
            koreanBt.GetComponent<Image>().sprite = languageBox;
            englishBt.GetComponent<Image>().sprite = alpha;

            //한국어자막 영상 통신
            subTitleVideoUrl = "?language=kr";
            HttpInfo info = new HttpInfo();

            info.Set(RequestType.GET, subTitleVideoUrl, (DownloadHandler downloadHandler) =>
            {

            });
            HttpManager.instance.SendRequest(info);
        }
        //영어 버튼일때
        else if (language == "English")
        {
            koreanBt.GetComponent<Image>().sprite = alpha;
            englishBt.GetComponent<Image>().sprite = languageBox;

            //외국어자막 영상 통신
        }

    }

    private void Update()
    {
        EnterUI();
    }

    //퀴즈 통신
    public void GetQuiz()
    {
        //퀴즈 겟
        HttpInfo info = new HttpInfo();
        string url = "/quiz-rank";
        info.Set(RequestType.GET, url, (DownloadHandler downloadHandler) =>
        {
            //Get 데이터 전송했을 때 서버로부터 응답온다
            JObject jObject = JObject.Parse(downloadHandler.text);
            JArray dataArray = jObject["data"].ToObject<JArray>();

            foreach (var item in dataArray)
            {
                string english = item["english"].ToString();
                string korean = item["korean"].ToString();

                questionEnglish.text = english;
                questionKorean.text = korean;
            }
        });
        HttpManager.instance.SendRequest(info);
    }

    //씬이동 -- 비디오씬, 커스텀씬
    public void SceneChange(string sceneName)
    {
        GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
        GlobalValue.CurSceneName = sceneName;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(sceneName);
    }

    //설정 탭 열기
    public void OpenTab()
    {
        settingTab.SetActive(true);
    }
    //설정 탭 닫기
    public void CloseTab()
    {
        if (closeBt.onClick != null)
            settingTab.SetActive(false);
    }
    //플레이어가 전광판의 일정거리안에 들어가면 EnterUI가 생성된다.
    public void EnterUI()
    {
        //플레이어와 전광판간의 거리를 잰다
        Distance = Vector3.Distance(billboard.transform.position, KHHPhotonManager.Instance.player.transform.position);

        //일정거리 안에 들어가면
        if (Distance < constDist)
        {
            enterTab.SetActive(true);
        }
        else
        {
            enterTab.SetActive(false);
        }
        if (enterBt != null && Input.GetKeyDown(KeyCode.F))
        {
            GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
            GlobalValue.CurSceneName = "VideoScene";
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("VideoScene");
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (videoBt != null && videoBt.onClick.GetPersistentEventCount() == 0)
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(videoBt.onClick, SceneChange, videoSceneName);
        if (customizeBt != null && customizeBt.onClick.GetPersistentEventCount() == 0)
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(customizeBt.onClick, SceneChange, customizationSceneName);
    }
#endif
}
