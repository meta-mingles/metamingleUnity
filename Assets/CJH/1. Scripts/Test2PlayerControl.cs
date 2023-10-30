using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class Test2PlayerControl : MonoBehaviour
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
    public GameObject chair; //����

    private float horizontalInput;
    private float verticalInput;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>(); // CharacterController ������Ʈ ��������
        trCam = Camera.main.transform; // ���� ī�޶��� Transform ��������
        anim = GetComponent<Animator>(); //�ִϸ�����

    }
    // Start is called before the first frame update
    void Start()
    {

        //UpdateIdle();
    }

    // Update is called once per frame
    void Update()
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
            anim.SetTrigger("Idle");
            //Debug.Log("�ȱ� �ִ� ���� ��");
        }
        playerVelocity.y += gravityValue * Time.deltaTime; // �߷��� �����Ͽ� ���� �ӵ� ������Ʈ
        cc.Move(playerVelocity * Time.deltaTime); // �÷��̾��� ���� �̵� ������Ʈ

        //�ɱ�
        if(Vector3.Distance(transform.position, chair.transform.position) < 1.5f)
        {
            UpdateSit();
        }
    }
    void UpdateIdle()
    {
        anim.Play("Idle");
    }

    void UpdateMove(Vector3 move)
    {
        cc.Move(move * Time.deltaTime * playerSpeed);
        anim.SetTrigger("Walk");
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
        
    }
    void UpdateSit()
    {
        Debug.Log("Sit ����");
        //���ڿ� ��������� �ɱ� ��ư UI�� Ȱ��ȭ�ǰ� 

        //�ɱ� ��ư E�� ������ ���ڿ� �ɴ´�.
        if (Input.GetKeyDown(KeyCode.E))
        {            
            SitOnChair();
        }
    }
    void SitOnChair()
    {
        //�������� �ִϸ��̼�
        //anim.SetBool("IsSitting", true);
        

        cc.enabled = false;
        //���� ������ y���� 90 �Ȱ��� ����
        RotatePlayer();

        //isSitting �ִϸ��̼��� �ǽõȴ�
        anim.Play("isSitting");


        //transform.position = chair.transform.position + new Vector3(0, 0.5f, 0);
        
        
        //���� �⺻ �ִ�
        //anim.SetBool("SitIdle", true);

    }

    IEnumerator RotatePlayer()
    {
        transform.DORotate(new Vector3(0, 90, 0), 1.7f);
        yield return new WaitForSeconds(1f);
    }
    


}
