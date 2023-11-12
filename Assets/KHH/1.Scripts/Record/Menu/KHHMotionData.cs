using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KHHMotionData : KHHData
{
    public void Update()
    {
        if (IsSelected)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                //파일 삭제
                File.Delete(KHHEditData.FileMotionPath + "/" + fileName + ".csv");
                File.Delete(KHHEditData.FileMotionPath + "/" + fileName + ".wav");
                khhDataManager.Refresh();
            }
        }
    }
}
