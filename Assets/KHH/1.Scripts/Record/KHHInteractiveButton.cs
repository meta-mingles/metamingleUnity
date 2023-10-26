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

    //��� �������� ���� ������ ����Ǿ��� �� ȣ��
    public void OnDropItem(GameObject dropItem)
    {
        //��� �������� ���� �̸��� ���´�.
        fileName = dropItem.GetComponent<KHHMotionData>().FileName;
        fileNameText.text = "Link: " + fileName;
    }
}
