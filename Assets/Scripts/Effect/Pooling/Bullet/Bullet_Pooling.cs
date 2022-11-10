using Common;
using DefferedRender;
using UnityEngine;

/// <summary>
/// 池化的子弹，由对象池进行创建以及删除
/// </summary>
public class Bullet_Pooling : ObjectPoolBase
{
    float time;
    /// <summary> /// 子弹最多存活事件，超过就会自动死亡 /// </summary>
    [SerializeField]
    float maxLifeTime = 10;
    /// <summary>  /// 子弹的移动速度 /// </summary>
    [SerializeField]
    float moveSpeed = 10;
    ParticleDrawData drawData;
    /// <summary>    /// 每一帧初始化一下时间，只有时间需要次次初始化    /// </summary>
    protected override void OnEnable()
    {
        time = 0;
    }

    //绘制数据只用初始化一次就够了，因此放在这里初始化
    public override void InitializeObject(Vector3 positon, Vector3 lookAt)
    {
        base.InitializeObject(positon, lookAt);
        Vector2 sizeRange = new Vector2(0.1f, 0.2f);
        drawData = new ParticleDrawData
        {
            beginPos = transform.position,
            beginSpeed = Vector3.up,
            speedMode = SpeedMode.JustBeginSpeed,
            useGravity = true,
            followSpeed = true,
            radian = 3.14f,
            radius = 1f,
            cubeOffset = new Vector3(0.1f, 0.1f, 0.1f),
            lifeTime = 1,
            showTime = 1,
            frequency = 1f,
            octave = 8,
            intensity = 20,
            sizeRange = sizeRange,
            colorIndex = ColorIndexMode.HighlightToAlpha,
            sizeIndex = SizeCurveMode.Small_Hight_Small,
            textureIndex = 0,
            groupCount = 1,
        };
    }

    public override void InitializeObject(Vector3 positon, Quaternion quaternion)
    {
        base.InitializeObject(positon, quaternion);
        Vector2 sizeRange = new Vector2(0.1f, 0.2f);
        drawData = new ParticleDrawData
        {
            beginPos = transform.position,
            beginSpeed = Vector3.up,
            speedMode = SpeedMode.JustBeginSpeed,
            useGravity = true,
            followSpeed = true,
            radian = 3.14f,
            radius = 1f,
            cubeOffset = new Vector3(0.1f, 0.1f, 0.1f),
            lifeTime = 1,
            showTime = 1,
            frequency = 1f,
            octave = 8,
            intensity = 20,
            sizeRange = sizeRange,
            colorIndex = ColorIndexMode.HighlightToAlpha,
            sizeIndex = SizeCurveMode.Small_Hight_Small,
            textureIndex = 0,
            groupCount = 1,
        };
    }

    [HideInInspector]
    /// <summary>  /// 攻击的目标，当打到该目标的敌人时会扣血 /// </summary>
    public LayerMask attackLayer;

    //该技能的伤害效果
    Info.ActionData action = new Info.ActionData
    {
        hp = -5,
        solide = -5,
    };

    private void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time > maxLifeTime)
        {
            CloseObject();
            return;
        }
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & attackLayer.value) == 0)
            return;

        Info.ExternalAction externalAction = 
            other.gameObject.GetComponent<Info.ExternalAction>();

        if (externalAction != null)
            externalAction.AddAction(action);

        drawData.groupCount = 30;
        drawData.beginPos = other.ClosestPoint(transform.position);
        drawData.speedMode = SpeedMode.JustBeginSpeed;
        Vector3 normal = Vector3.Reflect(transform.forward, (drawData.beginPos - transform.position).normalized);
        drawData.beginSpeed = normal * moveSpeed * 0.3f;
        drawData.lifeTime = 5; drawData.showTime = 5f;
        drawData.intensity = 50;

        ParticleNoiseFactory.Instance.DrawPos(drawData);

        drawData.groupCount = 1;
        drawData.speedMode = SpeedMode.JustBeginSpeed;
        drawData.intensity = 20;

        CloseObject();

        return;
    }

}
