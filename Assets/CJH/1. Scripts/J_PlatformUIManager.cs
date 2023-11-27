using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using Photon.Pun;
using DG.Tweening;

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
    public int videoNo;
    [Header("Platform")]
    public Button videoBt; //영상보러가기 버튼
    public Button customizeBt; //아바타 커스터마이징 가기 버튼
    [Header("Setting")]
    public GameObject blockSetting;
    public CanvasGroup settingCG;
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
    public float constDist = 3f; //일정거리
    private void Start()
    {
        //언어
        koreanBt.onClick.AddListener(() => ChangeLanguage("Korean", "?language=kr"));
        englishBt.onClick.AddListener(() => ChangeLanguage("English","?language=eng"));
        //한국어 버튼일때
        if (GlobalValue.myLanguage== SystemLanguage.Korean)
        {
            koreanBt.GetComponent<Image>().sprite = languageBox;
            englishBt.GetComponent<Image>().sprite = alpha;
        }
        //영어 버튼일때
        else
        {
            koreanBt.GetComponent<Image>().sprite = alpha;
            englishBt.GetComponent<Image>().sprite = languageBox;
        }
        //퀴즈
        GetQuiz();
    }
    private void ChangeLanguage(string language, string subTitleVideoUrl)
    {
        //한국어 버튼일때
        if(language == "Korean")
        {
            GlobalValue.myLanguage = SystemLanguage.Korean; 
            koreanBt.GetComponent<Image>().sprite = languageBox;
            englishBt.GetComponent<Image>().sprite = alpha;
        }
        //영어 버튼일때
        else if (language == "English")
        {
            GlobalValue.myLanguage = SystemLanguage.English;
            koreanBt.GetComponent<Image>().sprite = alpha;
            englishBt.GetComponent<Image>().sprite = languageBox;
        }
    }
    //떠오르기 애니메이션 ui
    void Init()
    {
        blockSetting.SetActive(true);
        settingCG.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.Linear);
        settingCG.DOFade(1, 0.5f).SetEase(Ease.Linear).OnComplete(() => settingCG.blocksRaycasts = true);
    }
    //사라지기 애니메이션 ui
    void Outit()
    {
        settingCG.transform.DOLocalMoveY(-270f, 0.5f).SetEase(Ease.Linear);
        settingCG.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => 
        {
            blockSetting.SetActive(false);
            settingCG.blocksRaycasts = true;
        });
    }
    //퀴즈 통신
    public void GetQuiz()
    {
        //퀴즈 겟
        HttpInfo info = new HttpInfo();
        string quizUrl = "/quiz-rank";
        info.Set(RequestType.GET, quizUrl, (DownloadHandler downloadHandler) =>
        {
            //Get 데이터 전송했을 때 서버로부터 응답온다
            JObject jObject = JObject.Parse(downloadHandler.text);
            JArray dataArray = jObject["data"].ToObject<JArray>();

            foreach (var item in dataArray)
            {
                int shortFormNo = item["shortFormNo"].ToObject<int>();
                string english = item["english"].ToString();
                string korean = item["korean"].ToString();
                questionEnglish.text = english;
                questionKorean.text = korean;
                videoNo = shortFormNo;
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
        Init();
    }
    //설정 탭 닫기
    public void CloseTab()
    {
        if (closeBt.onClick != null)
         Outit();
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
