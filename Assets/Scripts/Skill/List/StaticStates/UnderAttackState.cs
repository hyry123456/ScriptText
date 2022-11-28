using UnityEngine;

namespace Skill
{
    /// <summary>
    /// 受到攻击的静态状态技能，为敌人受到攻击时进行技能暂停以及播放动画
    /// </summary>
    public class UnderAttackState : SkillBase, Info.CharacterState
    {
        /// <summary> /// 保存技能释放对象    /// </summary>
        SkillManage mana;
        AnimateManage animateManage;
        Motor.MotorBase motor;

        public UnderAttackState()
        {
            skillType = SkillType.StaticState;
            nowTime = waitTime + upTime + 1;        //初始化时间
            releaseTime = 1.5f;
        }

        public override bool OnSkillRelease(SkillManage mana)
        {
            this.mana = mana;
            Info.ExternalAction external = mana.GetComponent<Info.ExternalAction>();
            external.AddAction(new Info.ActionData
            {
                state = this
            });
            animateManage = mana.gameObject.GetComponent<AnimateManage>();
            motor = mana.gameObject.GetComponent<Motor.MotorBase>();
            return true;
        }

        //不改变hp
        public int OnHPChange(int hp, Info.CharacterInfo info)
        {
            return hp;
        }

        public int OnSolidChange(int solid, Info.CharacterInfo info)
        {
            this.info = info;
            return solid;
        }

        public bool OnUpdate(Info.CharacterInfo info, int finalHp, int finalSolid)
        {
            if (animateManage == null)
                return true;

            //检查是否被破防
            if (info.SolidRadio < 0)
            {
                //在等待阶段，直接打断动画
                if (nowTime < waitTime)
                {
                    animateManage.ExitAnimate(1, releaseTime);
                    mana.SetEndTime(Time.time + releaseTime);
                    motor.Move(0, 0);
                }
                //上升阶段是全刚体，不被打断
                else if (nowTime < waitTime + upTime)
                {
                    return false;
                }
                //第一次被打断，加入协程
                else
                {
                    animateManage.ExitAnimate(1, releaseTime);
                    mana.SetEndTime(Time.time + releaseTime);
                    nowTime = 0;
                    motor.Move(0, 0);
                    Common.SustainCoroutine.Instance.AddCoroutine(BeBreakWait);
                }
            }
            else
            {
                if (finalHp < 0)
                {
                    animateManage.ExitAnimate(1, 0.1f);
                    mana.SetEndTime(Time.time + 0.1f);
                    motor.Move(0, 0);
                }
            }
            return false;
        }

        float nowTime, waitTime = 5, upTime = 3;
        Info.CharacterInfo info;

        /// <summary>
        /// 当坚挺值被打破时执行的方法，等待加载时间
        /// </summary>
        bool BeBreakWait()
        {
            nowTime += Time.deltaTime;
            if(nowTime >= waitTime)
            {
                Common.SustainCoroutine.Instance.AddCoroutine(AddSolid);
                return true;
            }
            return false;
        }

        /// <summary>     /// 护盾恢复    /// </summary>
        bool AddSolid()
        {
            nowTime += Time.deltaTime;
            if(nowTime >= waitTime + upTime)
            {
                info.SetSolidRadio(info.MaxSolidRadio);
                return true;
            }
            info.SetSolidRadio((int)Mathf.Lerp(0,
                info.MaxSolidRadio, (nowTime - waitTime) / upTime));
            return false;
        }

        public float GetRunSpeed(Info.CharacterInfo info, float runSpeed)
        {            
            return runSpeed;
        }

        public float GetWalkSpeed(Info.CharacterInfo info, float walkSpeed)
        {
            return walkSpeed;
        }

        public int GetAttack(Info.CharacterInfo info, int attack)
        {
            return attack;
        }

        public int GetDefense(Info.CharacterInfo info, int defense)
        {
            return defense;
        }
    }
}