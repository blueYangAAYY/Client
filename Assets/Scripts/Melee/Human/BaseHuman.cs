using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    //�Ƿ������ƶ�
    protected bool isMoing = false;

    //Ŀ��λ��
    private Vector3 targetPosition;

    //�ƶ��ٶ�
    private float speed = 4f;

    //�������
    private Animator animator;

    //����
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

        //��ȡ������������λ��
        Vector3 pos = transform.position;

        //�ƶ�
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);

        //�ı䳯��
        transform.LookAt(targetPosition);

        //����Ŀ��λ��
        if (Vector3.Distance(transform.position, targetPosition) <= 0.5f)
        {
            isMoing = false;
            animator.SetBool("isMoving", false);
        }
    }

    //�ƶ���ĳ��
    public virtual void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoing = true;
        animator.SetBool("isMoving", true);
    }
}
