using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KHHMotionDataManager : KHHDataManager
{
    /// <summary>
    /// 새로고침
    /// </summary>
    public override void Refresh()
    {
        base.Refresh();
        //경로가 없으면 생성
        if (!Directory.Exists(KHHEditData.FileMotionPath))
            Directory.CreateDirectory(KHHEditData.FileMotionPath);
        DirectoryInfo di = new DirectoryInfo(KHHEditData.FileMotionPath);
        if (!di.Exists)
            return;

        foreach (FileInfo file in di.GetFiles("*.csv"))
        {
            GameObject gameObject = Instantiate(dataPrefab, content);
            KHHMotionData motionData = gameObject.GetComponent<KHHMotionData>();
            string fileName = file.Name.Replace(".csv", "");
            motionData.Set(fileName, ".csv", this);

            khhDatas.Add(motionData);
        }
    }
}
