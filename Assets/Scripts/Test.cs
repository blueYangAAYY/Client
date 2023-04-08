using NetFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void OnConnect()
    {
        NetFramework.NetManager.Connect("127.0.0.1", 8888);
    }

    public void OnMove()
    {
        MsgMove msgMove = new MsgMove();
        msgMove.x = 1;
        msgMove.y = 1;
        msgMove.z = 1;

        NetFramework.NetManager.Send(msgMove);
    }
}
