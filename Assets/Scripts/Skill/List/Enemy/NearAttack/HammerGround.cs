using UnityEngine;
using DefferedRender;

namespace Skill
{
    /// <summary>
    /// 捶地范围攻击
    /// </summary>
    public class HammerGround : SkillBase
    {

        public HammerGround()
        {
            maxSkillCount = -1;
            skillCount = maxSkillCount;
            preReleaseTime = 0;
            releaseTime = 2.1f;
            coolTime = 5;
            skillName = "捶地";
            skillType = SkillType.NearDisAttack;
        }

        ParticleDrawData drawOutside = new ParticleDrawData
        {
            //begin，end，
            beginSpeed = Vector3.up * 10,
            speedMode = SpeedMode.VerticalVelocityOutside,
            useGravity = false,
            followSpeed = true,
            cubeOffset = Vector3.one,
            lifeTime = 3,
            showTime = 2.5f,
            frequency = 1,
            octave = 8,
            intensity = 20,
            sizeRange = new Vector2(0.7f, 1.5f),
            colorIndex = ColorIndexMode.HighlightToAlpha,
            sizeIndex = SizeCurveMode.SmallToBig_Subken,
            textureIndex = 0,
            groupCount = 30,
        };

        public override bool OnSkillRelease(SkillManage mana)
        {
            manage = mana;
            if (isReleasing)
                return false;
            if(!mana.AnimateManage.SetAttack(3, releaseTime))
                return false;
            isReleasing = true;
            mana.AnimateManage.animateEvent = (var) =>
            {
                isStop = !var;
                //释放结束
                isReleasing = false;
            };
            Common.SustainCoroutine.Instance.AddCoroutine(CheckRelease);
            return true;
        }

        /// <summary>  /// 用来判断是否已经锤到地面   /// </summary>
        bool isReleasing, isStop;

        SkillManage manage;

        bool CheckRelease()
        {
            //是就释放捶地特效
            if (!isReleasing)
            {
                if (isStop)
                    return true;
                drawOutside.beginPos = manage.AnimateManage.RightHand.position;
                drawOutside.endPos = manage.AnimateManage.LeftHand.position;
                ParticleNoiseFactory.Instance.DrawCube(drawOutside);
                return true;
            }
            return false;
        }
    }
}