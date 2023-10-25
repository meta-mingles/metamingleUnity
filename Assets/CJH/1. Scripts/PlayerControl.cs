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

        //전후좌우 이동방향 벡터
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //translate
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        //회전
        tr.Translate(Vector3.up * turnSpeed * Time.deltaTime * r);

        //애니메이션
        PlayerAnim(h, v);
        

    }
    void PlayerAnim(float h, float v)
    {
        if(v >= 0.1f)
        {
            anim.CrossFade("Walk", 0); //전진
        }
        else if(v <= -0.1f)
        {
            anim.CrossFade("Walk", 0); //후진
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade("Walk", 0); //오른쪽 
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade("Walk", 0); //왼쪽
        }
        else
        {
            anim.CrossFade("Idle", 0f); //정지
        }
    }
}
