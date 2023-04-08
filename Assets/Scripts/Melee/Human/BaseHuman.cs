using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    //�Ƿ������ƶ�
    protected bool isMoing = false;

    //�Ƿ����ڹ���
    protected bool isAttack = false;

    //�Ƿ�������ת
    protected bool isRotate = false;

    // Ŀ�귽��
    Vector3 targetDirection = Vector3.zero;

    // ��ǰ����
    Vector3 currentDirection = Vector3.zero;

    //����ʱ��
    protected float attackTime = float.MinValue;

    //Ŀ��λ��
    private Vector3 targetPosition;

    //�ƶ��ٶ�
    private float speed = 4f;

    //�������
    private Animator animator;

    //����
    public string desc = "";

    // ��ת�ٶ�
    float rotationSpeed = 3f;

    public float angleThreshold = 10f; // �н���ֵ

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
        //RotateUpdateY();
        AttackUpdate();
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


    //����
    public virtual void Attack()
    {
        isAttack = true;
        attackTime = Time.time;
        animator.SetBool("isAttack", true);
    }

    public virtual void AttackUpdate()
    {
        if (!isAttack)
        {
            return;
        }

        if (Time.time - attackTime < 1.2f)
        {
            return;
        }

        isAttack = false;
        animator.SetBool("isAttack", false);
    }

    /// <summary>
    /// ƽ����ת
    /// </summary>
    /// <param name="targetDirection">��ǰ����</param>
    /// <param name="currentDirection">Ŀ�귽��</param>
    public virtual void RotateUpdate()
    {
        if (!isRotate)
        {
            return;
        }

        // ����Ŀ�귽��͵�ǰ����֮��ļн�
        float angle = Mathf.Acos(Vector3.Dot(targetDirection.normalized, currentDirection.normalized)) * Mathf.Rad2Deg;

        // ���ݼнǺ���ת�ٶȼ�����Ҫ��ת�ĽǶ�
        float rotateAngle = Mathf.Min(angle, rotationSpeed * Time.deltaTime);

        // �����м䷽��
        Vector3 middleDirection = Vector3.RotateTowards(currentDirection, targetDirection, rotateAngle * Mathf.Deg2Rad, 0.0f);

        // ���м䷽��ת��Ϊ��Ԫ��
        Quaternion targetRotation = Quaternion.LookRotation(middleDirection);

        // ��ֵ��ǰ�����Ŀ�귽��֮�����Ԫ��
        Quaternion currentRotation = Quaternion.Lerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // ����Ԫ��ת��Ϊ��ת�����ŷ���ǣ�������Ӧ���ڽ�ɫ
        transform.rotation = currentRotation;

        if (currentDirection == targetDirection)
        {
            isRotate = false;
            Attack();
        }
    }

    public virtual void RotateUpdateY()
    {
        if (!isRotate)
        {
            return;
        }

        // ����Ŀ����ת����
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        // ��ֵ��ǰ�����Ŀ�귽��֮�����Ԫ��
        Quaternion currentRotation = Quaternion.Lerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // ����Ԫ��ת��Ϊ��ת�����ŷ���ǣ�������Ӧ���ڽ�ɫ
        transform.rotation = currentRotation;

        Vector3 currentDirectionTmp = transform.forward;
        // ���㵱ǰ����������Ŀ�곯������֮��ļн�����ֵ
        float cosAngle = Vector3.Dot(currentDirectionTmp, targetDirection) / (currentDirectionTmp.magnitude * targetDirection.magnitude);

        // ����н�����ֵ����ָ������ֵ����Ϊ�Ѿ�����Ŀ�귽��
        if (cosAngle > Mathf.Cos(angleThreshold * Mathf.Deg2Rad))
        {
            isRotate = false;
            Attack();
        }
    }

    public virtual void HumanRotate(Vector3 targetDirection, Vector3 currentDirection)
    {
        isRotate = true;
        this.targetDirection = targetDirection;
        this.currentDirection = currentDirection;
    }
}
