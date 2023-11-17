using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Change_language : MonoBehaviour
{

    public Button myButton; // ��ư ����
    public TextMeshProUGUI buttontext;

    // ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public bool iskorea=true;
    string selected_language;
    public void OnButtonClick()
    {
        iskorea = !iskorea;
        Debug.Log(iskorea);

        if (iskorea)
        {
            selected_language = "�ѱ���";
        }
        else
        {
            selected_language = "english";
        }

        // ��ư ������ Text ������Ʈ ��������

        
        buttontext = myButton.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(buttontext);
        // Text ������Ʈ�� ��ȿ���� Ȯ���ϰ� �ؽ�Ʈ ���� �����մϴ�.
        if (buttontext != null)
        {
            buttontext.text = $"[{selected_language}]�� ����";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
