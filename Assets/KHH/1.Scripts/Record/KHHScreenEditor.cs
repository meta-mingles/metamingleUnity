using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KHHScreenEditor : MonoBehaviour
{
    public VNectModel model;
    public RawImage backgroundImage;

    bool fileLoaded = false;
    public bool FileLoaded { get { return fileLoaded; } }

    bool isPlaying = false;
    public bool IsPlaying { get { return isPlaying; } set { isPlaying = value; } }

    List<KHHEditItem> editItemList;
    public GameObject[] editItemPrefabs;

    public Transform editItemParent;
    public KHHModelRecorder modelRecorder;

    public float playTime = 0.0f;
    float endTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        editItemList = new List<KHHEditItem>();
    }

    void Update()
    {
        if (isPlaying)
        {
            playTime += Time.deltaTime;
            if (playTime > endTime)
            {
                End();
                KHHEditManager.Instance.StopButtonEvent();
                return;
            }
        }
    }

    //드롭 아이템이 편집 영역에 드랍되었을 때 호출
    public void OnDropItem(GameObject dropItem)
    {
        fileLoaded = false;

        //드롭 아이템의 파일 이름을 얻어온다.
        KHHData data = dropItem.GetComponent<KHHData>();
        string fileName = data.FileName + data.FileExtension;
        //드롭 아이템의 타입을 얻어온다.
        switch (data.type)
        {
            case KHHData.DataType.None:
                break;
            case KHHData.DataType.MotionData:
                LoadFileMotion(fileName);
                break;
            case KHHData.DataType.SoundData:
                LoadFileSound(fileName);
                break;
            case KHHData.DataType.BackgroundData:
                LoadFileBackground(fileName);
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
        editItem.LoadItemData(this, fileName, () => fileLoaded = true);
        editItem.Init(model);
        editItem.Set();
        editItemList.Add(editItem);
    }

    public void LoadFileSound(string fileName)
    {
        fileLoaded = false;
        GameObject go = Instantiate(editItemPrefabs[1], editItemParent);

        KHHEditItemSound editItem = go.GetComponent<KHHEditItemSound>();
        //모델 레코더에 파일 이름을 전달한다.
        editItem.LoadItemData(this, fileName, () => fileLoaded = true);
        editItem.Set();
        editItemList.Add(editItem);
    }

    public void LoadFileBackground(string fileName)
    {
        fileLoaded = false;
        GameObject go = Instantiate(editItemPrefabs[2], editItemParent);

        KHHEditItemBackground editItem = go.GetComponent<KHHEditItemBackground>();
        //모델 레코더에 파일 이름을 전달한다.
        editItem.LoadItemData(this, fileName, () => fileLoaded = true);
        editItem.Set();
        editItemList.Add(editItem);
    }

    public void Play()
    {
        playTime = 0.0f;
        endTime = 0.0f;
        if (fileLoaded)
        {
            isPlaying = true;
            for (int i = 0; i < editItemList.Count; i++)
            {
                editItemList[i].PlayStart();
                if (endTime < editItemList[i].EndTime) endTime = editItemList[i].EndTime;
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
        }
    }
}
