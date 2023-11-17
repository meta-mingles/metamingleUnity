using DG.Tweening;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class J_VideoPlayerBase : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public ShortVideoInfo videoInfo;

    public Action onClickEvent;
    public Button playBt;
    public Button playBt1;
    public Button pauseBt;
    public Button restartBt;

    public Slider progressSlider;

    public TMP_Text currentTimeText;
    public TMP_Text totalTimeText;

    private void Update()
    {
        if(videoPlayer.isPlaying)
        {
            UpdateTimeText();
            UpdateProgressSlider();
        }
    }
    //현재 시간 텍스트 
    private void UpdateTimeText()
    {
        currentTimeText.text = FormatTime((int)videoPlayer.time);
    }
    //영상 진행률 슬라이더
    private void UpdateProgressSlider()
    {
        if(videoPlayer.frameCount > 0)
        {
            float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
            progressSlider.value = progress;
            //progressSlider.GetComponentInChildren<Image>().color = Color.red;

        }
    }

    public virtual void SetItem(ShortVideoInfo Info)
    {
        videoInfo = Info;

        // 영상 다운로드
        HttpInfo httpInfo = new HttpInfo();

        httpInfo.Set(RequestType.GET, videoInfo.url, (downloadHandler) =>
        {
            byte[] videoBytes = downloadHandler.data;
            RenderTexture rt = new RenderTexture(1920, 1080, 24);

            videoPlayer.targetTexture = rt;
            videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
            videoPlayer.url = videoInfo.url;
            videoPlayer.prepareCompleted += OnVideoPrepared; //비디오 준비 완료 이벤트
            videoPlayer.Play();
            videoPlayer.loopPointReached += MakeInteractiveUI;
            videoPlayer.loopPointReached += MakeRestartUI;
        }, false);

        HttpManager.instance.SendRequest(httpInfo);
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


    //희미해지는 재생 버튼
    public void FadePlayBt()
    {
        if(videoPlayer != null && playBt1 != null && playBt1.gameObject.activeSelf)
        {
            if (!videoPlayer.isPlaying) //멈췄을때
            {
                playBt1.interactable = false;

                //투명도 변경
                Image buttonImage = playBt1.GetComponent<Image>();
                if(buttonImage != null)
                {
                    buttonImage.DOFade(0.5f, 1f).OnComplete(() => {
                        playBt1.gameObject.SetActive(false);

                    });
                }
            }
        }
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

        Destroy(gameObject);
    }

}
