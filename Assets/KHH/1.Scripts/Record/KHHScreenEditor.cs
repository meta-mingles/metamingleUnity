using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHScreenEditor : MonoBehaviour
{
    public KHHModelRecorder recorder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //��� �������� ���� ������ ����Ǿ��� �� ȣ��
    public void OnDropItem(GameObject dropItem)
    {
        //��� �������� ���� �̸��� ���´�.
        string fileName = dropItem.GetComponent<KHHMotionData>().FileName;

        //�� ���ڴ��� ���� �̸��� �����Ѵ�.
        recorder.LoadRecordData(fileName);
    }
}
