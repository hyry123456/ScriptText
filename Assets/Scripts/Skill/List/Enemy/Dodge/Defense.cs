using UnityEngine;
using Common;
using DefferedRender;

namespace Skill
{
    /// <summary>
    /// 防御技能，创建一个球型碰撞体，以及特效作为技能
    /// </summary>
    public class Defense : SkillBase
    {
        float beginTime,              //释放的初始时间
        maxSustainTime = 5;     //护盾最大存在时间

        int maxDefense = 100,      //最大防御值为30
            defense;                //当前剩下的防御值

        PoolingSphere sphere;
        SkillManage manage;

        public Defense()
        {
            maxSkillCount = -1;
            skillCount = maxSkillCount;
            preReleaseTime = 0;
            //要保证释放时间大于护盾存在时间，不然一结束就立刻释放技能了
            releaseTime = maxSustainTime + 1;
            coolTime = 7;
            skillName = "展开护罩";
            skillType = SkillType.Dodge;
            beginTime = Time.time - maxSustainTime;
        }

        ParticleDrawData particle = new ParticleDrawData
        {
            speedMode = SpeedMode.PositionOutside,
            useGravity = false,
            followSpeed = false,
            radian = 6.28f,
            radius = 3,
            lifeTime = 0.5f,
            showTime = 0.5f,
            frequency = 1,
            octave = 8,
            intensity = 5,
            sizeRange = new Vector2(0.1f, 0.3f),
            colorIndex = ColorIndexMode.HighlightAlphaToAlpha,
            sizeIndex = SizeCurveMode.SmallToBig_Epirelief,
            textureIndex = 0,
            groupCount = 1,
        };

        public override bool OnSkillRelease(SkillManage mana)
        {
            manage = mana;
            //还在运行中
            if (Time.time - beginTime < maxSustainTime)
                return false;

            if (!mana.AnimateManage.SetAttack(4, 1.5f))
                return false;

            isRelease = true;
            mana.AnimateManage.animateEvent = (var) =>
            {
                isStop = !var;
                isRelease = false;
            };

            SustainCoroutine.Instance.AddCoroutine(WaitRelease);
            return true;
        }



        /// <summary>     /// 防御中执行的协程      /// </summary>
        bool Defensing()
        {
            ////护盾破碎，退出
            //if(sphere == null)
            //{
            //    Debug.Log("破碎");
            //    beginTime = Time.time - maxSustainTime;
            //    return true;
            //}
            if(Time.time - beginTime > maxSustainTime)
            {
                sphere.CloseObject();
                sphere = null;
                return true;
            }

            //检查伤害
            PoolingList<Info.ActionData> actions = external.GetActions;
            while(actions.Count > 0)
            {
                Info.ActionData action = actions.GetValue(0);
                actions.Remove(0);
                defense += action.hp;
                //护盾破碎，直接死亡
                if(defense < 0)
                {
                    beginTime = Time.time - maxSustainTime;
                    sphere.CloseObject();
                    sphere = null;
                    //修改结束时间
                    manage.SetEndTime(
                        Mathf.Min(Time.time + 1, manage.EndTime));
                    return true;
                }
            }

            particle.beginPos = manage.transform.position;
            ParticleNoiseFactory.Instance.DrawSphere(particle);
            sphere.transform.position = particle.beginPos;

            return false;
        }


        /// <summary>
        /// 初始化全部数据，时间、护盾值
        /// </summary>
        void InitializeDate()
        {
            defense = maxDefense;   //初始化防御力
            //初始化时间
            beginTime = Time.time;
        }

        bool isRelease, isStop;

        /// <summary>     /// 等待防护罩启动     /// </summary>
        bool WaitRelease()
        {
            if (!isRelease)
            {
                if (isStop) return true;
                InitializeCollider();   //初始化碰撞体
                InitializeDate();       //初始化数据
                SustainCoroutine.Instance.AddCoroutine(Defensing);
                return true;
            }
            return false;
        }

        Info.ExternalAction external;

        void InitializeCollider()
        {
            PoolingSphere origin = PoolingCollisionOrigin.PoolingSphereOrigin;
            sphere = SceneObjectPool.Instance.GetObject<PoolingSphere>
                (origin.name, origin.gameObject, manage.transform.position,
                Quaternion.identity);
            //碰撞时更新时间
            sphere.InitializeSphere(3, null);
            
            external = sphere.gameObject.AddComponent<Info.ExternalAction>();
        }
    }
}