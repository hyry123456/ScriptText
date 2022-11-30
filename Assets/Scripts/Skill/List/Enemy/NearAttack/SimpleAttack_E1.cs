using UnityEngine;
using DefferedRender;

namespace Skill
{

    public class SimpleAttack_E1 : SkillBase
    {
        public SimpleAttack_E1()
        {
            maxSkillCount = -1;
            coolTime = 3f;
            skillCount = maxSkillCount;
            releaseTime = 0.1f;
            skillName = "²âÊÔ¹¥»÷";
            skillType = SkillType.NearDisAttack;
        }


        float attackDistance = 2f;


        public override bool OnSkillRelease(SkillManage mana)
        {
            if (!mana.AnimateManage.SetAttack(1, releaseTime))
                return false;

            Control.PlayerControlBase player = Control.PlayerControlBase.Instance;
            if (player == null) return false;
            Vector3 dir = player.transform.position - mana.transform.position;
            if (Vector3.Dot(dir.normalized, mana.transform.right) > 0.1 &&
                dir.magnitude < attackDistance)
            {
                Info.ExternalAction external = player.GetComponent<Info.ExternalAction>();
                external.AddAction(new Info.ActionData()
                {
                    solide = -10,
                    hp = -10,
                    recall = (info, hp, solid) =>
                    {
                        if(hp < 0)
                        {
                            Rigidbody rigidbody = player.GetComponent<Rigidbody>();
                            rigidbody.AddForce((external.transform.position -
                                mana.transform.position).normalized * 300);

                            particle.beginPos = mana.transform.position;
                            ParticleNoiseFactory.Instance.DrawSphere(particle);
                        }
                    }
                });
            }

            return true;
        }

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

    }
}