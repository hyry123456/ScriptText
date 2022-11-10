using UnityEngine;
using Info;
using DefferedRender;

namespace Skill
{

    /// <summary>/// 主角靠近敌人时受伤/// </summary>
    public class PlayerNearInjured : SkillBase, CharacterState
    {
        /// <summary> /// 保存技能释放对象    /// </summary>
        SkillManage mana;
        AnimateManage animateManage;
        Motor.MotorBase motor;

        float preAttackTime, nowTime;
        //是否正在触发
        bool isNear;
        Collider nearTarget;
        Info.ExternalAction externalAction;

        ParticleDrawData particle = new ParticleDrawData
        {
            //beginPos = Vector3.zero,
            beginSpeed = Vector3.down * 2,
            speedMode = SpeedMode.PositionOutside,
            useGravity = false,
            followSpeed = true,
            radius = 0.1f,
            radian = 6.28f,
            lifeTime = 2,
            showTime = 2,
            frequency = 1,
            octave = 4,
            intensity = 1,
            sizeRange = new Vector2(0.2f, 0.5f),
            colorIndex = ColorIndexMode.HighlightToAlpha,
            textureIndex = 0,
            groupCount = 10
        };

        Info.ActionData nearAction = new ActionData()
        {
            hp = -10,
        };

        private void AddColliderDelegate()
        {
            Interaction.ColliderInteracteDelegate interacte 
                = mana.gameObject.AddComponent<Interaction.ColliderInteracteDelegate>();
            interacte.IsSustain = true;
            interacte.TrigerTags = "Enemy";
            interacte.TrigerEnter = (other) =>
            {
                isNear = true;
                nearTarget = other;
                externalAction.AddAction(nearAction);
            };
            interacte.TrigerExit = (other) =>
            {
                isNear = false;
                nearTarget = null;
            };
            interacte.CollisionEnter = (collsion) =>
            {
                isNear = true;
                nearTarget = collsion.collider;
                externalAction.AddAction(nearAction);
            };
            interacte.CollisionExit = (collsion) =>
            {
                isNear = false;
                nearTarget = null;
            };
        }

        public PlayerNearInjured()
        {
            skillType = SkillType.StaticState;
            releaseTime = 1.5f;
        }

        public override bool OnSkillRelease(SkillManage mana)
        {
            this.mana = mana;
            externalAction = mana.GetComponent<Info.ExternalAction>();
            externalAction.AddAction(new Info.ActionData
            {
                state = this
            });
            animateManage = mana.gameObject.GetComponent<AnimateManage>();
            motor = mana.gameObject.GetComponent<Motor.MotorBase>();
            AddColliderDelegate();
            return true;
        }

        public int OnSolidChange(int solid, Info.CharacterInfo info)
        {
            return solid;
        }

        public int OnHPChange(int hp, Info.CharacterInfo info)
        {
            return hp;
        }

        public bool OnUpdate(Info.CharacterInfo info, int finalHp, int finalSolid)
        {
            if(isNear && Time.time - preAttackTime > 1 && finalHp < 0)
            {
                preAttackTime = Time.time;
                Rigidbody rb = mana.gameObject.GetComponent<Rigidbody>();
                Vector3 force = mana.transform.position - 
                    nearTarget.ClosestPoint(mana.transform.position);
                force.z = 0; force.y = 0; force.x = force.x / Mathf.Abs(force.x);
                force *= 8; force.y = 1;
                rb.velocity = force;
                //rb.AddForce(force);
                nowTime = 0;
                particle.beginPos = mana.transform.position;
                ParticleNoiseFactory.Instance.DrawSphere(particle);
            }
            return false;
        }

    }
}