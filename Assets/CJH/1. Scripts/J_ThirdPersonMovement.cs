using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_ThirdPersonMovement : MonoBehaviour
{
    public CharacterController cc; //캐릭터컨트롤러
    public float speed = 5f; // 속도
    public Animator anim; //애니메이터
    public float gravity = -9.81f; // 중력
    public int jumpHeight = 3;
    float yVelocity; //속력
    bool isJump;

    float h, v; //가로,세로

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();   
    }

    // Update is called once per frame
    void Update()
    {
        //입력에 따라
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        //방향 만든다
        //좌우
        Vector3 dirH = transform.right * h;
        //앞뒤
        Vector3 dirV = transform.forward * v;
        //최종
        Vector3 dir = dirH + dirV;
        dir.Normalize();

        #region 점프
        //점프가 아니라면
        if (cc.isGrounded)
        {
            yVelocity = 0;
            if (isJump)
            {
                //점프 위치값 송신
            }
            //점프 아니라고 설정
            isJump = false;
        }
        //점프
        if(Input.GetKeyDown(KeyCode.Space))
        {
            yVelocity = jumpHeight;
            //점프 송신

            //점프 사운드

            //점프 애니메이션
        }
        #endregion
        //속력을 중력만큼 감소
        yVelocity += gravity * Time.deltaTime;
        //속력 = dir. y 값
        dir.y = yVelocity;
        //이동
        cc.Move(dir * speed * Time.deltaTime);
        //애니메이션 실행
        

        //애니메이션 parameter 값 전달 (송신)
    }


}
