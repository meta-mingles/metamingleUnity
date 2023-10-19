using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_TPSController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 2.0f;
    public GameObject playerCharacter; // �÷��̾� ĳ����

    private Transform playerTransform;
    private CinemachineFreeLook freeLookCam;

    private void Start()
    {
        playerTransform = playerCharacter.transform;
        freeLookCam = GetComponent<CinemachineFreeLook>();
    }

    private void Update()
    {
        // �̵� ����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        moveDirection = transform.TransformDirection(moveDirection);
        playerTransform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // ȸ�� ����
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        playerTransform.Rotate(Vector3.up * mouseX * rotationSpeed);

        // Cinemachine ī�޶� ȸ�� ����
        freeLookCam.m_XAxis.m_InputAxisName = "Mouse X"; // X �� (���� ȸ��)
        freeLookCam.m_YAxis.m_InputAxisName = "Mouse Y"; // Y �� (���� ȸ��)
    }
}
