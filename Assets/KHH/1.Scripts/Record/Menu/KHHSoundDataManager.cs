﻿using System.Collections;
using System.IO;
using UnityEngine;

public class KHHSoundDataManager : KHHDataManager
{
    public override void Refresh()
    {
        base.Refresh();
        //경로가 없으면 생성
        if (!Directory.Exists(KHHVideoData.FileSoundPath))
            Directory.CreateDirectory(KHHVideoData.FileSoundPath);
        DirectoryInfo di = new DirectoryInfo(KHHVideoData.FileSoundPath);
        foreach (FileInfo file in di.GetFiles())
        {
            var ext = file.Extension.ToLower();
            //사운드 파일인 경우 데이터 아이템 생성
            if (ext == ".wav")
            {
                StartCoroutine(CoLoadSound(file));
            }
        }
    }

    IEnumerator CoLoadSound(FileInfo fileInfo)
    {
        GameObject gameObject = Instantiate(dataPrefab, content);
        KHHSoundData soundData = gameObject.GetComponent<KHHSoundData>();
        soundData.Set(fileInfo.Name, ".wav", this);
        khhDatas.Add(soundData);

        yield return StartCoroutine(SaveLoadWav.Load(fileInfo.FullName, soundData.AudioSource));
    }
}
