using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    public GameObject target; // 카메라가 따라갈 대상 (플레이어)
    public Vector3 offset; // 카메라와 대상 간의 오프셋
    public float rotX,rotY; //누적 회전값

    public float rotSpeed = 200;//회전 속력
    //카메라 transform
    public Transform trCam;
    //public float rotationSpeed = 2.0f; // 카메라 회전 속도

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // 커서 고정
        //Cursor.visible = false; // 커서 숨김
        offset = transform.position - target.transform.position;
    }

    private void Update()
    {
        //1. 마우스 입력을 받자.
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        //2. 마우스의 움직임 값을 누적
        rotX += mx * rotSpeed * Time.deltaTime;
        rotY += my * rotSpeed * Time.deltaTime;

        //3. 누적된값만큼 회전
        transform.localEulerAngles = new Vector3(0, rotX, 0);
        trCam.localEulerAngles = new Vector3(-rotY, 0, 0);
    }

    private void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}
