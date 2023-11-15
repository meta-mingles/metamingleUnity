using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_CustomCharacter : MonoBehaviour
{
    void Start()
    {
        //load data
        KHHUserCustom.Init();
        KHHUserCustomData data = KHHUserCustom.LoadData();
    }

    void Update()
    {
        
    }
}
