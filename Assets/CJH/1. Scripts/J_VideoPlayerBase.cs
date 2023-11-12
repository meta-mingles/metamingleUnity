using DG.Tweening;
using System;
using System.IO;
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
    
    public virtual void SetItem(ShortVideoInfo Info)
    {
        videoInfo = Info;

        // 영상 다운로드
        HttpInfo httpInfo = new HttpInfo();

        httpInfo.Set(RequestType.GET, videoInfo.url, (downloadHandler) =>
        {
            byte[] videoBytes = downloadHandler.data;

 

            //FileStream file = new FileStream(Application.dataPath +"/" + "CJH" + "/10.Videos" + "/" + videoInfo.title + ".mp4", FileMode.Create);
            //byteData 를 file 에 쓰자
            //file.Write(videoBytes, 0, videoBytes.Length);
            //file.Close();

            RenderTexture rt = new RenderTexture(1920, 1080, 24);

            videoPlayer.targetTexture = rt;
            videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
            //videoPlayer.url = Application.dataPath + "/" + "CJH" + "/10.Videos"+ "/" + videoInfo.title + ".mp4";
            videoPlayer.url = videoInfo.url;
            videoPlayer.Play();

            videoPlayer.loopPointReached += MakeInteractiveUI;

            videoPlayer.loopPointReached += MakeRestartUI;


        }, false);

        HttpManager.Get().SendRequest(httpInfo);
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
        //if (onClickEvent != null)
        //{
        //    onClickEvent();
        //}
        onClickEvent?.Invoke();

        Destroy(gameObject);
        
    }

}
