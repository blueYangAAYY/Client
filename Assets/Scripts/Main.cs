using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject humanPref;
    private BaseHuman baseHuman;
    public Dictionary<string, BaseHuman> otherHuman = new Dictionary<string, BaseHuman>();

    // Start is called before the first frame update
    void Start()
    {
        NetManager.Connect("127.0.0.1", 8888);

        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);

        //ʵ������ɫ
        GameObject obj = GameObject.Instantiate(humanPref, this.transform);
        baseHuman = obj.AddComponent<CtrlHuman>();
        baseHuman.transform.position = new Vector3(Random.Range(0f, 5f), 0, Random.Range(0f, 5f));

        //��������б�
        NetManager.Send("List|");

        //��ȡ����
        baseHuman.desc = NetManager.GetDesc();

        Vector3 pos = baseHuman.transform.position;
        Vector3 ang = baseHuman.transform.eulerAngles;

        //���� Enter Э��
        string sendStr = "Enter|";

        sendStr += NetManager.GetDesc() + ",";
        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += ang.y;

        NetManager.Send(sendStr);
    }

    private void Update()
    {
        NetManager.Update();
    }

    void OnEnter(string msg)
    {
        Debug.Log("OnEnter" + msg);

        string[] reStr = msg.Split(',');

        string desc = reStr[0];

        float x = float.Parse(reStr[1]);
        float y = float.Parse(reStr[2]);
        float z = float.Parse(reStr[3]);
        float eulY = float.Parse(reStr[4]);

        //���Լ�
        if (desc == baseHuman.desc)
        {
            return;
        }

        //��ӽ�ɫ
        GameObject obj = GameObject.Instantiate(humanPref, this.transform);
        BaseHuman syncHuman = obj.AddComponent<SyncHuman>();
        syncHuman.transform.position = new Vector3(x, y, z);
        syncHuman.transform.eulerAngles = new Vector3(0, eulY, 0);

        //��ȡ����
        syncHuman.desc = desc;
        otherHuman.Add(desc, syncHuman);
    }

    private void OnList(string msg)
    {
        Debug.Log("OnList" + msg);

        string[] split = msg.Split(',');

        //���ݽ��յ���Ϣ ���� ��ɫ����
        int count = (split.Length - 1) / 6;

        for (int i = 0; i < count; i++)
        {
            string desc = split[i * 6 + 0];

            //���Լ�
            if (desc == NetManager.GetDesc())
                continue;

            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);

            //���һ����ɫ
            GameObject obj = (GameObject)Instantiate(humanPref);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, eulY, 0);
            BaseHuman h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHuman.Add(desc, h);
        }
    }


    void OnMove(string msg)
    {
        Debug.Log("OnMove" + msg);
    }

    void OnLeave(string msg)
    {
        Debug.Log("OnLeave" + msg);
    }
}
