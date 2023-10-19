using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_PlayerRot : MonoBehaviour
{
    //������ ȸ����
    float rotX, rotY;
    //ȸ�� �ӷ�
    float rotSpeed = 200;

    //ī�޶� Transform
    public Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //1. ���콺 �Է��� ����
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        //2. ���콺 ������ �� ����
        rotX += mx * rotSpeed * Time.deltaTime;
        rotY += my * rotSpeed * Time.deltaTime;
        //3. ������ ����ŭ ȸ��
        transform.localEulerAngles = new Vector3(0,rotX,0);
        cameraTransform.localEulerAngles = new Vector3(-rotY, 0, 0);
    }
}
