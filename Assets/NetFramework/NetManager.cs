using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/*
    网络管理类：对外提供网络的主要接口
 */
namespace NetFramework
{
    public static class NetManager
    {
        //套接字
        static Socket socket;

        //接收缓冲区
        static ByteArray readBuff;

        //写入队列
        static Queue<ByteArray> writeQueue;

        //事件委托类型  error: 错误信息
        public delegate void EventListener(string error);

        //事件监听列表
        private static Dictionary<NetEvent, EventListener> eventListeners
            = new Dictionary<NetEvent, EventListener>();


        //是否处于正在连接服务器 状态
        static bool isConnecting = false;

        //是否处于正在关闭连接状态
        static bool isClosing = false;

        //添加事件监听
        public static void AddEventListener(NetEvent netEvent,EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] += listener;
            }
            else
            {
                eventListeners.Add(netEvent, listener);
            }
        }

        //删除事件监听
        public static void RemoveEventLisstener(NetEvent netEvent,EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] -= listener;
            }

            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }

        //触发事件
        private static void FireEvent(NetEvent netEvent,string err)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent](err);
            }
        }

        //连接
        public static void Connect(string ip,int port)
        {
            //当处于 已连接时
            if (socket != null && socket.Connected)
            {
                Debug.LogError("已连接服务器，请勿重复连接");
                return;
            }

            //当处于 正在连接时
            if (isConnecting)
            {
                Debug.LogError("正在连接服务器，请勿重复连接");
            }

            //初始化
            InitState();

            //不使用 NoDelay 算法
            socket.NoDelay = true;

            //连接服务器
            isConnecting = true;
            socket.BeginConnect(ip, port, ConnectCallback, socket);
        }

        //连接服务器回调
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);

                Debug.Log("连接服务器成功");

                //触发回调
                FireEvent(NetEvent.ConnectSucc, "");

                isConnecting = false;
            }
            catch(SocketException ex)
            {
                Debug.LogError("连接服务器失败:" + ex.ToString());

                //触发连接失败回调
                FireEvent(NetEvent.ConnextFail, ex.ToString());
                isConnecting = false;
            }
        }

        //发送数据
        public static void Send(MsgBase msg)
        {
            //状态判断
            if (socket == null || !socket.Connected)
            {
                return;
            }

            if (isConnecting)
            {
                return;
            }

            if (isClosing)
            {
                return;
            }

            //数据编码
            byte[] nameBytes = MsgBase.EncodeNmae(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);

            //获取消息长度
            int len = nameBytes.Length + bodyBytes.Length;

            //组装消息长度
            byte[] sendBytes = new byte[2 + len];
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);

            //组装名字
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);

            //组装消息体
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

            //写入队列
            ByteArray ba = new ByteArray(sendBytes);

            //writeQueue的长度
            int count = 0;

            //线程同步锁，避免引起线程问题
            lock(writeQueue)
            {
                //将消息添加进队列
                writeQueue.Enqueue(ba);

                //获取队列长度
                count = writeQueue.Count;
            }

            //发送消息，当队列中的长度为1时，可发送消息
            if (count == 1)
            {
                socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
            }

        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            //状态判断
            if (socket == null || !socket.Connected)
            {
                return;
            }

            //先结束结束
            int count = socket.EndSend(ar);

            //获取写入队列第一条数据
            ByteArray ba;
            lock(writeQueue)
            {
                ba = writeQueue.First();
            }

            //完整发送
            ba.readIndex += count;
            if (ba.Length == 0)
            {
                lock(writeQueue)
                {
                    writeQueue.Dequeue();
                    ba = writeQueue.First();
                }
            }

            //继续发送
            if (ba != null)
            {
                socket.BeginSend(ba.bytes, ba.readIndex, ba.Length, 0, SendCallback, socket);
            }
            //当正在关闭
            else if(isClosing)
            {
                socket.Close();
            }
        }

        //初始化
        private static void InitState()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            readBuff = new ByteArray();
            writeQueue = new Queue<ByteArray>();
            isConnecting = false;
            isClosing = false;
        }

        //关闭连接,只有在连接建立后才能关闭
        public static void Close()
        {
            if (socket == null || !socket.Connected)
            {
                return;
            }

            if (isConnecting)
            {
                return;
            }

            //还有数据在发送
            if (writeQueue.Count > 0)
            {
                isClosing = true;
            }
            //没有数据在发送
            else
            {
                socket.Close();

                //触发关闭事件
                FireEvent(NetEvent.Colse, "");
            }
        }
    }
}
