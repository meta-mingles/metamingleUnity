using UnityEngine;

[RequireComponent(typeof(CharacterController))] // CharacterController  컴포넌트 필요
public class J_CameraController : MonoBehaviour
{
    public float speed = 7.5f; // 이동 속도
    public float jumpSpeed = 8.0f; // 점프 속도
    public float gravity = 20.0f; // 중력 값
    public Transform playerCameraParent; // 플레이어 카메라의 부모 트랜스폼
    public float lookSpeed = 2.0f; // 카메라 회전 속도
    public float lookXLimit = 60.0f; // 카메라 X축 회전 제한

    CharacterController cc; // CharacterController 컴포넌트
    Vector3 moveDirection = Vector3.zero; // 이동 방향
    Vector2 rotation = Vector2.zero; // 회전 값

    [HideInInspector]
    public bool canMove = true; // 이동 가능 여부

    void Start()
    {
        cc = GetComponent<CharacterController>(); // CharacterController 컴포넌트 가져오기
        rotation.y = transform.eulerAngles.y; // 초기 회전 값을 현재 Y 축 회전 값으로 설정
    }

    void Update()
    {
        if (cc.isGrounded)
        {
            // 바닥에 있을 때, 축에 기반한 이동 방향 다시 계산
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            Vector3 right = transform.TransformDirection(Vector3.right);

            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;


            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpSpeed; // 점프 입력이 있을 때, 수직 이동 속도 설정
            }
        }

        // 중력 적용. 중력은 deltaTime을 두 번 곱함 (여기서 한 번, 아래에서 이동 방향에 deltaTime을 다시 곱함). 중력은 가속도로 적용되어야 함 (m/s^2)
        moveDirection.y -= gravity * Time.deltaTime;

        // 컨트롤러 이동
        cc.Move(moveDirection * Time.deltaTime);

        // 플레이어와 카메라 회전
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed; // 마우스 X 축 입력을 이용한 Y 축 회전 값 계산
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed; // 마우스 Y 축 입력을 이용한 X 축 회전 값 계산
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit); // X 축 회전 값의 제한 설정
            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0); // 카메라의 로컬 회전 설정
            transform.eulerAngles = new Vector2(0, rotation.y); // 플레이어의 회전 값 설정
        }
    }
}