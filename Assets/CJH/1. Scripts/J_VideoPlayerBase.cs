using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class J_VideoPlayerBase : MonoBehaviour
{
    public string videoUrl;
    public VideoPlayer videoPlayer;
    public ShortVideoInfo videoInfo;
    public Action onClickEvent;
    public Button playBt;
    public Button pauseBt;
    public Button restartBt;
    public Slider progressSlider; //영상 진행 
    public Slider soundSlider; //사운드 볼륨 조절 
    public TMP_Text currentTimeText;
    public TMP_Text totalTimeText;

    private void Awake()
    {
        soundSlider.onValueChanged.AddListener(HandleVolumeChange);
    }
    private void Update()
    {
        if (videoPlayer.isPlaying)
        {
            //비디오 준비 완료되면 로딩UI 비활성화
            UpdateTimeText();
            UpdateProgressSlider();
        }
    }
    //사운드 조정
    private void HandleVolumeChange(float volume)
    {
        videoPlayer.SetDirectAudioVolume(0, volume);
    }
    //현재 시간 텍스트 
    private void UpdateTimeText()
    {
        currentTimeText.text = FormatTime((int)videoPlayer.time);
    }
    public virtual void SetItem(ShortVideoInfo Info)
    {
        RenderTexture rt = new RenderTexture(1920, 1080, 24);
        videoPlayer.targetTexture = rt;
        videoPlayer.GetComponentInChildren<RawImage>().texture = rt;

        videoInfo = Info;

        // 영상 다운로드
        HttpInfo httpInfo = new HttpInfo();
        string subTitleurl = "";
        if (GlobalValue.myLanguage == SystemLanguage.Korean)
        {
            subTitleurl = "?language=kr";
        }
        else if (GlobalValue.myLanguage == SystemLanguage.English)
        {
            subTitleurl = "?language=eng";
        }

        if (GlobalValue.directVideoNo != 0)
        {
            videoUrl = videoInfo.url + GlobalValue.directVideoNo + subTitleurl;
            GlobalValue.directVideoNo = 0;
        }
        else
        {
            videoUrl = videoInfo.url + subTitleurl;
        }

        httpInfo.Set(RequestType.GET, videoUrl, (downloadHandler) =>
        {
            byte[] videoBytes = downloadHandler.data;
            videoPlayer.url = videoInfo.url;
            videoPlayer.prepareCompleted += OnVideoPrepared; //비디오 준비 완료 이벤트
            videoPlayer.Play();
            videoPlayer.loopPointReached += MakeInteractiveUI;
            videoPlayer.loopPointReached += MakeRestartUI;
        }, null, false);
        HttpManager.instance.SendRequest(httpInfo);
        UpdateTimeText();
        UpdateProgressSlider();
    }
    //비디오 준비
    private void OnVideoPrepared(VideoPlayer source)
    {
        double videoLength = source.length;
        int totalTime = Convert.ToInt32(videoLength);
        totalTimeText.text = FormatTime(totalTime);
    }

    private string FormatTime(int timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }
    //영상이 끝날때 인터렉티브 UI 생성
    protected virtual void MakeInteractiveUI(VideoPlayer source)
    {

    }
    //영상이 끝날때 Restart UI 생성
    protected virtual void MakeRestartUI(VideoPlayer source)
    {

    }
    //RestartButton으로 여는 함수
    public void ClickRestartButton()
    {
        Destroy(gameObject);
    }
    //영상 재생
    public void PlayPauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            playBt.gameObject.SetActive(false);
            pauseBt.gameObject.SetActive(true);
        }
        else
        {
            videoPlayer.Play();
            pauseBt.gameObject.SetActive(false);
            playBt.gameObject.SetActive(true);
        }
    }
    //영상 끄기
    public virtual void CloseVideo()
    {
        onClickEvent?.Invoke();
        //InteractiveDTOS.ReferenceEquals(gameObject, null);
        Destroy(gameObject);
    }
    //영상 진행률 슬라이더
    private void UpdateProgressSlider()
    {
        if (videoPlayer.frameCount > 0)
        {
            float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
            progressSlider.value = progress;
        }
    }

}
