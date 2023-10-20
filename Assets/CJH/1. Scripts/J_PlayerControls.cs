using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngineInternal;

[RequireComponent(typeof(CharacterController))]
public class J_PlayerControls : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private float playerSpeed = 2.0f; // 플레이어 이동 속도

    [SerializeField]
    private float runSpeed = 10f;

    [SerializeField]
    private float jumpHeight = 1.0f; // 점프 높이

    [SerializeField]
    private float gravityValue = -9.81f; // 중력 가속도

    [SerializeField]
    private float rotationSpeed = 4f; // 플레이어 회전 속도

    private CharacterController cc; // CharacterController 컴포넌트
    private Vector3 playerVelocity; // 플레이어의 현재 속도
    private bool groundedPlayer; // 플레이어가 지면에 있는지 여부

    private Transform cameraMainTransform; // 메인 카메라의 Transform

    Animator anim; //애니메이터

    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        cc = gameObject.GetComponent<CharacterController>(); // CharacterController 컴포넌트 가져오기
        cameraMainTransform = Camera.main.transform; // 메인 카메라의 Transform 가져오기
        anim = GetComponent<Animator>(); //애니메이터
        anim.Play("Idle");
    }
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // 수평 이동 입력값
        verticalInput = Input.GetAxis("Vertical"); // 수직 이동 입력값
        Vector3 move = cameraMainTransform.forward * verticalInput + cameraMainTransform.right * horizontalInput;
        move.y = 0;

        groundedPlayer = cc.isGrounded; // 플레이어가 지면에 있는지 확인


        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f; // 플레이어가 땅에 닿은 경우 수직 속도를 0으로 초기화
        }

        if (Input.GetButtonDown("Jump")) // 점프 입력이 활성화되고 플레이어가 지면에 있는 경우
        {
           if(groundedPlayer) {
                    UpdateJump();
            }
            else
            {
                // 움직이면서 점프를 실행할 수 있도록 원하는 다른 동작을 추가하거나 로직을 작성
            }

        }
 


        playerVelocity.y += gravityValue * Time.deltaTime; // 중력을 적용하여 수직 속도 업데이트
        cc.Move(playerVelocity * Time.deltaTime); // 플레이어의 수직 이동 업데이트

        if (horizontalInput != 0 || verticalInput != 0) // 이동 입력이 있는 경우
        {

            if (move != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            }
                UpdateMove(move);
                //Debug.Log("걷기 애니 실행");

            if ( Input.GetKey(KeyCode.LeftShift))
            {
                UpdateRun(move);
            }
        }
        else
        {
            anim.SetBool("Walking", false);
            //Debug.Log("걷기 애니 실행 끝");
        }


    }
    void UpdateMove(Vector3 move)
    {
        cc.Move(move * Time.deltaTime * playerSpeed);
        anim.SetBool("Walking",true);
    }
    void UpdateRun(Vector3 move)
    {
        cc.Move(move * Time.deltaTime * runSpeed);
        anim.SetTrigger("Run");
    }

    //void UpdateWalk(Vector3 move)
    //{
    //    cc.Move(move * Time.deltaTime * playerSpeed); // 플레이어 이동
    //    float targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y; // 이동 방향으로 회전 각도 계산
    //    Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f); // 목표 회전 Quaternion 계산
    //    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed); // 부드러운 회전 적용
    //}



    void UpdateJump()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue); // 점프 높이에 따른 수직 속도 설정
        anim.SetTrigger("Jump");
    }
}
