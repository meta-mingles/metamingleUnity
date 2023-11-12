using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    public J_TpsCamera tpsCamera;

    public 
    // Start is called before the first frame update
    void Start()
    {
        //OnPhotonSerializeView 호출 빈도
        PhotonNetwork.SerializationRate = 10;

        //나의 플레이어 생성
        GameObject myPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

        //마우스 포인터 비활성화
        //Cursor.visible = false;

        tpsCamera.target = myPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {

        //만약에 esc키를 누르면
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //마우스 포인터 활성화 
            Cursor.visible = true;
        }
        //마우스 클릭했을 때
        if (Input.GetMouseButton(0))
        {
            //마우스 클릭시 해당위치에 UI가 없으면 
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                //마우스 포인터 비활성화
                Cursor.visible = false;

            }
        }
        //UI 클릭했을 때 마우스 포인터는 사라지지 않게해야함
        


    }
}
