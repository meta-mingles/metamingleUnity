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
        //connection bgm ����
        //SoundManager.instance.PlayBGM(SoundManager.EBgm.BGM_CONNECTION);

        //inputNickName �� ������ ����� �� ȣ��Ǵ� �Լ� ���
        inputNickName.onValueChanged.AddListener(OnValueChanged);

        //inputNickName ���� ���� ���� �� ȣ��Ǵ� �Լ� ���
        inputNickName.onSubmit.AddListener(

            (string s) => {
                //��ư�� Ȱ��ȭ �Ǿ��ִٸ�
                if (btnConnect.interactable)
                {
                    //OnClickConnect ȣ��
                    OnClickConnect();
                }
            }

        );

        //��ư ��Ȱ��
        btnConnect.interactable = false;
    }

    void OnValueChanged(string s)
    {
        btnConnect.interactable = s.Length > 0;

        ////���࿡ s �� ���̰� 0���� ũ��
        //if(s.Length > 0)
        //{
        //    //���� ��ư�� Ȱ��ȭ
        //    btnConnect.interactable = true;
        //}
        ////�׷��� ������ (s �� ���̰� 0)
        //else
        //{
        //    //���� ��ư�� ��Ȱ��ȭ
        //    btnConnect.interactable = false;
        //}
    }

    public void OnClickConnect()
    {

        //SoundManager.instance.PlaySFX(SoundManager.ESfx.SFX_BUTTON);

        // ���� ���� ��û
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        //�г��� ����
        PhotonNetwork.NickName = inputNickName.text;

        //Ư�� Lobby ���� ����
        //TypedLobby typedLobby = new TypedLobby("Meta Lobby", LobbyType.Default);
        //PhotonNetwork.JoinLobby(typedLobby);

        //�⺻ �κ� ���� ��û
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        print(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //�κ� ������ �̵�
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