using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class SimpleConnectionMgr : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        //photon ȯ�漳���� ������� ������ �õ�
        PhotonNetwork.ConnectUsingSettings(); 

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //������ ���� ���� �Ϸ�
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print(nameof(OnConnectedToMaster));

        //�κ� ����
        JoinLobby();
        //print(nameof(JoinLobby));
    }
    //�κ� ����
    void JoinLobby()
    {
        //�г��� ����
        PhotonNetwork.NickName = "������";
        //�⺻ �κ� ����
        PhotonNetwork.JoinLobby();
    }
    //�κ����� �Ϸ�
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print(nameof(OnJoinedLobby));

        //���� ���� or ����
        RoomOptions roomOption = new RoomOptions();
        PhotonNetwork.JoinOrCreateRoom("meta_mingle",roomOption,TypedLobby.Default);

    }
    //�� ���� �Ϸ�� ȣ��Ǵ� �Լ�
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print(nameof(OnCreatedRoom));
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print(nameof(OnCreateRoomFailed));

        //�� ���� ���� ���� �����ִ� �˾�
    }

    //�� ���� ������ ȣ��Ǵ� �Լ�
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print(nameof(OnJoinedRoom));
        //���Ӿ����� �̵�
        PhotonNetwork.LoadLevel("Proto_CJH");

    }
    //�� ���� ���н� ȣ��Ǵ� �Լ�
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }

}
