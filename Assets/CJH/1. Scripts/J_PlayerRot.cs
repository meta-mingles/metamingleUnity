using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_PlayerRot : MonoBehaviour
{
    //누적된 회전값
    float rotX, rotY;
    //회전 속력
    float rotSpeed = 200;

    //카메라 Transform
    public Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //1. 마우스 입력을 받자
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        //2. 마우스 움직임 값 누적
        rotX += mx * rotSpeed * Time.deltaTime;
        rotY += my * rotSpeed * Time.deltaTime;
        //3. 누적된 값만큼 회전
        transform.localEulerAngles = new Vector3(0,rotX,0);
        cameraTransform.localEulerAngles = new Vector3(-rotY, 0, 0);
    }
}
