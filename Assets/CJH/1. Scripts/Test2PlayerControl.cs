using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class Test2PlayerControl : MonoBehaviour
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

    //public GameObject voice; //보이스 
    public GameObject chair; //의자

    private float horizontalInput;
    private float verticalInput;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>(); // CharacterController 컴포넌트 가져오기
        trCam = Camera.main.transform; // 메인 카메라의 Transform 가져오기
        anim = GetComponent<Animator>(); //애니메이터

    }
    // Start is called before the first frame update
    void Start()
    {

        //UpdateIdle();
    }

    // Update is called once per frame
    void Update()
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
            anim.SetTrigger("Idle");
            //Debug.Log("걷기 애니 실행 끝");
        }
        playerVelocity.y += gravityValue * Time.deltaTime; // 중력을 적용하여 수직 속도 업데이트
        cc.Move(playerVelocity * Time.deltaTime); // 플레이어의 수직 이동 업데이트

        //앉기
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
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue); // 점프 높이에 따른 수직 속도 설정
        Debug.Log("Jump");
        
    }
    void UpdateSit()
    {
        Debug.Log("Sit 하자");
        //의자에 가까워지면 앉기 버튼 UI가 활성화되고 

        //앉기 버튼 E를 누르면 의자에 앉는다.
        if (Input.GetKeyDown(KeyCode.E))
        {            
            SitOnChair();
        }
    }
    void SitOnChair()
    {
        //앉으려는 애니메이션
        //anim.SetBool("IsSitting", true);
        

        cc.enabled = false;
        //나의 방향은 y방향 90 똑같이 설정
        RotatePlayer();

        //isSitting 애니메이션이 실시된다
        anim.Play("isSitting");


        //transform.position = chair.transform.position + new Vector3(0, 0.5f, 0);
        
        
        //앉은 기본 애니
        //anim.SetBool("SitIdle", true);

    }

    IEnumerator RotatePlayer()
    {
        transform.DORotate(new Vector3(0, 90, 0), 1.7f);
        yield return new WaitForSeconds(1f);
    }
    


}
