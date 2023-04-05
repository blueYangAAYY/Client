using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public static class NetManager
{
    static Socket socket;

    //接收缓冲区
    static byte[] readBuff = new byte[1024];

    //消息委托
    public delegate void MsgListener(string str);

    //监听列表
    private static Dictionary<string, MsgListener> listeners =
        new Dictionary<string, MsgListener>();

    //消息列表
    static List<string> msgList = new List<string>();

    //添加监听
    public static void AddListener(string msgName,MsgListener listener)
    {
        listeners[msgName] = listener;

    }

    //获取描述
    public static string GetDesc()
    {
        if (socket == null || !socket.Connected)
        {
            return "";
        }

        //返回当前socket绑定的IP和端口
        return socket.LocalEndPoint.ToString();
    }

    //连接
    public static void Connect(string ip,int port)
    {
        //init socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //connect
        socket.Connect(ip, port);

        //begin receive
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
    }

    //异步接收消息回调
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);

            //接收到消息，并存入消息队列
            string str = Encoding.Default.GetString(readBuff, 0, count);
            msgList.Add(str);

            //继续接收
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch(SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    //发送消息
    public static void Send(string sendStr)
    {
        if (socket == null || !socket.Connected)
        {
            return;
        }

        byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    //从消息队列中取出消息进行处理
    public static void Update()
    {
        if (msgList.Count <= 0)
        {
            return;
        }

        string msgStr = msgList[0];
        msgList.RemoveAt(0);

        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];

        //根据消息头，进行消息处理
        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgArgs);
        }
    }
}
