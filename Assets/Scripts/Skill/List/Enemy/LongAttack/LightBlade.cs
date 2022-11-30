using UnityEngine;
using DefferedRender;
using Common;

namespace Skill
{
    /// <summary>
    /// 光刃攻击，划出一道水平的光刃
    /// </summary>
    public class LightBlade : SkillBase
    {
        ParticleDrawData particle = new ParticleDrawData
        {
            //beginPos = Vector3.zero,
            beginSpeed = Vector3.down,
            speedMode = SpeedMode.PositionOutside,
            //cubeOffset = Vector3.one * 50,
            useGravity = false,
            followSpeed = true,
            radius = 0.5f,
            radian = 0.6f,
            lifeTime = 5,
            showTime = 5,
            frequency = 1,
            octave = 4,
            intensity = 1,
            sizeRange = Vector2.up * 0.5f,
            colorIndex = ColorIndexMode.HighlightToAlpha,
            textureIndex = 0,
            groupCount = 1
        };

        public LightBlade()
        {
            maxSkillCount = -1;
            skillCount = maxSkillCount;
            preReleaseTime = 0;
            releaseTime = 2;
            coolTime = 6;
            skillName = "Wave Blade";
            skillType = SkillType.LongDisAttack;
            nowTime = waitTime + 1;
        }

        public override bool OnSkillRelease(SkillManage mana)
        {
            if (nowTime < waitTime)
                return false;

            manage = mana;
            if(!mana.AnimateManage.SetAttack(2, releaseTime))
                return false;
            mana.AnimateManage.animateEvent = (var) =>
            {
                isStop = !var;
                isRelease = true;
            };
            isRelease = false;
            InitialOnHandParticle();
            SustainCoroutine.Instance.AddCoroutine(ParticleOnHand);
            return true;
        }

        bool isRelease, isStop;
        float nowTime, waitTime = 3, moveSpeed = 35;
        SkillManage manage;
        Vector3 pos,            //当前位置
            direction;          //移动方向
        Quaternion quaternion;  //初始朝向
        bool ParticleOnHand()
        {
            if (isRelease)
            {
                if (isStop)
                    return true;
                pos = manage.AnimateManage.RightHand.position;  //火焰在右手，只初始化在右手
                direction = manage.transform.forward;
                quaternion = manage.transform.rotation;
                nowTime = 0;
                particle.endPos = manage.transform.eulerAngles;     //设置角度
                InitialMoveParticle();
                InitialCheckBoxCollider();          //初始化碰撞器
                SustainCoroutine.Instance.AddCoroutine(CheckOnTrigger);
                return true;
            }
            //绘制右手
            particle.beginPos = manage.AnimateManage.RightHand.position;

            ParticleNoiseFactory.Instance.DrawSphere(particle);
            return false;
        }


        Vector3 checkBox = new Vector3(2.5f, 2, 1);
        bool CheckOnTrigger()
        {
            pos += direction * Time.deltaTime * moveSpeed;
            nowTime += Time.deltaTime;
            if(poolingBox == null)  return true;    //如果碰撞物体被消除了，就直接退出
            //超时销毁
            if (nowTime > waitTime)
            {
                poolingBox.CloseObject();
                poolingBox = null;
                return true;
            }
            particle.beginPos = pos;
            ParticleNoiseFactory.Instance.DrawSphere(particle);

            poolingBox.transform.position = pos;

            //if (Physics.BoxCast(pos, checkBox, direction, quaternion, 
            //    0.5f, manage.CharacterInfo.attackLayer))
            //{

            //}

            return false;
        }

        /// <summary>    /// 在手臂时的粒子初始化     /// </summary>
        void InitialOnHandParticle()
        {
            particle.speedMode = SpeedMode.JustBeginSpeed;
            particle.beginSpeed = Vector3.up;
            particle.radius = 0.01f;
            particle.radian = 6.28f;
            particle.endPos = Vector3.zero;
            particle.groupCount = 1;
            particle.lifeTime = 1;
            particle.showTime = 1;
            particle.intensity = 5;
            particle.sizeRange = new Vector2(0.1f, 3f);
        }

        //移动时的粒子初始化
        void InitialMoveParticle()
        {
            particle.speedMode = SpeedMode.JustBeginSpeed;
            particle.beginSpeed = -direction;
            particle.radius = 2f;
            particle.radian = 0.6f;
            particle.groupCount = 2;
            particle.lifeTime = 0.2f;
            particle.showTime = 0.2f;
            particle.intensity = 5;
            particle.sizeRange = new Vector2(0.5f, 1f);
        }

        PoolingBox poolingBox;

        //初始化盒子碰撞体
        void InitialCheckBoxCollider()
        {
            PoolingBox origin = PoolingCollisionOrigin.PoolingBoxOrigin;
            poolingBox = SceneObjectPool.Instance.GetObject<PoolingBox>
                (origin.name, origin.gameObject, pos, quaternion);
            poolingBox.InitializeBox(checkBox, (collsion) =>
            {
                //不是要碰撞的物体，继续执行，退出该方法
                if (((1 << collsion.gameObject.layer) & manage.CharacterInfo.attackLayer.value) == 0)
                    return;
                //是碰撞的物体，开始销毁
                particle.beginPos = pos;
                particle.beginSpeed = (pos - manage.transform.position).normalized * 10;
                particle.groupCount = 30;
                particle.speedMode = SpeedMode.VerticalVelocityOutside;
                particle.lifeTime = 5;
                particle.showTime = 4f;
                particle.intensity = 10;
                ParticleNoiseFactory.Instance.DrawSphere(particle);
                nowTime = waitTime + 1;
                poolingBox.CloseObject();
                poolingBox = null;
                return;
            });
        }

    }
}