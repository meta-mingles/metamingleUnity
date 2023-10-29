using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class J_PlayerControls : MonoBehaviourPun
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
    private float rotationSpeed = 2f; // 플레이어 회전 속도

    private CharacterController cc; // CharacterController 컴포넌트
    private Vector3 playerVelocity; // 플레이어의 현재 속도
    private bool groundedPlayer; // 플레이어가 지면에 있는지 여부

    private Transform trCam; // 메인 카메라의 Transform

    Animator anim; //애니메이터

    private float horizontalInput;
    private float verticalInput;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>(); // CharacterController 컴포넌트 가져오기
        trCam = Camera.main.transform; // 메인 카메라의 Transform 가져오기
        anim = GetComponent<Animator>(); //애니메이터

    }
    private void Start()
    {
        UpdateIdle();

        //내가 생성한 플레이어일때만 카메라 활성화
        if (photonView.IsMine)
        {
            trCam.gameObject.SetActive(true);
        }
    }
    void Update()
    {
        //내가 만든 플레이어라면
        if (photonView.IsMine)
        {

            horizontalInput = Input.GetAxis("Horizontal"); // 수평 이동 입력값
            verticalInput = Input.GetAxis("Vertical"); // 수직 이동 입력값
            Vector3 move = trCam.forward * verticalInput + trCam.right * horizontalInput;
            move.y = 0;

            groundedPlayer = cc.isGrounded; // 플레이어가 지면에 있는지 확인

            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f; // 플레이어가 땅에 닿은 경우 수직 속도를 0으로 초기화
            }

            if (!groundedPlayer && Input.GetButtonDown("Jump")) // 점프 입력이 활성화되고 플레이어가 지면에 있는 경우
            {
                UpdateJump();
            }
            else if (Input.GetButtonUp("Jump"))
            {
                UpdateIdle();
            }

            if (horizontalInput != 0 || verticalInput != 0) // 이동 입력이 있는 경우
            {

                if (move != Vector3.zero)
                {
                    float targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg + trCam.eulerAngles.y;
                    Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                }
                UpdateMove(move);
                //Debug.Log("걷기 애니 실행");

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    UpdateRun(move);
                }
            }
            else
            {
                anim.SetBool("Walking", false);
                //Debug.Log("걷기 애니 실행 끝");
            }
            playerVelocity.y += gravityValue * Time.deltaTime; // 중력을 적용하여 수직 속도 업데이트
            cc.Move(playerVelocity * Time.deltaTime); // 플레이어의 수직 이동 업데이트

        }
        //나의 플레이어가 아니라면
        else
        {

        }

    }

    void UpdateIdle()
    {
        anim.Play("Idle");
    }

    void UpdateMove(Vector3 move)
    {
        cc.Move(move * Time.deltaTime * playerSpeed);
        anim.SetBool("Walking", true);
    }
    void UpdateRun(Vector3 move)
    {
        cc.Move(move * Time.deltaTime * runSpeed);
        anim.SetTrigger("Run");
    }
    void UpdateJump()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue); // 점프 높이에 따른 수직 속도 설정
        Debug.Log("Jump");
        anim.CrossFade("Jumping", 0, 0);
    }
}