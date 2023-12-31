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
        //photon 환경설정을 기반으로 접속을 시도
        PhotonNetwork.ConnectUsingSettings(); 

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //마스터 서버 접속 완료
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print(nameof(OnConnectedToMaster));

        //로비 진입
        JoinLobby();
        //print(nameof(JoinLobby));
    }
    //로비 진입
    void JoinLobby()
    {
        //닉네임 설정
        PhotonNetwork.NickName = "조재희";
        //기본 로비 입장
        PhotonNetwork.JoinLobby();
    }
    //로비진입 완료
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print(nameof(OnJoinedLobby));

        //방을 생성 or 참여
        RoomOptions roomOption = new RoomOptions();
        PhotonNetwork.JoinOrCreateRoom("meta_mingle",roomOption,TypedLobby.Default);

    }
    //방 생성 완료시 호출되는 함수
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print(nameof(OnCreatedRoom));
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print(nameof(OnCreateRoomFailed));

        //방 생성 실패 원인 보여주는 팝업
    }

    //방 참여 성공시 호출되는 함수
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print(nameof(OnJoinedRoom));
        //게임씬으로 이동
        PhotonNetwork.LoadLevel("Proto_CJH");

    }
    //방 참여 실패시 호출되는 함수
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }

}
