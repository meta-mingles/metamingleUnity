using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Camera))]
public class J_TpsCamera : MonoBehaviourPun
{
    public float moveSpeed;               // 이동 속도
    public float shiftAdditionalSpeed;    // Shift 키 추가 속도
    public float mouseSensitivity;        // 마우스 감도
    public bool invertMouse;              // 마우스 반전 여부
    public bool autoLockCursor;           // 자동 커서 락 여부

    public Transform target; //플레이어
    public Vector3 offset;

    private Camera cam;  // 카메라 컴포넌트에 대한 참조

   
    void Awake()
    {
        //target = playerControls.GetComponent<J_PlayerControls>().transform;
        cam = this.gameObject.GetComponent<Camera>();  // 카메라 컴포넌트에 대한 참조 획득
        this.gameObject.name = "SpectatorCamera";      // 게임 오브젝트 이름 설정
        //Cursor.lockState = (autoLockCursor) ? CursorLockMode.Locked : CursorLockMode.None;  // 커서 락 설정
    }
    public void Start()
    {
        offset = transform.position - target.transform.position;
    }

    void Update()
    {
        ////내것이 아닐때 함수를 나가자
        //if (photonView.IsMine == false) { return; }


        float speed = (moveSpeed + (Input.GetAxis("Fire3") * shiftAdditionalSpeed));  // 이동 속도 계산

        // 키 입력에 따라 카메라 이동
        this.gameObject.transform.Translate(Vector3.forward * speed * Input.GetAxis("Vertical"));
        this.gameObject.transform.Translate(Vector3.right * speed * Input.GetAxis("Horizontal"));
        this.gameObject.transform.Translate(Vector3.up * speed * (Input.GetAxis("Jump") + (Input.GetAxis("Fire1") * -1)));


        // 마우스 입력에 따라 카메라 회전
        this.gameObject.transform.Rotate(Input.GetAxis("Mouse Y") * mouseSensitivity * ((invertMouse) ? 1 : -1), Input.GetAxis("Mouse X") * mouseSensitivity * ((invertMouse) ? -1 : 1), 0);

        // 카메라 회전 강제 제한
        this.gameObject.transform.localEulerAngles = new Vector3(this.gameObject.transform.localEulerAngles.x, this.gameObject.transform.localEulerAngles.y, 0);




        #region 커서 락
        //// 커서 락 상태 변경
        //if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        //{
        //    Cursor.lockState = CursorLockMode.Locked;  // 커서 락 설정
        //}
        //else if (Cursor.lockState == CursorLockMode.Locked && Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Cursor.lockState = CursorLockMode.None;  // 커서 락 해제
        //}
        #endregion

    }

    private void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }

}
