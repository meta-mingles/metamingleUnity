using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TestPlayercontrol : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 2.0f; // �÷��̾� �̵� �ӵ�

    [SerializeField]
    private float runSpeed = 10f;

    [SerializeField]
    private float jumpHeight = 1.0f; // ���� ����

    [SerializeField]
    private float gravityValue = -9.81f; // �߷� ���ӵ�

    [SerializeField]
    private float rotationSpeed = 2f; // �÷��̾� ȸ�� �ӵ�

    private CharacterController cc; // CharacterController ������Ʈ
    private Vector3 playerVelocity; // �÷��̾��� ���� �ӵ�
    private bool groundedPlayer; // �÷��̾ ���鿡 �ִ��� ����

    private Transform trCam; // ���� ī�޶��� Transform

    Animator anim; //�ִϸ�����

    //public GameObject voice; //���̽� 

    private float horizontalInput;
    private float verticalInput;


    private enum State
    {
        Idle,
        Walk,
        Run,
        Jump,
        Sit
         
    }
    private State currentState;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>(); // CharacterController ������Ʈ ��������
        trCam = Camera.main.transform; // ���� ī�޶��� Transform ��������
        anim = GetComponent<Animator>(); //�ִϸ�����
    }
    private void Start()
    {
        //UpdateIdle();
    }
    void Update()
    {

        //���࿡ ���콺 Ŀ���� Ȱ��ȭ�Ǿ������� �Լ��� ������
        if (Cursor.visible == true) return;

        horizontalInput = Input.GetAxis("Horizontal"); // ���� �̵� �Է°�
        verticalInput = Input.GetAxis("Vertical"); // ���� �̵� �Է°�
        Vector3 move = trCam.forward * verticalInput + trCam.right * horizontalInput;
        move.y = 0;

        groundedPlayer = cc.isGrounded; // �÷��̾ ���鿡 �ִ��� Ȯ��

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f; // �÷��̾ ���� ���� ��� ���� �ӵ��� 0���� �ʱ�ȭ
        }

        if (!groundedPlayer && Input.GetButtonDown("Jump"))
        {
            SetPlayerState(State.Jump);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            SetPlayerState(State.Idle);
        }

        if (horizontalInput != 0 || verticalInput != 0) // �̵� �Է��� �ִ� ���
        {

            if (move != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg + trCam.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                SetPlayerState(State.Run);
            }
            else
            {
                SetPlayerState(State.Walk);
            }
        }
        else
        {
            SetPlayerState(State.Idle);
            //Debug.Log("�ȱ� �ִ� ���� ��");
        }
        playerVelocity.y += gravityValue * Time.deltaTime; // �߷��� �����Ͽ� ���� �ӵ� ������Ʈ
        cc.Move(playerVelocity * Time.deltaTime); // �÷��̾��� ���� �̵� ������Ʈ
    }
    private Vector3 CalMoveVector(float horizontal, float vertical)
    {
       return trCam.forward* vertical +trCam.right * horizontal;
    }

    void SetPlayerState(State state)
    {
        if(currentState != state)
        {
            currentState = state;

            switch (state)
            {
                case State.Idle:
                    UpdateIdle();
                    Debug.Log("Idle");
                    break;
                case State.Walk:
                    UpdateWalk (CalMoveVector(horizontalInput, verticalInput));
                    break;
                case State.Run:
                    UpdateRun(CalMoveVector(horizontalInput, verticalInput));
                    break;
                case State.Jump:
                    UpdateJump();
                    break;
                case State.Sit:
                    UpdateSit();
                    break;
            }
        }
    }


    void UpdateIdle()
    {
        Debug.Log("Idle");
        anim.Play("Idle");
    }

    void UpdateWalk(Vector3 move)
    {
        cc.Move(move * Time.deltaTime * playerSpeed);
        anim.SetBool("Walk", true);
    }
    void UpdateRun(Vector3 move)
    {
        cc.Move(move * Time.deltaTime * runSpeed);
        anim.SetTrigger("Run");
    }
    void UpdateJump()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue); // ���� ���̿� ���� ���� �ӵ� ����
        Debug.Log("Jump");
        anim.CrossFade("Jumping", 0, 0);
    }
    private void UpdateSit()
    {

    }
}
