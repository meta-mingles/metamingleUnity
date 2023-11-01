using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHScreenEditor : MonoBehaviour
{
    public VNectModel model;

    bool fileLoaded = false;
    public bool FileLoaded { get { return fileLoaded; } }

    bool isPlaying = false;
    public bool IsPlaying { get { return isPlaying; } set { isPlaying = value; } }

    List<KHHEditItem> editItemList;
    public GameObject[] editItemPrefabs;

    List<AudioSource> audioSourceList;
    List<AudioClip> audioClipList;

    public Transform editItemParent;
    public KHHModelRecorder modelRecorder;

    // Start is called before the first frame update
    void Start()
    {
        audioSourceList = new List<AudioSource>();
        audioClipList = new List<AudioClip>();

        editItemList = new List<KHHEditItem>();
    }

    //드롭 아이템이 편집 영역에 드랍되었을 때 호출
    public void OnDropItem(GameObject dropItem)
    {
        fileLoaded = false;

        //드롭 아이템의 파일 이름을 얻어온다.
        KHHData data = dropItem.GetComponent<KHHData>();

        //드롭 아이템의 타입을 얻어온다.
        switch (data.type)
        {
            case KHHData.DataType.None:
                break;
            case KHHData.DataType.MotionData:
                LoadFileMotion(data.FileName);
                break;
            case KHHData.DataType.SoundData:
                break;
            case KHHData.DataType.CaptionData:
                break;
        }
    }

    public void LoadFileMotion(string fileName)
    {
        fileLoaded = false;
        GameObject go = Instantiate(editItemPrefabs[0], editItemParent);

        KHHEditItemMotion editItem = go.GetComponent<KHHEditItemMotion>();
        //모델 레코더에 파일 이름을 전달한다.
        //editItem.LoadRecordData(this, fileName, () => { fileLoaded = true; });
        editItem.Init(model);
        editItem.Set();
        editItemList.Add(editItem);
    }

    public void Play()
    {
        if (fileLoaded)
        {
            isPlaying = true;
            for (int i = 0; i < editItemList.Count; i++)
            {
                editItemList[i].PlayStart();
            }
            for (int i = 0; i < audioSourceList.Count; i++)
            {
                audioSourceList[i].clip = audioClipList[i];
                audioSourceList[i].Play();
            }
        }
    }

    public void Stop()
    {
        if (fileLoaded)
        {
            isPlaying = false;
            for (int i = 0; i < editItemList.Count; i++)
            {
                editItemList[i].PlayStop();
            }
            for (int i = 0; i < audioSourceList.Count; i++)
            {
                audioSourceList[i].Stop();
            }
        }
    }

    public void End()
    {
        if (fileLoaded)
        {
            isPlaying = false;
            for (int i = 0; i < editItemList.Count; i++)
            {
                editItemList[i].PlayEnd();
            }
            for (int i = 0; i < audioSourceList.Count; i++)
            {
                audioSourceList[i].Stop();
            }
        }

    }
}
