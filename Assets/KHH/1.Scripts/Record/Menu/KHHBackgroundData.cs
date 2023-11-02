using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KHHBackgroundData : KHHData
{
    RawImage image;

    private void Start()
    {
        image = GetComponent<RawImage>();
    }

    public void Set(string fileName, Texture2D texture)
    {
        base.Set(fileName);
        image.texture = texture;
    }
}
