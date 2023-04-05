using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public static class NetManager
{
    static Socket socket;

    //���ջ�����
    static byte[] readBuff = new byte[1024];

    //��Ϣί��
    public delegate void MsgListener(string str);

    //�����б�
    private static Dictionary<string, MsgListener> listeners =
        new Dictionary<string, MsgListener>();

    //��Ϣ�б�
    static List<string> msgList = new List<string>();

    //��Ӽ���
    public static void AddListener(string msgName,MsgListener listener)
    {
        listeners[msgName] = listener;

    }

    //��ȡ����
    public static string GetDesc()
    {
        if (socket == null || !socket.Connected)
        {
            return "";
        }

        //���ص�ǰsocket�󶨵�IP�Ͷ˿�
        return socket.LocalEndPoint.ToString();
    }

    //����
    public static void Connect(string ip,int port)
    {
        //init socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //connect
        socket.Connect(ip, port);

        //begin receive
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
    }

    //�첽������Ϣ�ص�
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);

            //���յ���Ϣ����������Ϣ����
            string str = Encoding.Default.GetString(readBuff, 0, count);
            msgList.Add(str);

            //��������
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch(SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    //������Ϣ
    public static void Send(string sendStr)
    {
        if (socket == null || !socket.Connected)
        {
            return;
        }

        byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    //����Ϣ������ȡ����Ϣ���д���
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

        //������Ϣͷ��������Ϣ����
        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgArgs);
        }
    }
}
