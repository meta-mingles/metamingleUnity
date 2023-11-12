using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class KHHVideoDataManager : KHHDataManager
{
    public override void Refresh()
    {
        base.Refresh();
        //경로가 없으면 생성
        if (!Directory.Exists(KHHEditData.FileVideoPath))
            Directory.CreateDirectory(KHHEditData.FileVideoPath);
        DirectoryInfo di = new DirectoryInfo(KHHEditData.FileVideoPath);
        foreach (FileInfo file in di.GetFiles())
        {
            var ext = file.Extension.ToLower();
            //사운드 파일인 경우 데이터 아이템 생성
            if (ext == ".mp4")
            {
                LoadVideo(file, ext);
            }
        }
    }

    void LoadVideo(FileInfo fileInfo, string fileExtension)
    {
        GameObject gameObject = Instantiate(dataPrefab, content);
        KHHVideoData videoData = gameObject.GetComponent<KHHVideoData>();
        string fileName = fileInfo.Name.Replace(fileExtension, "");
        videoData.Set(fileName, fileExtension, this);
        khhDatas.Add(videoData);
    }
}
