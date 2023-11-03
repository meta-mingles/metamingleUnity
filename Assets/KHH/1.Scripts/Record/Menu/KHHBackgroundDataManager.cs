using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class KHHBackgroundDataManager : KHHDataManager
{
    /// <summary>
    /// 새로고침
    /// </summary>
    public override void Refresh()
    {
        base.Refresh();
        //경로가 없으면 생성
        if (!Directory.Exists(KHHVideoData.FileImagePath))
            Directory.CreateDirectory(KHHVideoData.FileImagePath);
        DirectoryInfo di = new DirectoryInfo(KHHVideoData.FileImagePath);
        foreach (FileInfo file in di.GetFiles())
        {
            var ext = file.Extension.ToLower();
            //이미지 파일인 경우 데이터 아이템 생성
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
            {
                //이미지를 불러온다
                byte[] bytes = File.ReadAllBytes(file.FullName);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);

                GameObject gameObject = Instantiate(dataPrefab, content);

                KHHBackgroundData backgroundData = gameObject.GetComponent<KHHBackgroundData>();
                string fileName = file.Name.Replace(ext, "");
                backgroundData.Set(fileName, ext, this, texture);
                khhDatas.Add(backgroundData);
            }
        }
    }
}
