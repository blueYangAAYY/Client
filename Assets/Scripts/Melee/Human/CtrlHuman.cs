using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlHuman : BaseHuman
{
    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            if (hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);

                //发送移动协议
                string sendStr = "Move|";

                sendStr += NetManager.GetDesc() + ",";
                sendStr += hit.point.x + ",";
                sendStr += hit.point.y + ",";
                sendStr += hit.point.z + ",";

                NetManager.Send(sendStr);
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            if (isAttack) return;
            if (isMoing) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            HumanRotate((hit.point - this.transform.position).normalized,this.transform.eulerAngles.normalized);
        }


    }
}
