using UnityEngine;

[RequireComponent(typeof(CharacterController))] // CharacterController  ������Ʈ �ʿ�
public class J_CameraController : MonoBehaviour
{
    public float speed = 7.5f; // �̵� �ӵ�
    public float jumpSpeed = 8.0f; // ���� �ӵ�
    public float gravity = 20.0f; // �߷� ��
    public Transform playerCameraParent; // �÷��̾� ī�޶��� �θ� Ʈ������
    public float lookSpeed = 2.0f; // ī�޶� ȸ�� �ӵ�
    public float lookXLimit = 60.0f; // ī�޶� X�� ȸ�� ����

    CharacterController cc; // CharacterController ������Ʈ
    Vector3 moveDirection = Vector3.zero; // �̵� ����
    Vector2 rotation = Vector2.zero; // ȸ�� ��

    [HideInInspector]
    public bool canMove = true; // �̵� ���� ����

    void Start()
    {
        cc = GetComponent<CharacterController>(); // CharacterController ������Ʈ ��������
        rotation.y = transform.eulerAngles.y; // �ʱ� ȸ�� ���� ���� Y �� ȸ�� ������ ����
    }

    void Update()
    {
        if (cc.isGrounded)
        {
            // �ٴڿ� ���� ��, �࿡ ����� �̵� ���� �ٽ� ���
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            Vector3 right = transform.TransformDirection(Vector3.right);

            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;


            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpSpeed; // ���� �Է��� ���� ��, ���� �̵� �ӵ� ����
            }
        }

        // �߷� ����. �߷��� deltaTime�� �� �� ���� (���⼭ �� ��, �Ʒ����� �̵� ���⿡ deltaTime�� �ٽ� ����). �߷��� ���ӵ��� ����Ǿ�� �� (m/s^2)
        moveDirection.y -= gravity * Time.deltaTime;

        // ��Ʈ�ѷ� �̵�
        cc.Move(moveDirection * Time.deltaTime);

        // �÷��̾�� ī�޶� ȸ��
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed; // ���콺 X �� �Է��� �̿��� Y �� ȸ�� �� ���
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed; // ���콺 Y �� �Է��� �̿��� X �� ȸ�� �� ���
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit); // X �� ȸ�� ���� ���� ����
            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0); // ī�޶��� ���� ȸ�� ����
            transform.eulerAngles = new Vector2(0, rotation.y); // �÷��̾��� ȸ�� �� ����
        }
    }
}