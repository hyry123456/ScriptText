using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    /// <summary> /// 测试用的闪避技能 /// </summary>
    public class SimpleDoage : SkillBase, Info.CharacterState
    {
        public SimpleDoage()
        {
            maxSkillCount = -1;
            skillCount = -1;
            releaseTime = 0.5f;     //移动时间
            coolTime = 3;
            skillName = "闪避";
            skillType = SkillType.Dodge;
        }

        float rollBeginTime, moveSpeed = 10;
        Vector3 moveDir, currentPos;
        SkillManage manage;

        public override bool OnSkillRelease(SkillManage mana)
        {
            if (!mana.AnimateManage.SetAttack(3, releaseTime))
                return false;
            if(Time.time - rollBeginTime < releaseTime)
                return false;

            rollBeginTime = Time.time;
            currentPos = mana.transform.position;
            manage = mana;
            moveDir = mana.transform.right; moveDir.y = 0;
            mana.GetComponent<Collider>().isTrigger = true;
            mana.GetComponent<Rigidbody>().velocity = Vector3.zero;
            mana.GetComponent<Info.ExternalAction>().AddAction(new Info.ActionData
            {
                state = this
            });
            Common.SustainCoroutine.Instance.AddCoroutine(Rolling, false);
            return true;
        }
        
        bool Rolling()
        {
            float radio = (Time.time - rollBeginTime) / releaseTime;
            if(radio > 1)
            {
                manage.GetComponent<Collider>().isTrigger = false;
                manage.GetComponent<Rigidbody>().velocity = moveDir * moveSpeed;
                return true;
            }
            currentPos += moveDir * moveSpeed * Time.deltaTime;
            manage.transform.position = currentPos;
            return false;
        }

        public bool OnUpdate(Info.CharacterInfo info, int finalHp, int finalSolid)
        {
            if(Time.time - rollBeginTime > releaseTime)
                return true;
            return false;
        }

        public int OnHPChange(int hp, Info.CharacterInfo info)
        {
            return Mathf.Max(hp, 0);
        }

        public int OnSolidChange(int solid, Info.CharacterInfo info)
        {
            return Mathf.Max(solid, 0);
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