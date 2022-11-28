using UnityEngine;


namespace Skill
{
    /// <summary>  /// 钩锁技能，将钩锁技能移动到这里 /// </summary>
    public class HookRope : SkillBase
    {
        Motor.RigibodyMotor motor;
        public HookRope()
        {
            maxSkillCount = -1;
            skillCount = maxSkillCount;
            preReleaseTime = 0;
            coolTime = 0;
            skillName = "Hook Rope";
            skillType = SkillType.LongDisAttack;
        }


        public override bool OnSkillRelease(SkillManage mana)
        {
            if (motor == null)
                motor = mana.GetComponent<Motor.RigibodyMotor>();
            motor.TransferToPosition(HookRopeManage.Instance.Target, 1);
            return true;
        }
    }
}