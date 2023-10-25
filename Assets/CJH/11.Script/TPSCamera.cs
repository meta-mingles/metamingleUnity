using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    public GameObject target; // ī�޶� ���� ��� (�÷��̾�)
    public Vector3 offset; // ī�޶�� ��� ���� ������
    public float rotX,rotY; //���� ȸ����

    public float rotSpeed = 200;//ȸ�� �ӷ�
    //ī�޶� transform
    public Transform trCam;
    //public float rotationSpeed = 2.0f; // ī�޶� ȸ�� �ӵ�

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ����
        //Cursor.visible = false; // Ŀ�� ����
        offset = transform.position - target.transform.position;
    }

    private void Update()
    {
        //1. ���콺 �Է��� ����.
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        //2. ���콺�� ������ ���� ����
        rotX += mx * rotSpeed * Time.deltaTime;
        rotY += my * rotSpeed * Time.deltaTime;

        //3. �����Ȱ���ŭ ȸ��
        transform.localEulerAngles = new Vector3(0, rotX, 0);
        trCam.localEulerAngles = new Vector3(-rotY, 0, 0);
    }

    private void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}
