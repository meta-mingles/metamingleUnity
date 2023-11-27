using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;


public class J_VideoReceiver : MonoBehaviour
{
    public static J_VideoReceiver instance;
    [Header("ThumNailList")]
    public GameObject thumbnailScrollView;
    public GameObject thumbnailFactory;
    public Transform trCtOfSV; //썸네일 스크롤뷰 생성장소
    [Header("ShortVideo")]
    public GameObject videoFactory;
    public GameObject interactiveVideoFactory;
    public Transform trCtOFVideoSV; //비디오 스크롤뷰 생성장소
    private string thumbnailUrl;
    J_InteractiveVideoPlayer interactiveVideoPlayer;
    //public GameObject errorVideoFactory; //조회실패 팝업창
    private void Awake()
    {
        instance = this;
        // thumbnailUrl 초기화
        if (GlobalValue.myLanguage == SystemLanguage.Korean)
        {
            thumbnailUrl = "?language=kr";
        }
        else if (GlobalValue.myLanguage == SystemLanguage.English)
        {
            thumbnailUrl = "?language=eng";
        }
        if (GlobalValue.directVideoNo != 0)
        {
            VideoLoad();
        }
        else
        {
            ListLoad();
        }
    }
    //바로 숏폼 조회가 되는 씬
    private void VideoLoad()
    {
        //서버한테 영상 정보 요청
        HttpInfo httpInfo = new HttpInfo();
        string url = "/short-form";
        httpInfo.Set(RequestType.GET, url + "/"+ GlobalValue.directVideoNo + thumbnailUrl, OnDirectVideo);
        HttpManager.instance.SendRequest(httpInfo);
    }
    //리스트가 열리는 씬
    private void ListLoad()
    {
        // 서버한테 영상 정보 요청
        HttpInfo httpInfo = new HttpInfo();
        string thumbnailUrl = "";
        if (GlobalValue.myLanguage == SystemLanguage.Korean)
        {
            thumbnailUrl = "?language=kr";
        }
        else if (GlobalValue.myLanguage == SystemLanguage.English)
        {
            thumbnailUrl = "?language=eng";
        }
        string url = "/short-form";
        httpInfo.Set(RequestType.GET, url+ thumbnailUrl, OnCompleteSearchVideo);
        HttpManager.instance.SendRequest(httpInfo);
    }

    //숏폼 다운
    void OnCompleteSearchVideo(DownloadHandler downloadHandler)
    {
        //데이터 셋팅
        //J_DataManager.instance.SetShortVideoInfoListByCSV(downloadHandler.text);
        J_DataManager.instance.SetShortVideoInfoListByJSON(downloadHandler.text);

        //UI 만들자
        for (int i = 0; i < J_DataManager.instance.shortVideoInfoList.Count; i++)
        {
            // 섬네일(비디오 썸네일)을 만듭니다.
            GameObject video = Instantiate(thumbnailFactory, trCtOfSV);
            J_ThumItem item = video.GetComponent<J_ThumItem>();
            // 항목 설정 - 각 섬네일에 대한 정보를 설정합니다.
            item.SetItem(J_DataManager.instance.shortVideoInfoList[i]);
            //썸네일 클릭 시, 숏폼(짧은 형식의) 영상 창을 만듭니다.
            item.onClickEvent = CreateShortVideo;
        }
    }

    //숏폼 다운
    void OnDirectVideo(DownloadHandler downloadHandler)
    {
        ListLoad();

        JObject jObject = JObject.Parse(downloadHandler.text);

        ShortVideoInfo data =jObject["data"].ToObject<ShortVideoInfo>();

        CreateShortVideo(data);

        //list 버튼 = > 닫기 버튼눌렀을 때 리스트 생성

    }

    //인터렉티브 비디오 열리는 함수
    public void CreateInteractiveMovie(ShortVideoInfo info)
    {
        //인터렉티브 비디오 생성
        GameObject video = Instantiate(interactiveVideoFactory, trCtOFVideoSV);
        interactiveVideoPlayer = video.GetComponent<J_InteractiveVideoPlayer>();
        interactiveVideoPlayer.SetItem(info);
        interactiveVideoPlayer.onClickEvent = CloseShortVideo;
    }
    //숏폼비디오 열리는 함수
    public void CreateShortVideo(ShortVideoInfo info)
    {
        //숏비디오 생성
        GameObject video = Instantiate(videoFactory, trCtOFVideoSV);
        J_ShortVideoPlayer item = video.GetComponent<J_ShortVideoPlayer>();
        item.SetItem(info);
        thumbnailScrollView.SetActive(false);
        item.onClickEvent = CloseShortVideo;
    }
    //숏폼비디오 닫는 함수
    void CloseShortVideo()
    {
        if(interactiveVideoPlayer != null)
        {
            interactiveVideoPlayer.gameObject.SetActive(false);
        }

        thumbnailScrollView.SetActive(true);
  
    }
}

