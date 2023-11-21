using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public class J_PlatformUIManager : MonoBehaviour
{
    public static J_PlatformUIManager Instance;
    private void Awake()
    {
        Instance = this;

        //사운드 키기
        SoundManager.instance.BGMVolume = -5;
        SoundManager.instance.PlayBGM("PlatformBGM");
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

    string videoSceneName = "VideoScene";
    string customizationSceneName = "Customization";

    [Header("Enter")]
    public GameObject billboard; //전광판
    public GameObject enterTab; // 입장 UI
    public Button enterBt;//입장 버튼
    private float Distance; // 거리
    public float constDist = 3f; //일정거리


    [Header("Name")]
    public GameObject nameText;
    [SerializeField] public float temp = 1.8f;

    private void Start()
    {
        GetQuiz();
    }

    private void Update()
    {
        UpdateNickNamePosition();
        SetNickNameText();
        EnterUI();
    }
    
    //퀴즈 통신
    public void GetQuiz()
    {
        //퀴즈 겟
        HttpInfo info = new HttpInfo();
        info.Set(RequestType.GET, "/quiz-rank", (DownloadHandler downloadHandler) =>
        {
            //Get 데이터 전송했을 때 서버로부터 응답온다
            Debug.Log("quiz : " + downloadHandler.text);

            JObject jObject = JObject.Parse(downloadHandler.text);
            JObject data = jObject["data"].ToObject<JObject>();
            HttpManager.instance.rankNo = data["rankNo"].ToObject<int>();
            HttpManager.instance.english = data["english"].ToObject<string>();
            HttpManager.instance.korean = data["korean"].ToObject<string>();
            HttpManager.instance.shortFormNo = data["shortFormNo"].ToObject<int>();
        });

        JObject jObject = new JObject();
        jObject["english"] = questionEnglish.text;
        jObject["korean"] = questionKorean.text;

        info.body = jObject.ToString();
        HttpManager.instance.SendRequest(info);
    }


    //씬이동 -- 비디오씬, 커스텀씬
    public void SceneChange(string sceneName)
    {

        GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
        GlobalValue.CurSceneName = sceneName;
        SceneManager.LoadScene(sceneName);
    }

    //설정 탭 열기
    public void OpenTab()
    {
        settingTab.SetActive(true);
        Debug.Log("Tab열림");
    }
    //설정 탭 닫기
    public void CloseTab()
    {
        if (closeBt.onClick != null)
            settingTab.SetActive(false);
        Debug.Log("Tab닫힘");
    }
    //플레이어가 전광판의 일정거리안에 들어가면 EnterUI가 생성된다.
    public void EnterUI()
    {
        //플레이어와 전광판간의 거리를 잰다
        Distance = Vector3.Distance(billboard.transform.position, KHHPhotonManager.Instance.player.transform.position);

        //일정거리 안에 들어가면
        if(Distance  < constDist)
        {
            enterTab.SetActive(true);
        }
        else
        {
            enterTab.SetActive(false);
        }
        if(enterBt != null && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("VideoScene");
        }
    }

    //로그인씬의 저장된 닉네임 플레이어 위에 생성
    public void SetNickNameText()
    {
        nameText.GetComponentInChildren<TMP_Text>().text = HttpManager.instance.nickname;
        
        //위치
        UpdateNickNamePosition();
    }

    private void UpdateNickNamePosition()
    {
        //위치 업데이트
        nameText.transform.position = new Vector3(KHHPhotonManager.Instance.player.transform.position.x, KHHPhotonManager.Instance.player.transform.position.y + temp, KHHPhotonManager.Instance.player.transform.position.z);
        //카메라의 Y 축 회전값을 사용하여 텍스트의 회전 업데이트
        Vector3 cameraRotation = Camera.main.transform.eulerAngles;
        nameText.transform.eulerAngles = new Vector3(0,cameraRotation.y, 0);
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
