using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KHHMotionData : KHHData
{
    public TextMeshProUGUI dataNameText;

    public override void Set(string fileName)
    {
        base.Set(fileName);
        dataNameText.text = fileName;
    }
}
