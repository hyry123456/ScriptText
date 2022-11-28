using UnityEngine;
using DefferedRender;
using Common;

namespace Skill
{

    public class FireOnHandAttack : SkillBase
    {
        public FireOnHandAttack()
        {
            maxSkillCount = -1;
            skillCount = maxSkillCount;
            releaseTime = 2.2f;
            coolTime = 10;
            skillName = "喷射火焰";
            skillType = SkillType.LongDisAttack;
            nowTime = sustainTime + 1;
        }

        SkillManage manage;
        ParticleDrawData particleDrawData = new ParticleDrawData
        {
            speedMode = SpeedMode.JustBeginSpeed,
            useGravity = false,
            followSpeed = false,
            radian = 6.28f,
            lifeTime = 0.3f,
            showTime = 0.3f,
            frequency = 1,
            octave = 8,
            intensity = 5,
            sizeRange = new Vector2(0, 0.3f),
            colorIndex = ColorIndexMode.HighlightAlphaToAlpha,
            sizeIndex = SizeCurveMode.SmallToBig_Epirelief,
            textureIndex = 1,
            groupCount = 1,
        };

        ParticleDrawData drawOutside = new ParticleDrawData
        {
            speedMode = SpeedMode.VerticalVelocityOutside,
            useGravity = false,
            followSpeed = false,
            radian = 6.28f,
            radius = 1,
            lifeTime = 3,
            showTime = 3,
            frequency = 1,
            octave = 8,
            intensity = 30,
            sizeRange = new Vector2(0.7f, 1.5f),
            cubeOffset = new Vector3(1, 0, 0),
            colorIndex = ColorIndexMode.HighlightRedToBlue,
            sizeIndex = SizeCurveMode.SmallToBig_Subken,
            textureIndex = 1,
            groupCount = 30,
        };

        public override bool OnSkillRelease(SkillManage mana)
        {
            AnimateManage animate = mana.AnimateManage;
            if (animate == null)
                return false;
            if (nowTime < sustainTime)
                return false;
            manage = mana;
            mana.AnimateManage.SetAttack(1, releaseTime);
            release = false;
            mana.AnimateManage.animateEvent = (var) =>
            {
                isStop = !var;
                release = true;
            };
            SustainCoroutine.Instance.AddCoroutine(FireOnHand);
            return true;
        }

        bool release, isStop;
        Vector3 pos, direction;
        float nowTime = 0,          //当前时间
            sustainTime = 5,     //火焰存在时间
            speed = 30;         //火焰移动速度

        float defense;
        float maxDefense = 100;     //最多承受伤害值

        Info.ExternalAction external;

        PoolingSphere sphere;
        //确定下技能型对象
        LayerMask skillLayer = LayerMask.NameToLayer("Effect");

        Info.ActionData action = new Info.ActionData
        {
            hp = -10,
            solide = -10,
        };


        bool FireOnHand()
        {
            if (release)
            {
                if (isStop)
                    return true;
                pos = (manage.AnimateManage.RightHand.position 
                    + manage.AnimateManage.LeftHand.position) / 2;
                direction = manage.transform.forward;
                nowTime = 0;
                InitializeSphere();
                SustainCoroutine.Instance.AddCoroutine(CheckOnTrigger);
                return true;
            }
            //先绘制左手
            particleDrawData.beginPos = manage.AnimateManage.LeftHand.position;
            particleDrawData.speedMode = SpeedMode.JustBeginSpeed;
            particleDrawData.beginSpeed = Vector3.up;
            particleDrawData.radius = 0.001f;
            ParticleNoiseFactory.Instance.DrawSphere(particleDrawData);
            //绘制右手
            particleDrawData.beginPos = manage.AnimateManage.RightHand.position;
            ParticleNoiseFactory.Instance.DrawSphere(particleDrawData);
            return false;
        }

        bool CheckOnTrigger()
        {
            pos += direction * Time.deltaTime * speed;
            nowTime += Time.deltaTime;
            if(sphere == null)
            {
                return true;
            }

            //超时销毁
            if (nowTime > sustainTime)
            {
                sphere.CloseObject();
                sphere = null;
                return true;
            }
            defense += external.GetHPChange;
            if (defense < 0)
            {
                DrawExplodeAndClose();
                return true;
            }

            particleDrawData.speedMode = SpeedMode.JustBeginSpeed;
            particleDrawData.beginSpeed = -direction * speed;
            particleDrawData.radius = 2;
            particleDrawData.beginPos = pos;
            ParticleNoiseFactory.Instance.DrawSphere(particleDrawData);

            //更新位置
            sphere.transform.position = pos;


            return false;
        }

        //初始化球型碰撞体
        void InitializeSphere()
        {
            PoolingSphere origin = PoolingCollisionOrigin.PoolingSphereOrigin;
            sphere = SceneObjectPool.Instance.GetObject<PoolingSphere>
                (origin.name, origin.gameObject, pos, Quaternion.identity);

            external = sphere.gameObject.AddComponent<Info.ExternalAction>();

            sphere.InitializeSphere(2, (collider) =>
            {
                //不是要碰撞的物体，继续执行，退出该方法
                if (((1 << collider.gameObject.layer) 
                    & manage.CharacterInfo.attackLayer.value) == 0)
                    return;

                Info.ExternalAction external = collider.gameObject.GetComponent<Info.ExternalAction>();
                if (external != null)
                {
                    external.AddAction(action);
                    Rigidbody rigidbody = collider.gameObject.GetComponent<Rigidbody>();
                    if(rigidbody != null)
                        rigidbody.AddForce(direction * speed * 100);
                }

                //不管技能物体，是技能物体就退出,不直接破碎
                if (collider.gameObject.layer == skillLayer.value)
                    return;

                DrawExplodeAndClose();
                return;
            });

            defense = maxDefense;
        }

        void DrawExplodeAndClose()
        {
            drawOutside.beginPos = pos;
            drawOutside.beginSpeed = (drawOutside.beginPos - manage.transform.position).normalized;
            ParticleNoiseFactory.Instance.DrawSphere(drawOutside);
            nowTime = sustainTime + 1;      //标记停止
            sphere.CloseObject();
            sphere = null;              //退出协程标记
        }
    }
}