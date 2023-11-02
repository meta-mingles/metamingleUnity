using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KHHDataManager : MonoBehaviour
{
    public Transform content;
    public GameObject dataPrefab;

    public abstract void Refresh();
}
