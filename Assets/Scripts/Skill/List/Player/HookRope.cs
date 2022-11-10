using UnityEngine;


namespace Skill
{
    /// <summary>  /// �������ܣ������������ƶ������� /// </summary>
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