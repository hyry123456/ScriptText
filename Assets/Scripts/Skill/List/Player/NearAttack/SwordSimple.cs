using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using DefferedRender;

namespace Skill
{
    /// <summary>
    /// 一个简单的攻击技能，也就是播放2D的攻击动画
    /// </summary>
    public class SwordSimple : SkillBase
    {
        public SwordSimple()
        {
            maxSkillCount = -1;
            coolTime = 0.6f;
            skillCount = maxSkillCount;
            releaseTime = 0.4f;
            skillName = "测试攻击";
            skillType = SkillType.NearDisAttack;

        }
        float releaseTime2 = 0.15f;

        float preRelaseTime, nowTime;
        int index = 0;

        ParticleDrawData particle = new ParticleDrawData
        {
            //beginPos = Vector3.zero,
            beginSpeed = Vector3.down,
            speedMode = SpeedMode.PositionOutside,
            //cubeOffset = Vector3.one * 50,
            useGravity = false,
            followSpeed = true,
            radius = 0.3f,
            radian = 6.28f,
            lifeTime = 2,
            showTime = 2,
            frequency = 1,
            octave = 4,
            intensity = 1,
            sizeRange = new Vector2(0.2f, 0.5f),
            colorIndex = ColorIndexMode.HighlightToAlpha,
            textureIndex = 0,
            groupCount = 1
        };

        SkillManage manage;
        float attackDistance = 1.5f;

        Info.ActionData externalAction = new Info.ActionData
        {
            hp = -10,
            solide = 0,
        };

        public override bool OnSkillRelease(SkillManage mana)
        {
            manage = mana;

            if (Time.time - preRelaseTime < 1.5f && index == 0)
            {
                if (!mana.AnimateManage.SetAttack(2, releaseTime2))
                    return false;
                index++;
                //第二个技能，释放时间稍微长一点
                mana.SetEndTime(Time.time + releaseTime2 + 0.1f);
                mana.AnimateManage.animateEvent = (active) =>
                {
                    if(active)
                        CheckEnemyCanAttack();
                };
            }
            else
            {
                if (!mana.AnimateManage.SetAttack(1, releaseTime))
                    return false;
                index = 0;
                mana.SetEndTime(Time.time + releaseTime + 0.1f);
                mana.AnimateManage.animateEvent = (active) =>
                {
                    if (active)
                        CheckEnemyCanAttack();
                };
            }
            preRelaseTime = Time.time;

            return true;
        }

        //bool ReleaseBlade()
        //{
        //    currentPos += speed * direction * Time.deltaTime;
        //    nowTime += Time.deltaTime;
        //    if(sphere == null)  //退出
        //    {
        //        nowTime = maxWaitTime + 1;
        //        return true;
        //    }

        //    sphere.transform.position = currentPos;

        //    if(nowTime > maxWaitTime)
        //    {
        //        sphere.CloseObject();
        //        sphere = null;
        //        return true;
        //    }

        //    particle.beginPos = currentPos;
        //    particle.endPos = direction.x > 0 ? Vector3.up * 90 : -Vector3.up * 90;
        //    particle.beginSpeed = Vector3.down;
        //    ParticleNoiseFactory.Instance.DrawSphere(particle);

        //    return false;

        //}

        ////初始化球型碰撞体
        //void InitializeSphere()
        //{
        //    PoolingSphere origin = PoolingCollisionOrigin.PoolingSphereOrigin;
        //    sphere = SceneObjectPool.Instance.GetObject<PoolingSphere>
        //        (origin.name, origin.gameObject, manage.transform.position, Quaternion.identity);

        //    sphere.InitializeSphere(0.5f, (collider) =>
        //    {
        //        //不是要碰撞的物体，继续执行，退出该方法
        //        if (((1 << collider.gameObject.layer)
        //            & manage.CharacterInfo.attackLayer.value) == 0)
        //            return;

        //        AttackTarget(collider.GetComponent<Info.ExternalAction>());

        //        DrawExplodeAndClose(collider.ClosestPointOnBounds(manage.transform.position));
        //        return;
        //    });
        //}

        void DrawExplodeAndClose(Vector3 contract)
        {
            particle.beginPos = contract;
            particle.beginSpeed = manage.transform.right;
            particle.speedMode = SpeedMode.VerticalVelocityOutside;
            particle.groupCount = 10;
            ParticleNoiseFactory.Instance.DrawSphere(particle);

        }

        void CheckEnemyCanAttack()
        {
            List<StateMachine.StateMachineManage> enemys = StateMachine.StateMachineManage.Enemys;
            if (enemys == null) return;
            for(int i=0; i < enemys.Count; i++)
            {
                Vector3 dir = enemys[i].transform.position - manage.transform.position;
                if (Vector3.Dot(dir.normalized, manage.transform.right) > 0.1 &&
                    dir.magnitude < attackDistance)
                {
                    AttackTarget(enemys[i].GetComponent<Info.ExternalAction>());
                    return;
                }
            }

        }

        void AttackTarget(Info.ExternalAction external)
        {
            if (external != null)
            {
                externalAction.recall = (info, hp, solid) =>
                {
                    if (hp < 0)
                    {
                        DrawExplodeAndClose(info.transform.position);
                        Rigidbody rigidbody = external.gameObject.GetComponent<Rigidbody>();
                        rigidbody.AddForce((external.transform.position -
                            manage.transform.position).normalized * 300);
                    }
                };
                external.AddAction(externalAction);
            }
        }


    }
}