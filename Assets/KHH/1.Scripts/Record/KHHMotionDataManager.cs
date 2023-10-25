using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KHHMotionDataManager : MonoBehaviour
{
    public GameObject motionDataPrefab;

    List<KHHMotionData> kHHMotionDatas;

    // Start is called before the first frame update
    void Start()
    {
        kHHMotionDatas = new List<KHHMotionData>();
        Refresh();
    }

    /// <summary>
    /// 새로고침
    /// </summary>
    public void Refresh()
    {
        foreach (KHHMotionData motionData in kHHMotionDatas)
            Destroy(motionData.gameObject);
        kHHMotionDatas.Clear();

        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
        foreach (FileInfo file in di.GetFiles("*.csv"))
        {
            GameObject gameObject = Instantiate(motionDataPrefab, this.transform);
            KHHMotionData motionData = gameObject.GetComponent<KHHMotionData>();
            motionData.Set(file.Name);

            kHHMotionDatas.Add(motionData);
        }
    }
}
