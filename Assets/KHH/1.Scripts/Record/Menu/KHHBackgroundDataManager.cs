using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class KHHBackgroundDataManager : KHHDataManager
{
    List<KHHBackgroundData> backgroundDatas;

    // Start is called before the first frame update
    void Start()
    {
        backgroundDatas = new List<KHHBackgroundData>();
    }

    /// <summary>
    /// 새로고침
    /// </summary>
    public override void Refresh()
    {
        foreach (var backgroundData in backgroundDatas)
            Destroy(backgroundData.gameObject);
        backgroundDatas.Clear();

        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + "/" + KHHEditManager.Instance.videoName + "/Images");
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
                backgroundData.Set(file.Name, texture);
                backgroundDatas.Add(backgroundData);
            }
        }
    }
}
