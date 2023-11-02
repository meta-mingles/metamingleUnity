using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Diagnostics;

public class Animationcode : MonoBehaviour
{
    private Socket clientSocket;
    byte[] buffer = new byte[1024];

    [SerializeField] KHHMotionTracking khhMotionTracking;

    private void Start()
    {
        Process psi = new Process();
        psi.StartInfo.FileName = "C:/Users/user/AppData/Local/Programs/Python/Python310/python.exe";
        // 파이썬 환경 연결
        psi.StartInfo.Arguments = $"{Application.dataPath}/KHH/socket_pose_cvzone.py";
        // 실행할 파이썬 파일
        psi.StartInfo.CreateNoWindow = true;
        psi.StartInfo.UseShellExecute = false;
        psi.Start();
        UnityEngine.Debug.Log("실행완료");

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        IPEndPoint serverEndPoint = new IPEndPoint(serverIP, 12345);

        try
        {
            clientSocket.Connect(serverEndPoint);
            UnityEngine.Debug.Log("Connected to server");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Error connecting to server: {e}");
            return;
        }
    }

    void Update()
    {
        try
        {
            //유니티에 값보내기 
            string message = "Hello from Unity!";
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            clientSocket.Send(messageBytes);

            //파이썬에서 값 받기
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), buffer);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[알림] 에러발생: " + e.Message);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        int receivedBytes = clientSocket.EndReceive(ar);
        string receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, receivedBytes); //receivedMessage 문자열받기
        khhMotionTracking.MotionTracking(receivedMessage);
    }
    private void OnDisable()
    {
        // 게임 오브젝트가 비활성화될 때 실행되는 코드
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }
}