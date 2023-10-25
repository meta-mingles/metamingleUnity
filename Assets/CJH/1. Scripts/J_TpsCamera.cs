using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Camera))]
public class J_TpsCamera : MonoBehaviourPun
{
    public float moveSpeed;               // �̵� �ӵ�
    public float shiftAdditionalSpeed;    // Shift Ű �߰� �ӵ�
    public float mouseSensitivity;        // ���콺 ����
    public bool invertMouse;              // ���콺 ���� ����
    public bool autoLockCursor;           // �ڵ� Ŀ�� �� ����

    public Transform target; //�÷��̾�
    public Vector3 offset;

    private Camera cam;  // ī�޶� ������Ʈ�� ���� ����

   
    void Awake()
    {
        //target = playerControls.GetComponent<J_PlayerControls>().transform;
        cam = this.gameObject.GetComponent<Camera>();  // ī�޶� ������Ʈ�� ���� ���� ȹ��
        this.gameObject.name = "SpectatorCamera";      // ���� ������Ʈ �̸� ����
        //Cursor.lockState = (autoLockCursor) ? CursorLockMode.Locked : CursorLockMode.None;  // Ŀ�� �� ����
    }
    public void Start()
    {
        offset = transform.position - target.transform.position;
    }

    void Update()
    {
        ////������ �ƴҶ� �Լ��� ������
        //if (photonView.IsMine == false) { return; }


        float speed = (moveSpeed + (Input.GetAxis("Fire3") * shiftAdditionalSpeed));  // �̵� �ӵ� ���

        // Ű �Է¿� ���� ī�޶� �̵�
        this.gameObject.transform.Translate(Vector3.forward * speed * Input.GetAxis("Vertical"));
        this.gameObject.transform.Translate(Vector3.right * speed * Input.GetAxis("Horizontal"));
        this.gameObject.transform.Translate(Vector3.up * speed * (Input.GetAxis("Jump") + (Input.GetAxis("Fire1") * -1)));


        // ���콺 �Է¿� ���� ī�޶� ȸ��
        this.gameObject.transform.Rotate(Input.GetAxis("Mouse Y") * mouseSensitivity * ((invertMouse) ? 1 : -1), Input.GetAxis("Mouse X") * mouseSensitivity * ((invertMouse) ? -1 : 1), 0);

        // ī�޶� ȸ�� ���� ����
        this.gameObject.transform.localEulerAngles = new Vector3(this.gameObject.transform.localEulerAngles.x, this.gameObject.transform.localEulerAngles.y, 0);




        #region Ŀ�� ��
        //// Ŀ�� �� ���� ����
        //if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        //{
        //    Cursor.lockState = CursorLockMode.Locked;  // Ŀ�� �� ����
        //}
        //else if (Cursor.lockState == CursorLockMode.Locked && Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Cursor.lockState = CursorLockMode.None;  // Ŀ�� �� ����
        //}
        #endregion

    }

    private void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }

}
