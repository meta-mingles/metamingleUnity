using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class J_PlayerControls : MonoBehaviourPun
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

    private float horizontalInput;
    private float verticalInput;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>(); // CharacterController ������Ʈ ��������
        trCam = Camera.main.transform; // ���� ī�޶��� Transform ��������
        anim = GetComponent<Animator>(); //�ִϸ�����

    }
    private void Start()
    {
        UpdateIdle();

        //���� ������ �÷��̾��϶��� ī�޶� Ȱ��ȭ
        if (photonView.IsMine)
        {
            trCam.gameObject.SetActive(true);
        }
    }
    void Update()
    {
        //���� ���� �÷��̾���
        if (photonView.IsMine)
        {

            horizontalInput = Input.GetAxis("Horizontal"); // ���� �̵� �Է°�
            verticalInput = Input.GetAxis("Vertical"); // ���� �̵� �Է°�
            Vector3 move = trCam.forward * verticalInput + trCam.right * horizontalInput;
            move.y = 0;

            groundedPlayer = cc.isGrounded; // �÷��̾ ���鿡 �ִ��� Ȯ��

            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f; // �÷��̾ ���� ���� ��� ���� �ӵ��� 0���� �ʱ�ȭ
            }

            if (!groundedPlayer && Input.GetButtonDown("Jump")) // ���� �Է��� Ȱ��ȭ�ǰ� �÷��̾ ���鿡 �ִ� ���
            {
                UpdateJump();
            }
            else if (Input.GetButtonUp("Jump"))
            {
                UpdateIdle();
            }

            if (horizontalInput != 0 || verticalInput != 0) // �̵� �Է��� �ִ� ���
            {

                if (move != Vector3.zero)
                {
                    float targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg + trCam.eulerAngles.y;
                    Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                }
                UpdateMove(move);
                //Debug.Log("�ȱ� �ִ� ����");

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    UpdateRun(move);
                }
            }
            else
            {
                anim.SetBool("Walking", false);
                //Debug.Log("�ȱ� �ִ� ���� ��");
            }
            playerVelocity.y += gravityValue * Time.deltaTime; // �߷��� �����Ͽ� ���� �ӵ� ������Ʈ
            cc.Move(playerVelocity * Time.deltaTime); // �÷��̾��� ���� �̵� ������Ʈ

        }
        //���� �÷��̾ �ƴ϶��
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
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue); // ���� ���̿� ���� ���� �ӵ� ����
        Debug.Log("Jump");
        anim.CrossFade("Jumping", 0, 0);
    }
}