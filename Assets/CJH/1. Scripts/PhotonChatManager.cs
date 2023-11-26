using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PhotonChatManager : MonoBehaviourPun, IChatClientListener
{
    public TMP_InputField inputChat;
    public GameObject chatItemFactory;
    public RectTransform trContent;
    public RectTransform rtScrollView;
    float prevContentH;//채팅이 추가되기 전의 Content의 H 값을 가지고 있는 변수
    //photon Chat Setting
    ChatAppSettings chatAppSettings;
    //채팅 총괄하는 객체 
    ChatClient chatClient;
    //기본 채팅 채널 목록
    public List<string> channelNames = new List<string>();
    //현재 선택된 채널
    int currChannelIdx = 0;
    // Start is called before the first frame update
    void Start()
    {
        //텍스트를 작성하고 엔터를 쳤을 때 호출되는 함수 등록(onSubmit.AddListener)
        inputChat.onSubmit.AddListener(OnSubmit);
        //PhotonChat 초기설정
        PhotonChatSetting();
        //접속시도
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }
    void OnSubmit(string text)
    {
        //새로운 채팅이 추가되기 전의 content의 H값을 저장
        prevContentH = trContent.sizeDelta.y;
        //귓속말인지 판단
        // /w 아이디 메시지 = > 메시지에 띄어쓰기를 기준으로 string 배열로 판단
        string[] s = text.Split(" ", 3);
        //일반 채팅
        if (s[0] == "/w")
        {
            //귓속말을 보내자
            if (s[1].Length > 0 && s[2].Length > 0)
            {
                chatClient.SendPrivateMessage(s[1], s[2]);
            }
        }
        else //귓속말
        {
            //채팅을 보내자
            chatClient.PublishMessage(channelNames[currChannelIdx], text);
        }
        //inputChat 내용 초기화
        inputChat.text = "";
        //inputChat 강제로 선택되게 함
        inputChat.ActivateInputField();
        //Rpc함수로 모든사람한테 채팅 내용 전달
        photonView.RPC(nameof(AddChatRpc), RpcTarget.All, text);

    }
    [PunRPC]
    void AddChatRpc(string chat)
    {
        //채팅 내용 추가

    }

    //자동 스크롤 다운
    IEnumerator AutoScrollBottom()
    {
        yield return 0;
        //스크롤뷰의 H보다 content의 H값이 크다면
        if(rtScrollView.sizeDelta.y < trContent.sizeDelta.y)
        {
            //이전에 바닥에 닿아있었다면
            if(prevContentH - rtScrollView.sizeDelta.y <= trContent.anchoredPosition.y)
            {
                //content의 y값 재설정
                trContent.anchoredPosition = new Vector2(0, trContent.sizeDelta.y - rtScrollView.sizeDelta.y);
            }
        }
    }
    //스타트 때 호출된 포톤챗 설정
    void PhotonChatSetting()
    {
        //포톤 설정을 가져와서 ChatAppSettings에 설정하자
        AppSettings photonSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

        //위 설정을 가지고 ChatAppSettings 셋팅
        chatAppSettings = new ChatAppSettings();
        chatAppSettings.AppIdChat = photonSettings.AppIdChat;
        chatAppSettings.AppVersion = photonSettings.AppVersion;
        chatAppSettings.FixedRegion = photonSettings.FixedRegion;
        chatAppSettings.NetworkLogging = photonSettings.NetworkLogging;
        chatAppSettings.Protocol = photonSettings.Protocol;
        chatAppSettings.EnableProtocolFallback = photonSettings.EnableProtocolFallback;
        chatAppSettings.Server = photonSettings.Server;
        chatAppSettings.Port = (ushort)photonSettings.Port;
        chatAppSettings.ProxyServer = photonSettings.ProxyServer;
    }
    //PhotonChat에 접속하는 함수
    void Connect()
    {
        chatClient = new ChatClient(this);
        //채팅할 때 NickName 설정
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(HttpManager.instance.nickname); //추후에 회원가입에서 닉네임을 서버에서 불러와야함 
        //초기설정을 이용해서 채팅서버에 연결 시도
        chatClient.ConnectUsingSettings(chatAppSettings);
    }

    //챗 생성 함수
    void CreateChat(string sender, string message, Color color)
    {
        //chatItem 생성함( scrollView -> content의 자식으로 등록)
        GameObject go = Instantiate(chatItemFactory, trContent);
        //생성된 게임오브젝트에서 ChatItem 컴포넌트 가져온다
        PhotonChatItem item = go.GetComponent<PhotonChatItem>();
        //가져온 컴포넌트에서 SetText 함수 실행
        item.SetText(sender, message, color);
        //자동스크롤
        StartCoroutine(AutoScrollBottom());
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }
    //접속이 끊겼을 때
    public void OnDisconnected()
    {

    }
    //접속 성공했을 때
    public void OnConnected()
    {
        print("**** 채팅 서버 접속 성공*****");
        //채널 추가
        if (channelNames.Count > 0)
        {
            chatClient.Subscribe(channelNames.ToArray());
        }
        //나의 상태를 온라인으로 한다
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnChatStateChange(ChatState state)
    {

    }
    //특정채널에 메시지가 올때
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            CreateChat(senders[i], messages[i].ToString(), Color.black);
        }
    }
    //귓속말 메세지가 올때
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        CreateChat(sender, message.ToString(), Color.green);
    }
    //채팅채널을 추가했을 때
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            Debug.Log("**** 채널 [" + channels[i] + "]추가 성공");
        }

    }
    //채팅채널 삭제했을때
    public void OnUnsubscribed(string[] channels)
    {

    }
    //친구 상태가 온라인, 오프라인 상태로 변경했을 때
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {

    }
    //친구 추가 성공적으로 이루어졌을 때
    public void OnUserSubscribed(string channel, string user)
    {

    }
    //친구 삭제가 성공적일 때
    public void OnUserUnsubscribed(string channel, string user)
    {

    }
}
