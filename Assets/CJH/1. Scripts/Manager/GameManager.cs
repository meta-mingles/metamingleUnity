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
        //OnPhotonSerializeView ȣ�� ��
        PhotonNetwork.SerializationRate = 10;

        //���� �÷��̾� ����
        GameObject myPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

        //���콺 ������ ��Ȱ��ȭ
        Cursor.visible = false;

        tpsCamera.target = myPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {

        //���࿡ escŰ�� ������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //���콺 ������ Ȱ��ȭ 
            Cursor.visible = true;
        }
        //���콺 Ŭ������ ��
        if (Input.GetMouseButton(0))
        {
            //���콺 Ŭ���� �ش���ġ�� UI�� ������ 
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                //���콺 ������ ��Ȱ��ȭ
                Cursor.visible = false;

            }
        }
        //UI Ŭ������ �� ���콺 �����ʹ� ������� �ʰ��ؾ���
        


    }
}
