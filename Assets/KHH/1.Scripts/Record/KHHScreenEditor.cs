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

    //드롭 아이템이 편집 영역에 드랍되었을 때 호출
    public void OnDropItem(GameObject dropItem)
    {
        //드롭 아이템의 파일 이름을 얻어온다.
        string fileName = dropItem.GetComponent<KHHMotionData>().FileName;

        //모델 레코더에 파일 이름을 전달한다.
        recorder.LoadRecordData(fileName);
    }
}
