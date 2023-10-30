using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    //NickName InputField
    public InputField inputNickName;

    //Connect Button
    public Button btnConnect;

    void Start()
    {
        //connection bgm 실행
        //SoundManager.instance.PlayBGM(SoundManager.EBgm.BGM_CONNECTION);

        //inputNickName 의 내용이 변경될 때 호출되는 함수 등록
        inputNickName.onValueChanged.AddListener(OnValueChanged);

        //inputNickName 에서 엔터 쳤을 때 호출되는 함수 등록
        inputNickName.onSubmit.AddListener(

            (string s) => {
                //버튼이 활성화 되어있다면
                if (btnConnect.interactable)
                {
                    //OnClickConnect 호출
                    OnClickConnect();
                }
            }

        );

        //버튼 비활성
        btnConnect.interactable = false;
    }

    void OnValueChanged(string s)
    {
        btnConnect.interactable = s.Length > 0;

        ////만약에 s 의 길이가 0보다 크면
        //if(s.Length > 0)
        //{
        //    //접속 버튼을 활성화
        //    btnConnect.interactable = true;
        //}
        ////그렇지 않으면 (s 의 길이가 0)
        //else
        //{
        //    //접속 버튼을 비활성화
        //    btnConnect.interactable = false;
        //}
    }

    public void OnClickConnect()
    {

        //SoundManager.instance.PlaySFX(SoundManager.ESfx.SFX_BUTTON);

        // 서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        //닉네임 설정
        PhotonNetwork.NickName = inputNickName.text;

        //특정 Lobby 정보 셋팅
        //TypedLobby typedLobby = new TypedLobby("Meta Lobby", LobbyType.Default);
        //PhotonNetwork.JoinLobby(typedLobby);

        //기본 로비 진입 요청
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        print(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //로비 씬으로 이동
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    SoundManager.instance.PlaySFX(SoundManager.ESfx.SFX_BUTTON);
        //}
    }
}