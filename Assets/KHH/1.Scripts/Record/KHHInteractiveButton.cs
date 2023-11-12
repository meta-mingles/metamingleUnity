using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KHHInteractiveButton : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI fileNameText;

    string fileName;
    public string FileName { get { return fileName; } }
    public string Title { get { return inputField.text; } }
    string filePath;
    public string FilePath { get { return filePath; } }

    //드롭 아이템이 편집 영역에 드랍되었을 때 호출
    public void OnDropItem(GameObject dropItem)
    {
        //드롭 아이템의 파일 이름을 얻어온다.
        fileName = dropItem.GetComponent<KHHVideoData>().FileName;
        filePath = dropItem.GetComponent<KHHVideoData>().FilePath;
        fileNameText.text = "Link: " + fileName;
    }
}
