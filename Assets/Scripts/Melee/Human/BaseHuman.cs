using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    //是否正在移动
    protected bool isMoing = false;

    //目标位置
    private Vector3 targetPosition;

    //移动速度
    private float speed = 4f;

    //动画组件
    private Animator animator;

    //描述
    public string desc = "";

    protected virtual void Start()
    {
        if (animator == null)
        {
            animator = transform.Find("RPG-Character").GetComponent<Animator>();
        }
    }

    protected virtual void Update()
    {
        MoveUpdate();
    }

    public virtual void MoveUpdate()
    {
        if (!isMoing)
        {
            return;
        }

        //获取自身世界坐标位置
        Vector3 pos = transform.position;

        //移动
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);

        //改变朝向
        transform.LookAt(targetPosition);

        //到达目标位置
        if (Vector3.Distance(transform.position, targetPosition) <= 0.5f)
        {
            isMoing = false;
            animator.SetBool("isMoving", false);
        }
    }

    //移动到某处
    public virtual void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoing = true;
        animator.SetBool("isMoving", true);
    }
}
