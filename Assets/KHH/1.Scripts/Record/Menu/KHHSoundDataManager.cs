using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KHHSoundDataManager : KHHDataManager
{
    [Header("Volume")]
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI volumeText;
    [SerializeField] Slider volumeSlider;

    KHHEditItemSound soundItem;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener((value) =>
        {
            if (soundItem != null)
            {
                soundItem.Volume = value;
                volumeText.text = string.Format("볼륨:{0}", (int)(value * 100));
            }
        });
    }

    public override void Refresh()
    {
        base.Refresh();
        //경로가 없으면 생성
        if (!Directory.Exists(KHHEditData.FileSoundPath))
            Directory.CreateDirectory(KHHEditData.FileSoundPath);
        DirectoryInfo di = new DirectoryInfo(KHHEditData.FileSoundPath);
        foreach (FileInfo file in di.GetFiles())
        {
            var ext = file.Extension.ToLower();
            //사운드 파일인 경우 데이터 아이템 생성
            if (ext == ".wav")
            {
                StartCoroutine(CoLoadSound(file, ext));
            }
        }
    }

    IEnumerator CoLoadSound(FileInfo fileInfo, string fileExtension)
    {
        GameObject gameObject = Instantiate(dataPrefab, content);
        KHHSoundData soundData = gameObject.GetComponent<KHHSoundData>();
        string fileName = fileInfo.Name.Replace(fileExtension, "");
        soundData.Set(fileName, fileExtension, this);
        khhDatas.Add(soundData);

        yield return StartCoroutine(SaveLoadWav.Load(fileInfo.FullName, soundData.AudioSource));
    }

    public void SetSelectedData(KHHEditItemSound item)
    {
        soundItem = item;
        nameText.text = item.FileName;
        volumeText.text = string.Format("볼륨:{0}", (int)(item.Volume * 100));
        volumeSlider.value = item.Volume;
    }
}
