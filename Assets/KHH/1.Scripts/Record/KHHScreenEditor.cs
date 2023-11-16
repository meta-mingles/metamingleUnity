using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KHHScreenEditor : MonoBehaviour
{
    public RawImage backgroundImage;

    int initCount = 0;
    int loadCount = 0;
    bool fileLoaded = false;
    public bool FileLoaded { get { return fileLoaded; } }

    bool isPlaying = false;
    public bool IsPlaying { get { return isPlaying; } set { isPlaying = value; } }

    List<KHHEditItem> editItemList;
    public List<KHHEditItem> EditItemList { get { return editItemList; } }
    public GameObject[] editItemPrefabs;

    public Transform editItemParent;
    public KHHModelRecorder modelRecorder;

    public float playTime = 0.0f;
    float endTime = 0.0f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        editItemList = new List<KHHEditItem>();
        while (modelRecorder.Model == null)
            yield return null;
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
        loadCount = 0;

        if (KHHEditVideoState.MotionName != string.Empty)
        {
            string filePathMotion = KHHEditData.FileMotionPath + "/" + KHHEditVideoState.MotionName;
            if (System.IO.File.Exists(filePathMotion + ".csv") && System.IO.File.Exists(filePathMotion + ".wav"))
            {
                initCount += 2;
                //모션 데이터를 로드한다.
                GameObject go1 = Instantiate(editItemPrefabs[0], editItemParent);
                KHHEditItemMotion editItemMotion = go1.GetComponent<KHHEditItemMotion>();
                editItemMotion.Init(KHHEditVideoState.MotionChangeX, KHHEditVideoState.MotionChangeLeftX, KHHEditVideoState.MotionChangeRightX);
                editItemMotion.LoadItemData(this, filePathMotion + ".csv", KHHEditVideoState.MotionName, InitEnd);
                editItemMotion.Model = modelRecorder.Model;
                editItemList.Add(editItemMotion);
                //보이스 데이터를 로드한다.
                GameObject go2 = Instantiate(editItemPrefabs[1], editItemParent);
                KHHEditItemSound editItemVoice = go2.GetComponent<KHHEditItemSound>();
                editItemVoice.IsVoice = true;
                editItemVoice.Init(KHHEditVideoState.MotionVChangeX, KHHEditVideoState.MotionVChangeLeftX, KHHEditVideoState.MotionVChangeRightX);
                editItemVoice.LoadItemData(this, filePathMotion + ".wav", KHHEditVideoState.MotionName, InitEnd);
                editItemList.Add(editItemVoice);
                //페어간 정보 획득
                editItemMotion.PairSound = editItemVoice;
                editItemVoice.PairMotion = editItemMotion;
            }
            else
                KHHEditVideoState.MotionName = string.Empty;
        }

        if (KHHEditVideoState.SoundName != string.Empty)
        {
            string filePathSound = KHHEditData.FileSoundPath + "/" + KHHEditVideoState.SoundName;
            if (System.IO.File.Exists(filePathSound))
            {
                initCount++;
                //사운드 데이터를 로드한다.
                GameObject go3 = Instantiate(editItemPrefabs[1], editItemParent);
                KHHEditItemSound editItemSound = go3.GetComponent<KHHEditItemSound>();
                editItemSound.Init(KHHEditVideoState.SoundChangeX, KHHEditVideoState.SoundChangeLeftX, KHHEditVideoState.SoundChangeRightX);
                editItemSound.LoadItemData(this, filePathSound, KHHEditVideoState.SoundName, InitEnd);
                editItemList.Add(editItemSound);
            }
            else
                KHHEditVideoState.SoundName = string.Empty;
        }

        if (KHHEditVideoState.ImageName != string.Empty)
        {
            string filePathBackground = KHHEditData.FileImagePath + "/" + KHHEditVideoState.ImageName;
            if (System.IO.File.Exists(filePathBackground))
            {
                initCount++;
                //배경 데이터를 로드한다.
                GameObject go4 = Instantiate(editItemPrefabs[2], editItemParent);
                KHHEditItemBackground editItemImage = go4.GetComponent<KHHEditItemBackground>();
                editItemImage.Init(KHHEditVideoState.ImageChangeX, KHHEditVideoState.ImageChangeLeftX, KHHEditVideoState.ImageChangeRightX);
                editItemImage.LoadItemData(this, filePathBackground, KHHEditVideoState.ImageName, InitEnd);
                editItemList.Add(editItemImage);
            }
            else
                KHHEditVideoState.ImageName = string.Empty;
        }
    }

    public void Refresh()
    {
        foreach (var item in editItemList)
        {
            if (!item.CheckFile())
            {
                editItemList.Remove(item);
                item.Remove();
                break;
            }
        }
    }

    void InitEnd()
    {
        loadCount++;
        if (loadCount == initCount)
            fileLoaded = true;
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
                RemoveCurMotion();
                KHHEditVideoState.MotionName = data.FileName;
                LoadFileMotion(data.FileName);
                break;
            case KHHData.DataType.SoundData:
                RemoveCurSound();
                KHHEditVideoState.SoundName = fileName;
                LoadFileSound(fileName);
                break;
            case KHHData.DataType.BackgroundData:
                RemoveCurBackground();
                KHHEditVideoState.ImageName = fileName;
                LoadFileBackground(fileName);
                break;
            case KHHData.DataType.CaptionData:
                break;
        }

        //순서 조절
        foreach (var item in editItemList)
        {
            if (item is KHHEditItemMotion) item.transform.SetAsFirstSibling();
            if (item is KHHEditItemSound) if (((KHHEditItemSound)item).IsVoice) item.transform.SetSiblingIndex(1);
            if (item is KHHEditItemBackground) item.transform.SetAsLastSibling();
        }
    }

    void RemoveCurMotion()
    {
        editItemList.Find(x => x is KHHEditItemMotion)?.Remove();
    }

    public void LoadFileMotion(string fileName)
    {
        fileLoaded = false;
        string filePath = KHHEditData.FileMotionPath + "/" + fileName;

        //모션 데이터를 로드한다.
        GameObject go1 = Instantiate(editItemPrefabs[0], editItemParent);
        KHHEditItemMotion editItemMotion = go1.GetComponent<KHHEditItemMotion>();
        editItemMotion.Init();
        editItemMotion.LoadItemData(this, filePath + ".csv", fileName, null);
        editItemMotion.Model = modelRecorder.Model;
        editItemList.Add(editItemMotion);

        //보이스 데이터를 로드한다.
        GameObject go2 = Instantiate(editItemPrefabs[1], editItemParent);
        KHHEditItemSound editItemVoice = go2.GetComponent<KHHEditItemSound>();
        editItemVoice.Init();
        editItemVoice.IsVoice = true;
        editItemVoice.LoadItemData(this, filePath + ".wav", fileName, () => fileLoaded = true);
        editItemList.Add(editItemVoice);

        //페어간 정보 획득
        editItemMotion.PairSound = editItemVoice;
        editItemVoice.PairMotion = editItemMotion;
    }

    void RemoveCurSound()
    {
        foreach (var item in editItemList.FindAll(x => x is KHHEditItemSound))
            if (((KHHEditItemSound)item).IsVoice == false) { item.Remove(); break; }
    }

    public void LoadFileSound(string fileName)
    {
        fileLoaded = false;
        string filePath = KHHEditData.FileSoundPath + "/" + fileName;

        //사운드 데이터를 로드한다.
        GameObject go = Instantiate(editItemPrefabs[1], editItemParent);
        KHHEditItemSound editItem = go.GetComponent<KHHEditItemSound>();
        editItem.Init();
        editItem.LoadItemData(this, filePath, fileName, () => fileLoaded = true);
        editItemList.Add(editItem);
    }

    void RemoveCurBackground()
    {
        editItemList.Find(x => x is KHHEditItemBackground)?.Remove();
    }

    public void LoadFileBackground(string fileName)
    {
        fileLoaded = false;
        string filePath = KHHEditData.FileImagePath + "/" + fileName;

        //배경 데이터를 로드한다.
        GameObject go = Instantiate(editItemPrefabs[2], editItemParent);
        KHHEditItemBackground editItem = go.GetComponent<KHHEditItemBackground>();
        editItem.Init();
        editItem.LoadItemData(this, filePath, fileName, () => fileLoaded = true);
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
