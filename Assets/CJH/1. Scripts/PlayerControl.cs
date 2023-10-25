using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Transform tr;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float turnSpeed = 4;

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        anim.Play("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        //�����¿� �̵����� ����
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //translate
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        //ȸ��
        tr.Translate(Vector3.up * turnSpeed * Time.deltaTime * r);

        //�ִϸ��̼�
        PlayerAnim(h, v);
        

    }
    void PlayerAnim(float h, float v)
    {
        if(v >= 0.1f)
        {
            anim.CrossFade("Walk", 0); //����
        }
        else if(v <= -0.1f)
        {
            anim.CrossFade("Walk", 0); //����
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade("Walk", 0); //������ 
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade("Walk", 0); //����
        }
        else
        {
            anim.CrossFade("Idle", 0f); //����
        }
    }
}
