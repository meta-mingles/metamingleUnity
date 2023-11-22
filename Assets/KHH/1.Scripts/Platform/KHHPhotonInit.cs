using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KHHPhotonInit : MonoBehaviourPunCallbacks
{
    public static KHHPhotonInit instance;

    List<RoomInfo> roomInfos = new List<RoomInfo>();

    string prevSceneName, nextSceneName, nickName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void Init(string prev, string next, string nick)
    {
        PhotonNetwork.ConnectUsingSettings();
        prevSceneName = prev;
        nextSceneName = next;
        nickName = nick;
    }

    //마스터 서버 접속 완료
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        //닉네임 설정
        PhotonNetwork.NickName = nickName;
        //기본 Lobby 입장
        PhotonNetwork.JoinLobby();
    }

    //로비진입 완료
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //랜덤방 입장 시도
        PhotonNetwork.JoinRandomRoom();

        ////방 참여 시도
        //for (int i = 0; i < roomInfos.Count; i++)
        //{
        //    if (roomInfos[i].Name == $"Platform{i}")
        //    {
        //        if (roomInfos[i].PlayerCount < roomInfos[i].MaxPlayers)
        //        {
        //            PhotonNetwork.JoinRoom($"Platform{i}");
        //            return;
        //        }
        //    }
        //}

        //string roomName = $"Platform{roomInfos.Count}";
        //RoomOptions roomOptioin = new RoomOptions();
        //roomOptioin.MaxPlayers = 20;
        //PhotonNetwork.CreateRoom(roomName, roomOptioin, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        //씬이동
        GlobalValue.PrevSceneName = prevSceneName;
        GlobalValue.CurSceneName = nextSceneName;
        if (nextSceneName.Equals("Customization")) SceneManager.LoadScene(nextSceneName);
        else PhotonNetwork.LoadLevel(nextSceneName);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        //방 생성
        string roomName = $"Platform{roomInfos.Count}";
        RoomOptions roomOptioin = new RoomOptions();
        roomOptioin.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptioin, TypedLobby.Default);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        roomInfos = roomList;
    }
}
