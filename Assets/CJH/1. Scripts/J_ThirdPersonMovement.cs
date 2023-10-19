using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_ThirdPersonMovement : MonoBehaviour
{
    public CharacterController cc; //ĳ������Ʈ�ѷ�
    public float speed = 5f; // �ӵ�
    public Animator anim; //�ִϸ�����
    public float gravity = -9.81f; // �߷�
    public int jumpHeight = 3;
    float yVelocity; //�ӷ�
    bool isJump;

    float h, v; //����,����

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();   
    }

    // Update is called once per frame
    void Update()
    {
        //�Է¿� ����
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        //���� �����
        //�¿�
        Vector3 dirH = transform.right * h;
        //�յ�
        Vector3 dirV = transform.forward * v;
        //����
        Vector3 dir = dirH + dirV;
        dir.Normalize();

        #region ����
        //������ �ƴ϶��
        if (cc.isGrounded)
        {
            yVelocity = 0;
            if (isJump)
            {
                //���� ��ġ�� �۽�
            }
            //���� �ƴ϶�� ����
            isJump = false;
        }
        //����
        if(Input.GetKeyDown(KeyCode.Space))
        {
            yVelocity = jumpHeight;
            //���� �۽�

            //���� ����

            //���� �ִϸ��̼�
        }
        #endregion
        //�ӷ��� �߷¸�ŭ ����
        yVelocity += gravity * Time.deltaTime;
        //�ӷ� = dir. y ��
        dir.y = yVelocity;
        //�̵�
        cc.Move(dir * speed * Time.deltaTime);
        //�ִϸ��̼� ����
        

        //�ִϸ��̼� parameter �� ���� (�۽�)
    }


}
