using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class Echo : MonoBehaviour
{
    public Button connectBtn;
    public Button sendBtn;
    public Text text;
    public InputField input;

    private Socket socket;

    //接收缓冲区
    byte[] readBuff = new byte[1024];
    String recvStr = "";

    private void Start()
    {
        connectBtn.onClick.AddListener(Connection);
        sendBtn.onClick.AddListener(Send);
    }

    private void Connection()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        socket.BeginConnect("127.0.0.1",8888,ConnectCallback,socket);
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);

            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }catch(SocketException ex)
        {
            Debug.Log("Socket Connect fail" + ex.ToString());
        }

    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket sockt = (Socket)ar.AsyncState;
            int count = sockt.EndReceive(ar);
            recvStr = Encoding.Default.GetString(readBuff, 0, count);

            //继续接收
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch(SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    private void Send()
    {
        string sendStr = input.text;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.BeginSend(sendBytes,0, sendBytes.Length, 0, SendCallback, socket);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket sockt = (Socket)ar.AsyncState;
            int count = sockt.EndSend(ar);

            Debug.Log("Socket Send succ" + count);
        }
        catch(SocketException ex)
        {
            Debug.Log("Socket Send fail" + ex.ToString());
        }
    }

    private void Update()
    {
        text.text = recvStr;
    }
}
