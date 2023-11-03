using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KHHBackgroundData : KHHData
{
    RawImage rawImage;
    //public Texture ImageTexture { get { return rawImage.texture as Texture; } }

    protected override void Awake()
    {
        base.Awake();
        rawImage = GetComponent<RawImage>();
    }

    public void Update()
    {
        if (IsSelected)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                //파일 삭제
                File.Delete(KHHVideoData.FileMotionPath + "/" + fileName + fileExtension);
                khhDataManager.Refresh();
            }
        }
    }

    public void Set(string fileName, string fileExtension, KHHDataManager manager, Texture2D texture)
    {
        base.Set(fileName, fileExtension, manager);
        this.fileExtension = fileExtension;
        rawImage.texture = texture;
    }
}
