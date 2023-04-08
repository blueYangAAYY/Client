using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    //是否正在移动
    protected bool isMoing = false;

    //是否正在攻击
    protected bool isAttack = false;

    //是否正在旋转
    protected bool isRotate = false;

    // 目标方向
    Vector3 targetDirection = Vector3.zero;

    // 当前方向
    Vector3 currentDirection = Vector3.zero;

    //攻击时间
    protected float attackTime = float.MinValue;

    //目标位置
    private Vector3 targetPosition;

    //移动速度
    private float speed = 4f;

    //动画组件
    private Animator animator;

    //描述
    public string desc = "";

    // 旋转速度
    float rotationSpeed = 3f;

    public float angleThreshold = 10f; // 夹角阈值

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


    //攻击
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
    /// 平滑旋转
    /// </summary>
    /// <param name="targetDirection">当前方向</param>
    /// <param name="currentDirection">目标方向</param>
    public virtual void RotateUpdate()
    {
        if (!isRotate)
        {
            return;
        }

        // 计算目标方向和当前方向之间的夹角
        float angle = Mathf.Acos(Vector3.Dot(targetDirection.normalized, currentDirection.normalized)) * Mathf.Rad2Deg;

        // 根据夹角和旋转速度计算需要旋转的角度
        float rotateAngle = Mathf.Min(angle, rotationSpeed * Time.deltaTime);

        // 计算中间方向
        Vector3 middleDirection = Vector3.RotateTowards(currentDirection, targetDirection, rotateAngle * Mathf.Deg2Rad, 0.0f);

        // 将中间方向转换为四元数
        Quaternion targetRotation = Quaternion.LookRotation(middleDirection);

        // 插值当前方向和目标方向之间的四元数
        Quaternion currentRotation = Quaternion.Lerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 将四元数转换为旋转矩阵或欧拉角，并将其应用于角色
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

        // 计算目标旋转方向
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        // 插值当前方向和目标方向之间的四元数
        Quaternion currentRotation = Quaternion.Lerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 将四元数转换为旋转矩阵或欧拉角，并将其应用于角色
        transform.rotation = currentRotation;

        Vector3 currentDirectionTmp = transform.forward;
        // 计算当前朝向向量和目标朝向向量之间的夹角余弦值
        float cosAngle = Vector3.Dot(currentDirectionTmp, targetDirection) / (currentDirectionTmp.magnitude * targetDirection.magnitude);

        // 如果夹角余弦值大于指定的阈值，认为已经朝向目标方向
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
