using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class KHHBackgroundDataManager : MonoBehaviour
{
    public GameObject backgroundDataPrefab;

    List<RawImage> backgroundImages;

    // Start is called before the first frame update
    void Start()
    {
        backgroundImages = new List<RawImage>();
    }

    /// <summary>
    /// 새로고침
    /// </summary>
    public void Refresh()
    {
        foreach (var backgroundImage in backgroundImages)
            Destroy(backgroundImage.gameObject);
        backgroundImages.Clear();

        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + "/Images");
        foreach (FileInfo file in di.GetFiles("*.jpg"))
        {
            GameObject gameObject = Instantiate(backgroundDataPrefab, this.transform);

            //이미지를 불러온다
            byte[] bytes = File.ReadAllBytes(file.FullName);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);

            RawImage rawImage = gameObject.GetComponent<RawImage>();
            rawImage.texture = texture;

            backgroundImages.Add(rawImage);
        }
    }
}
