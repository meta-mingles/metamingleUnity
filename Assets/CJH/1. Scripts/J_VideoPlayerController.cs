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


    //public Button playBt; //��� ��ư
    //public Button pauseBt; //���� ��ư


    // Start is called before the first frame update
    void Start()
    {
        //���� �÷��̾� ������Ʈ �߰�
        //videoPlayer = gameObject.AddComponent<VideoPlayer>();
        //videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        //videoPlayer.targetTexture = rt;
        //videoPlayer.url = J_VideoReceiver.Get().GetVideoUrl(); // ���� url ����
        //videoOutput.texture = rt;


        //��ư�� ������ ����
        //playBt.onClick.AddListener(PlayVideo);
        //pauseBt.onClick.AddListener(PauseVideo);        
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
        Debug.Log("���� ����");
    }

    public void PauseVideo()
    {
        videoPlayer.DOPause();
        Debug.Log("���� ����");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
