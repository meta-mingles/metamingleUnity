using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KHHScreenEditor : MonoBehaviour
{
    public VNectModel model;
    public RawImage backgroundImage;

    int initCount = 0;
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
        Init();
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

    //처음 로딩할 때 호출
    public void Init()
    {
        fileLoaded = false;
        initCount = 0;

        if (KHHEditVideoState.MotionName != string.Empty)
        {
            string filePathMotion = KHHVideoData.FileMotionPath + "/" + KHHEditVideoState.MotionName;
            //모션 데이터를 로드한다.
            GameObject go1 = Instantiate(editItemPrefabs[0], editItemParent);
            KHHEditItemMotion editItemMotion = go1.GetComponent<KHHEditItemMotion>();
            editItemMotion.Init(KHHEditVideoState.MotionChangeX, KHHEditVideoState.MotionChangeLeftX, KHHEditVideoState.MotionChangeRightX);
            editItemMotion.LoadItemData(this, filePathMotion + ".csv", () => { initCount++; if (initCount == 4) fileLoaded = true; });
            editItemMotion.Model = model;
            editItemList.Add(editItemMotion);

            //보이스 데이터를 로드한다.
            GameObject go2 = Instantiate(editItemPrefabs[1], editItemParent);
            KHHEditItemSound editItemVoice = go2.GetComponent<KHHEditItemSound>();
            editItemVoice.IsVoice = true;
            editItemVoice.Init(KHHEditVideoState.MotionVChangeX, KHHEditVideoState.MotionVChangeLeftX, KHHEditVideoState.MotionVChangeRightX);
            editItemVoice.LoadItemData(this, filePathMotion + ".wav", () => { initCount++; if (initCount == 4) fileLoaded = true; });
            editItemList.Add(editItemVoice);
        }

        if (KHHEditVideoState.SoundName != string.Empty)
        {
            string filePathSound = KHHVideoData.FileSoundPath + "/" + KHHEditVideoState.SoundName;
            //사운드 데이터를 로드한다.
            GameObject go3 = Instantiate(editItemPrefabs[1], editItemParent);
            KHHEditItemSound editItemSound = go3.GetComponent<KHHEditItemSound>();
            editItemSound.Init(KHHEditVideoState.SoundChangeX, KHHEditVideoState.SoundChangeLeftX, KHHEditVideoState.SoundChangeRightX);
            editItemSound.LoadItemData(this, filePathSound, () => { initCount++; if (initCount == 4) fileLoaded = true; });
            editItemList.Add(editItemSound);
        }

        if (KHHEditVideoState.ImageName != string.Empty)
        {
            string filePathBackground = KHHVideoData.FileImagePath + "/" + KHHEditVideoState.ImageName;
            //배경 데이터를 로드한다.
            GameObject go4 = Instantiate(editItemPrefabs[2], editItemParent);
            KHHEditItemBackground editItemImage = go4.GetComponent<KHHEditItemBackground>();
            editItemImage.Init(KHHEditVideoState.ImageChangeX, KHHEditVideoState.ImageChangeLeftX, KHHEditVideoState.ImageChangeRightX);
            editItemImage.LoadItemData(this, filePathBackground, () => { initCount++; if (initCount == 4) fileLoaded = true; });
            editItemList.Add(editItemImage);
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
                KHHEditVideoState.MotionName = data.FileName;
                LoadFileMotion(data.FileName);
                break;
            case KHHData.DataType.SoundData:
                KHHEditVideoState.SoundName = fileName;
                LoadFileSound(fileName);
                break;
            case KHHData.DataType.BackgroundData:
                KHHEditVideoState.ImageName = fileName;
                LoadFileBackground(fileName);
                break;
            case KHHData.DataType.CaptionData:
                break;
        }
    }

    public void LoadFileMotion(string fileName)
    {
        fileLoaded = false;
        string filePath = KHHVideoData.FileMotionPath + "/" + fileName;

        //모션 데이터를 로드한다.
        GameObject go1 = Instantiate(editItemPrefabs[0], editItemParent);
        KHHEditItemMotion editItemMotion = go1.GetComponent<KHHEditItemMotion>();
        editItemMotion.Init();
        editItemMotion.LoadItemData(this, filePath + ".csv", null);
        editItemMotion.Model = model;
        editItemList.Add(editItemMotion);

        //보이스 데이터를 로드한다.
        GameObject go2 = Instantiate(editItemPrefabs[1], editItemParent);
        KHHEditItemSound editItemVoice = go2.GetComponent<KHHEditItemSound>();
        editItemVoice.Init();
        editItemVoice.IsVoice = true;
        editItemVoice.LoadItemData(this, filePath + ".wav", () => fileLoaded = true);
        editItemList.Add(editItemVoice);
    }

    public void LoadFileSound(string fileName)
    {
        fileLoaded = false;
        string filePath = KHHVideoData.FileSoundPath + "/" + fileName;

        //사운드 데이터를 로드한다.
        GameObject go = Instantiate(editItemPrefabs[1], editItemParent);
        KHHEditItemSound editItem = go.GetComponent<KHHEditItemSound>();
        editItem.Init();
        editItem.LoadItemData(this, filePath, () => fileLoaded = true);
        editItemList.Add(editItem);
    }

    public void LoadFileBackground(string fileName)
    {
        fileLoaded = false;
        string filePath = KHHVideoData.FileImagePath + "/" + fileName;

        //배경 데이터를 로드한다.
        GameObject go = Instantiate(editItemPrefabs[2], editItemParent);
        KHHEditItemBackground editItem = go.GetComponent<KHHEditItemBackground>();
        editItem.Init();
        editItem.LoadItemData(this, filePath, () => fileLoaded = true);
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
