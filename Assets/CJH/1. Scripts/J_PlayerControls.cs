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
    private float playerSpeed = 2.0f; // �÷��̾� �̵� �ӵ�

    [SerializeField]
    private float runSpeed = 10f;

    [SerializeField]
    private float jumpHeight = 1.0f; // ���� ����

    [SerializeField]
    private float gravityValue = -9.81f; // �߷� ���ӵ�

    [SerializeField]
    private float rotationSpeed = 4f; // �÷��̾� ȸ�� �ӵ�

    private CharacterController cc; // CharacterController ������Ʈ
    private Vector3 playerVelocity; // �÷��̾��� ���� �ӵ�
    private bool groundedPlayer; // �÷��̾ ���鿡 �ִ��� ����

    private Transform cameraMainTransform; // ���� ī�޶��� Transform

    Animator anim; //�ִϸ�����

    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        cc = gameObject.GetComponent<CharacterController>(); // CharacterController ������Ʈ ��������
        cameraMainTransform = Camera.main.transform; // ���� ī�޶��� Transform ��������
        anim = GetComponent<Animator>(); //�ִϸ�����
        anim.Play("Idle");
    }
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // ���� �̵� �Է°�
        verticalInput = Input.GetAxis("Vertical"); // ���� �̵� �Է°�
        Vector3 move = cameraMainTransform.forward * verticalInput + cameraMainTransform.right * horizontalInput;
        move.y = 0;

        groundedPlayer = cc.isGrounded; // �÷��̾ ���鿡 �ִ��� Ȯ��


        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f; // �÷��̾ ���� ���� ��� ���� �ӵ��� 0���� �ʱ�ȭ
        }

        if (Input.GetButtonDown("Jump")) // ���� �Է��� Ȱ��ȭ�ǰ� �÷��̾ ���鿡 �ִ� ���
        {
           if(groundedPlayer) {
                    UpdateJump();
            }
            else
            {
                // �����̸鼭 ������ ������ �� �ֵ��� ���ϴ� �ٸ� ������ �߰��ϰų� ������ �ۼ�
            }

        }
 


        playerVelocity.y += gravityValue * Time.deltaTime; // �߷��� �����Ͽ� ���� �ӵ� ������Ʈ
        cc.Move(playerVelocity * Time.deltaTime); // �÷��̾��� ���� �̵� ������Ʈ

        if (horizontalInput != 0 || verticalInput != 0) // �̵� �Է��� �ִ� ���
        {

            if (move != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            }
                UpdateMove(move);
                //Debug.Log("�ȱ� �ִ� ����");

            if ( Input.GetKey(KeyCode.LeftShift))
            {
                UpdateRun(move);
            }
        }
        else
        {
            anim.SetBool("Walking", false);
            //Debug.Log("�ȱ� �ִ� ���� ��");
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
    //    cc.Move(move * Time.deltaTime * playerSpeed); // �÷��̾� �̵�
    //    float targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y; // �̵� �������� ȸ�� ���� ���
    //    Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f); // ��ǥ ȸ�� Quaternion ���
    //    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed); // �ε巯�� ȸ�� ����
    //}



    void UpdateJump()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue); // ���� ���̿� ���� ���� �ӵ� ����
        anim.SetTrigger("Jump");
    }
}
