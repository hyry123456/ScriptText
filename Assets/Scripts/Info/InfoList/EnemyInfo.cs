using Control;
using DefferedRender;
using UnityEngine;

namespace Info
{
    public class EnemyInfo : CharacterInfo
    {
        /// <summary>   /// 敌人的可视距离   /// </summary>
        public float seeDistance = 50;

        /// <summary>   /// 死亡时执行的行为   /// </summary>
        public Common.INonReturnAndNonParam dieBehavior;

        private void Start()
        {
            Vector2 sizeRange = new Vector2(0.4f, 1.5f);
            //drawData = new ParticleDrawData
            //{
            //    beginSpeed = Vector3.up * 10,
            //    speedMode = SpeedMode.VerticalVelocityOutside,
            //    useGravity = false,
            //    followSpeed = false,
            //    radian = 3.14f,
            //    radius = 1f,
            //    lifeTime = 4,
            //    showTime = 4f,
            //    frequency = 1f,
            //    octave = 4,
            //    intensity = 20,
            //    sizeRange = sizeRange,
            //    colorIndex = ColorIndexMode.AlphaToAlpha,
            //    sizeIndex = SizeCurveMode.SmallToBig_Subken,
            //    textureIndex = 1,
            //    groupCount = 10,
            //};
        }


        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void DealWithDeath()
        {
            //drawData.beginPos = transform.position;
            //ParticleNoiseFactory.Instance.DrawPos(drawData);
            ////执行死亡时的附加行为
            //if(dieBehavior != null)
            //{
            //    dieBehavior();
            //    dieBehavior = null;
            //}
            //enemyControl.CloseObject();
        }

    }
}