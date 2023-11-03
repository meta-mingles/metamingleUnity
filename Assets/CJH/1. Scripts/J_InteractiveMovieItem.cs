using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.Linq;

public class J_InteractiveMovieItem : MonoBehaviour
{
    public TMP_Text choice;

    public VideoPlayer videoPlayer;

    public InteractiveMovieInfo interactiveMovieInfo;

    private ShortVideoInfoContainer shortVideoInfoContainer;
   


    public Action<ShortVideoInfo> onClickEvent;

    void Start()
    {
    }

    void Update()
    {

    }


    //public void OnClick()
    //{
    //    if (onClickEvent != null)
    //    {
    //        onClickEvent(interactiveMovieInfo);
    //    }
    //}

    // 컨테이너 설정
    public void SetContainers(ShortVideoInfoContainer shortContainer)
    {
        shortVideoInfoContainer = shortContainer;
       
    }

    //인터렉티브 무비 데이터 셋팅
    public void SetItem(InteractiveMovieInfo Info)
    {
        interactiveMovieInfo = Info;

        // 인터렉티브무비인지 판단
        if (interactiveMovieInfo.sequence > 0)
        {
            PlayInteractiveMovie(interactiveMovieInfo.interactiveMovieNo);
        }
    }

    #region 기존
    ////인터렉티브 무비 데이터 셋팅
    //public void SetItem(InteractiveMovieInfo Info)
    //{
    //    interactiveMovieInfo = Info;

    //    HttpInfo httpInfo = new HttpInfo();

    //    httpInfo.Set(RequestType.GET, interactiveData.url, (downloadHandler) =>
    //    {

    //        byte[] videoBytes = downloadHandler.data;

    //        FileStream file = new FileStream(Application.dataPath + "/" + interactiveMovieInfo.title + ".mp4", FileMode.Create);
    //        //byteData 를 file 에 쓰자
    //        file.Write(videoBytes, 0, videoBytes.Length);
    //        file.Close();

    //        RenderTexture rt = new RenderTexture(1920, 1080, 24);

    //        videoPlayer.targetTexture = rt;
    //        videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
    //        videoPlayer.url = Application.dataPath + "/" + interactiveMovieInfo.title + ".mp4";
    //        videoPlayer.Play();

    //    }, false);

    //    HttpManager.Get().SendRequest(httpInfo);

    //}
    #endregion
    void PlayInteractiveMovie(long movieNo)
    {
        // movieNo에 해당하는 영상을 찾아서 재생하는 코드
        // 예를 들어, movieNo에 해당하는 비디오 정보를 ShortVideolInfo에서 찾은 후, 해당 비디오를 재생합니다.
        ShortVideoInfo selectedVideo = FindVideoByInteractiveMovieNo(movieNo);
        if (selectedVideo != null && selectedVideo.isInteractive)
        {
            // 선택한 영상을 재생하는 로직을 추가합니다.
            // 예를 들어, selectedVideo.url을 사용하여 영상을 재생합니다.
        }
        else
        {
            Debug.Log("인터랙티브 비디오를 찾을 수 없습니다!");
        }
    }
    //비디오 다운로드 및 재생
    IEnumerator DownloadAndPlayVideo(string videoUrl)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(videoUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("비디오 다운로드 실패: " + www.error);
                yield break;
            }

            string videoFilePath = Application.persistentDataPath + "/video.mp4";
            File.WriteAllBytes(videoFilePath, www.downloadHandler.data);

            StartCoroutine(PlayVideo(videoFilePath));
        }
    }

    //비디오 재생 함수
    IEnumerator PlayVideo(string videoPath)
    {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        videoPlayer.url = videoPath;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
    }
    //인터렉티브 비디오 찾는 함수
    ShortVideoInfo FindVideoByInteractiveMovieNo(long movieNo)
    {
        List<ShortVideoInfo> videoList = shortVideoInfoContainer.data.Where(v => v.isInteractive).ToList();
        ShortVideoInfo selectedVideo = videoList.Find(video => video.shortFormNo == movieNo);

        return selectedVideo;
    }

    //void OnChoiceSelected(long movieNo, string title)
    //{
    //    // 여기서 선택한 선택지에 따라 팝업된 선택지 스크롤뷰를 닫고 영상을 재생하는 코드를 작성합니다.
    //    CloseChoiceScrollView(); // 팝업된 선택지 스크롤뷰를 닫는 함수 호출

    //    // 선택한 영상을 재생하는 함수 호출
    //    PlayInteractiveMovie(movieNo, title);
    //}


    //void CloseChoiceScrollView()
    //{
    //    // 팝업된 선택지 스크롤뷰를 닫는 로직을 추가합니다.
    //}


}
