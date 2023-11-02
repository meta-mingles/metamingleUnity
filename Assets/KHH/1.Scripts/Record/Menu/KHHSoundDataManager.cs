using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class KHHSoundDataManager : KHHDataManager
{
    List<KHHSoundData> soundDatas;
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Refresh()
    {
        foreach (var soundData in soundDatas)
            Destroy(soundData.gameObject);
        soundDatas.Clear();

        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + "/" + KHHEditManager.Instance.videoName + "/Sounds");
        foreach (FileInfo file in di.GetFiles())
        {
            var ext = file.Extension.ToLower();
            //사운드 파일인 경우 데이터 아이템 생성
            if (ext == "wav")
            {
                StartCoroutine(CoLoadSound(file.FullName, file.Name));
            }
        }
    }
    IEnumerator CoLoadSound(string filePath, string fileName)
    {
        GameObject gameObject = Instantiate(dataPrefab, content);
        KHHSoundData soundData = gameObject.GetComponent<KHHSoundData>();
        soundData.Set(fileName);
        soundDatas.Add(soundData);

        yield return StartCoroutine(SaveLoadWav.Load(filePath, soundData.AudioSource));
    }
}
