﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

//상속 클래스
//비디오 플레이어의 부모
public class J_VideoPlayerBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public GameObject playArea;

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
    //영상 세팅
    public virtual void SetItem(ShortVideoInfo Info)
    {
        //VidoePlayer이용하여 영상 세팅
        RenderTexture rt = new RenderTexture(1920, 1080, 24);
        videoPlayer.targetTexture = rt;
        videoPlayer.GetComponentInChildren<RawImage>().texture = rt;

        videoInfo = Info;

        // 영상 다운로드
        HttpInfo httpInfo = new HttpInfo();
        string subTitleurl = "";

        //한글 or 영어 자막 세팅
        if (GlobalValue.myLanguage == SystemLanguage.Korean)
        {
            subTitleurl = "?language=kr";
        }
        else if (GlobalValue.myLanguage == SystemLanguage.English)
        {
            subTitleurl = "?language=eng";
        }
        //퀴즈로 연결되는 영상인지 확인
        if (GlobalValue.directVideoNo != 0)
        {
            videoUrl = videoInfo.url + GlobalValue.directVideoNo + subTitleurl;
            GlobalValue.directVideoNo = 0;
        }
        else
        {
            videoUrl = videoInfo.url + subTitleurl;
        }
        //영상 Get요청
        httpInfo.Set(RequestType.GET, videoUrl, (downloadHandler) =>
        {
            byte[] videoBytes = downloadHandler.data;
            videoPlayer.url = videoInfo.url; 
            videoPlayer.prepareCompleted += OnVideoPrepared; //비디오 준비 완료 이벤트
            videoPlayer.Play(); //영상 재생
            videoPlayer.loopPointReached += MakeInteractiveUI; //인터렉티브 UI 이벤트
            videoPlayer.loopPointReached += MakeRestartUI; //Restart UI 이벤트
        }, null, false);
        HttpManager.instance.SendRequest(httpInfo);
        UpdateTimeText();
        UpdateProgressSlider();
    }
    //비디오 생성전 영상 길이 표시
    private void OnVideoPrepared(VideoPlayer source)
    {
        double videoLength = source.length;
        int totalTime = Convert.ToInt32(videoLength);
        totalTimeText.text = FormatTime(totalTime);
    }
    //시간 변환
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
    public void CloseVideo()
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

    //플레이 바 특정영역을 벗어나면 껏다 켰다 하기
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playArea != null)
        {
            playArea.SetActive(true); // Show the playArea
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (playArea != null)
        {
            playArea.SetActive(false); // Hide the playArea
        }
    }


}
