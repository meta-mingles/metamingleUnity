using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHDataManager : MonoBehaviour
{
    public Transform content;
    public GameObject dataPrefab;

    protected List<KHHData> khhDatas;

    protected virtual void Start()
    {
        khhDatas = new List<KHHData>();
    }

    public virtual void Refresh()
    {
        foreach (KHHData khhData in khhDatas)
            Destroy(khhData.gameObject);
        khhDatas.Clear();
    }
}
