using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_CameraCollision : MonoBehaviour
{
    public Transform referenceTransform; 
    public float collisionOffset = 0.3f; //카메라가 클리핑하지 못하도록 하는 충돌설정
    public float cameraSpeed = 15f; //카메라가 돌아오는 속도

    Vector3 defaultPos; //원래 카메라 위치
    Vector3 directionNormalized; //정규화 벡터
    Transform parentTransform; //부모
    float defaultDistance; //원래 거리

    // Start is called before the first frame update
    void Start()
    {
        defaultPos = transform.localPosition; //원래 카메라 위치 저장
        directionNormalized = defaultPos.normalized; //방향벡터 저장
        parentTransform = transform.parent; // 부모위치 저장
        defaultDistance = Vector3.Distance(defaultPos, Vector3.zero); //원래 거리 계산

        //마우스 커서 잠금 설정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // LateUpdate is called after Update
    void LateUpdate()
    {
        Vector3 currentPos = defaultPos; //현재 카메라 위치 초기화
        RaycastHit hit;
        Vector3 dirTmp = parentTransform.TransformPoint(defaultPos) - referenceTransform.position; //방향 벡터 계산
        if (Physics.SphereCast(referenceTransform.position, collisionOffset, dirTmp, out hit, defaultDistance))
        {
            //충돌 감지 시 새로운 위치 계산
            currentPos = (directionNormalized * (hit.distance - collisionOffset)); 
            //카메라 위치 설정
            transform.localPosition = currentPos;
        }
        else
        {
            //충돌 감지 되지 않으면 Lerp하게 원래 위치로 돌아감
            transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, Time.deltaTime * cameraSpeed);
        }
    }
}