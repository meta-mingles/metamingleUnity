using DG.Tweening;
using UnityEngine.Video;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon.StructWrapping;

public class J_VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public RawImage videoOutput;
    public RenderTexture rt;


    //public Button playBt; //재생 버튼
    //public Button pauseBt; //정지 버튼


    // Start is called before the first frame update
    void Start()
    {
        //비디오 플레이어 컴포넌트 추가
        //videoPlayer = gameObject.AddComponent<VideoPlayer>();
        //videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        //videoPlayer.targetTexture = rt;
        //videoPlayer.url = J_VideoReceiver.Get().GetVideoUrl(); // 비디오 url 설정
        //videoOutput.texture = rt;


        //버튼에 리스너 연결
        //playBt.onClick.AddListener(PlayVideo);
        //pauseBt.onClick.AddListener(PauseVideo);        
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
        Debug.Log("비디오 실행");
    }

    public void PauseVideo()
    {
        videoPlayer.DOPause();
        Debug.Log("비디오 정지");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
