using UnityEngine;


public class AutoAnimate2D : AnimateManage
{
    Vector3 prePosition;     //上一帧的位置

    Motor.MotorBase motor;

    protected override void Start()
    {
        base.Start();
        motor = GetComponent<Motor.MotorBase>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        SavePrePosition();
    }

    protected override void AutoAnimate()
    {
        Vector3 dir = transform.position - prePosition;
        if (Mathf.Abs(dir.x) > magnifySize || Mathf.Abs(dir.z) > magnifySize)
            Animator.SetBool("Run", true);
        else Animator.SetBool("Run", false);

        //不在地上，高度位移
        if (!motor.OnGround())
        {
            Animator.SetInteger("JumpMode", dir.y < 0 ? 2 : 1);
        }
        else Animator.SetInteger("JumpMode", 0);
    }

    private void SavePrePosition()
    {
        prePosition = transform.position;
    }
}
