using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_CameraCollision : MonoBehaviour
{
    public Transform referenceTransform; 
    public float collisionOffset = 0.3f; //ī�޶� Ŭ�������� ���ϵ��� �ϴ� �浹����
    public float cameraSpeed = 15f; //ī�޶� ���ƿ��� �ӵ�

    Vector3 defaultPos; //���� ī�޶� ��ġ
    Vector3 directionNormalized; //����ȭ ����
    Transform parentTransform; //�θ�
    float defaultDistance; //���� �Ÿ�

    // Start is called before the first frame update
    void Start()
    {
        defaultPos = transform.localPosition; //���� ī�޶� ��ġ ����
        directionNormalized = defaultPos.normalized; //���⺤�� ����
        parentTransform = transform.parent; // �θ���ġ ����
        defaultDistance = Vector3.Distance(defaultPos, Vector3.zero); //���� �Ÿ� ���

        //���콺 Ŀ�� ��� ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // LateUpdate is called after Update
    void LateUpdate()
    {
        Vector3 currentPos = defaultPos; //���� ī�޶� ��ġ �ʱ�ȭ
        RaycastHit hit;
        Vector3 dirTmp = parentTransform.TransformPoint(defaultPos) - referenceTransform.position; //���� ���� ���
        if (Physics.SphereCast(referenceTransform.position, collisionOffset, dirTmp, out hit, defaultDistance))
        {
            //�浹 ���� �� ���ο� ��ġ ���
            currentPos = (directionNormalized * (hit.distance - collisionOffset)); 
            //ī�޶� ��ġ ����
            transform.localPosition = currentPos;
        }
        else
        {
            //�浹 ���� ���� ������ Lerp�ϰ� ���� ��ġ�� ���ư�
            transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, Time.deltaTime * cameraSpeed);
        }
    }
}