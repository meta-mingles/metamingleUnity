using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class KHHVideoData : KHHData
{
    protected string filePath;
    public string FilePath { get { return filePath; } }

    VideoPlayer videoPlayer;

    protected override void Awake()
    {
        base.Awake();
        videoPlayer = GetComponent<VideoPlayer>();
    }

    public void Update()
    {
        if (IsSelected)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                //파일 삭제
                File.Delete(KHHEditData.FileSoundPath + "/" + fileName + ".wav");
                khhDataManager.Refresh();
            }
        }
    }

    public override void Set(string fileName, string fileExtension, KHHDataManager manager)
    {
        base.Set(fileName, fileExtension, manager);
        filePath = KHHEditData.FileVideoPath + fileName + fileExtension;
        LoadVideo();
    }

    void LoadVideo()
    {
        //경로에 있는 mp4파일을 비디오 클립으로 로드한다.
        RenderTexture rt = new RenderTexture(1920, 1080, 24);
        videoPlayer.targetTexture = rt;
        videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
        videoPlayer.url = filePath;
        videoPlayer.SetDirectAudioVolume(0, 0);
        videoPlayer.Stop();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (videoPlayer.targetTexture != null)
            videoPlayer.Play();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (videoPlayer.targetTexture != null)
            videoPlayer.Stop();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        //드롭 아이템이 Interactive버튼 위에 드랍되었을 때 호출
        KHHInteractiveButton interactiveButton = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<KHHInteractiveButton>();
        if (interactiveButton != null)
        {
            interactiveButton.OnDropItem(this.gameObject);
        }

        base.OnEndDrag(eventData);
    }
}
