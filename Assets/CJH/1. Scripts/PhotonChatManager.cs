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
using UnityEngine.UIElements;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    public TMP_InputField inputChat;
    public GameObject chatItemFactory;
    public RectTransform trContent;
    public RectTransform rtScrollView;
    float prevContentH;//ä���� �߰��Ǳ� ���� Content�� H ���� ������ �ִ� ����


    //photon Chat Setting
    ChatAppSettings chatAppSettings;

    //ä�� �Ѱ��ϴ� ��ü 
    ChatClient chatClient;

    //�⺻ ä�� ä�� ���
    public List<string> channelNames = new List<string>();

    //���� ���õ� ä��
    int currChannelIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
        //�ؽ�Ʈ�� �ۼ��ϰ� ���͸� ���� �� ȣ��Ǵ� �Լ� ���(onSubmit.AddListener)
        inputChat.onSubmit.AddListener(OnSubmit);

        //PhotonChat �ʱ⼳��
        PhotonChatSetting();

        //���ӽõ�
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
        //���ο� ä���� �߰��Ǳ� ���� content�� H���� ����
        prevContentH = trContent.sizeDelta.y;

        //�ӼӸ����� �Ǵ�
        // /w ���̵� �޽��� = > �޽����� ���⸦ �������� string �迭�� �Ǵ�
        string[] s = text.Split(" ", 3);
        //�Ϲ� ä��
        if (s[0] == "/w")
        {
            //�ӼӸ��� ������
            if (s[1].Length > 0 && s[2].Length > 0)
            {
                chatClient.SendPrivateMessage(s[1], s[2]);

            }
        }
        else //�ӼӸ�
        {
            //ä���� ������
            chatClient.PublishMessage(channelNames[currChannelIdx], text);
        }

        //inputChat ���� �ʱ�ȭ
        inputChat.text = "";
        //inputChat ������ ���õǰ� ��
        inputChat.ActivateInputField();
        //
        StartCoroutine(AutoScrollBottom());


    }
    //�ڵ� ��ũ�� �ٿ�
    IEnumerator AutoScrollBottom()
    {
        yield return 0;
        //��ũ�Ѻ��� H���� content�� H���� ũ�ٸ�
        if(rtScrollView.sizeDelta.y < trContent.sizeDelta.y)
        {
            //������ �ٴڿ� ����־��ٸ�
            if(prevContentH - rtScrollView.sizeDelta.y <= trContent.anchoredPosition.y)
            {
                //content�� y�� �缳��
                trContent.anchoredPosition = new Vector2(0, trContent.sizeDelta.y - rtScrollView.sizeDelta.y);

            }
        }
    }



    //��Ÿ�� �� ȣ��� ����ê ����
    void PhotonChatSetting()
    {
        //���� ������ �����ͼ� ChatAppSettings�� ��������
        AppSettings photonSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

        //�� ������ ������ ChatAppSettings ����
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
    //PhotonChat�� �����ϴ� �Լ�
    void Connect()
    {
        chatClient = new ChatClient(this);
        //ä���� �� NickName ����
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues("�ֱ���"); //���Ŀ� ȸ�����Կ��� �г����� �������� �ҷ��;��� 

        //�ʱ⼳���� �̿��ؼ� ä�ü����� ���� �õ�
        chatClient.ConnectUsingSettings(chatAppSettings);
    }

    //ê ���� �Լ�
    void CreateChat(string sender, string message, Color color)
    {
        //chatItem ������( scrollView -> content�� �ڽ����� ���)
        GameObject go = Instantiate(chatItemFactory, trContent);
        //������ ���ӿ�����Ʈ���� ChatItem ������Ʈ �����´�
        PhotonChatItem item = go.GetComponent<PhotonChatItem>();
        //������ ������Ʈ���� SetText �Լ� ����
        item.SetText(sender + " : " + message, color);
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }
    //������ ������ ��
    public void OnDisconnected()
    {

    }
    //���� �������� ��
    public void OnConnected()
    {
        print("**** ä�� ���� ���� ����*****");
        //ä�� �߰�
        if (channelNames.Count > 0)
        {
            chatClient.Subscribe(channelNames.ToArray());
        }
        //���� ���¸� �¶������� �Ѵ�
        chatClient.SetOnlineStatus(ChatUserStatus.Online);


    }

    public void OnChatStateChange(ChatState state)
    {

    }
    //Ư��ä�ο� �޽����� �ö�
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            CreateChat(senders[i], messages[i].ToString(), Color.black);
        }


    }
    //�ӼӸ� �޼����� �ö�
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        CreateChat(sender, message.ToString(), Color.green);
    }
    //ä��ä���� �߰����� ��
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            Debug.Log("**** ä�� [" + channels[i] + "]�߰� ����");

        }

    }
    //ä��ä�� ����������
    public void OnUnsubscribed(string[] channels)
    {

    }
    //ģ�� ���°� �¶���, �������� ���·� �������� ��
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {

    }
    //ģ�� �߰� ���������� �̷������ ��
    public void OnUserSubscribed(string channel, string user)
    {

    }
    //ģ�� ������ �������� ��
    public void OnUserUnsubscribed(string channel, string user)
    {

    }
}
