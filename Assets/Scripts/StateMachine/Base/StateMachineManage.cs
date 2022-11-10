using Motor;
using Skill;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    [RequireComponent(typeof(AnimateManage))]
    [RequireComponent(typeof(MotorBase))]
    [RequireComponent(typeof(SkillManage))]
    /// <summary>  
    /// 状态机管理类，用来控制敌人的状态，拥有了状态机后敌人不再需要控制器，
    /// 因为本身状态机就是一个控制器，一个更加完善的控制器
    /// </summary>
    public class StateMachineManage : MonoBehaviour
    {
        private static List<StateMachineManage> enemys;
        public static List<StateMachineManage> Enemys => enemys;
        public static void AddEnemy(StateMachineManage enemy)
        {
            if(enemys == null)
                enemys = new List<StateMachineManage>();
            if (enemys.Contains(enemy))
                return;
            enemys.Add(enemy);
        }

        public static void RemoveEnemy(StateMachineManage enemy)
        {
            enemys.Remove(enemy);
        }


        /// <summary>    
        /// 初始状态，用来作为状态机的初始行为，以及方便直接回退
        /// </summary>
        public StateMachineBase beginState;
        [SerializeField]
        /// <summary>    /// 当前状态，用来作为实时行为    /// </summary>
        StateMachineBase nowState;
        private AnimateManage animate;
        /// <summary>   /// 角色的动画管理类    /// </summary>
        public AnimateManage AnimateManage => animate;

        private MotorBase motor;
        /// <summary>     /// 敌人的运动引擎     /// </summary>
        public MotorBase EnemyMotor => motor;

        private SkillManage skillManage;
        public SkillManage SkillManage => skillManage;
        private Info.CharacterInfo characterInfo;
        public Info.CharacterInfo CharacterInfo => characterInfo;

        private NavMeshAgent navMeshAgent;
        /// <summary>///自动寻路组件 /// </summary>
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        [SerializeField]
        List<Vector3> wayPoints;
        int nowIndex = 0;       //路点循环到的位置

        [SerializeField]
        float nearAttackDistance = 2;
        public float NearAttackDistance => nearAttackDistance;

        [SerializeField]
        float nearDistance = 1; //用来判断是否算是接近距离，在导航是到达该距离就会停止
        /// <summary>     /// 用来判断是否算是接近距离的参数    /// </summary>
        public float NearDistance => nearDistance;
        //保存点数组到底真正存储位置，由于这些保存点都是复用的，因此用一个数组来保存就够了
        private Vector3[] savePoints = new Vector3[1];
        /// <summary>      /// 保存点数组       /// </summary>
        public Vector3[] SavePoints => savePoints;
        /// <summary>
        /// 设置保存点，因为本状态机的执行部分是不包含数据存储的，
        /// 因此需要在挂载的物体上设置一个位置保存数组
        /// </summary>
        /// <param name="index">设置的编号</param>
        /// <param name="point">设置点</param>
        public void SetSavePoints(int index, Vector3 point)
        {
            savePoints[index] = point;
        }

        //同保存点，用来保存比例
        private float[] saveRadios = new float[3];
        /// <summary>      /// 保存比例数组       /// </summary>
        public float[] SaveRadios => saveRadios;
        /// <summary>
        /// 设置比例，因为本状态机的执行部分是不包含数据存储的，
        /// 因此需要在挂载的物体上设置一个位置保存数组
        /// </summary>
        /// <param name="index">设置的编号</param>
        /// <param name="point">设置点</param>
        public void SetSaveRadios(int index, float radio)
        {
            saveRadios[index] = radio;
        }

        /// <summary>
        /// 获得下一个路点，当敌人需要进行巡逻时会执行该方法
        /// </summary>
        /// <param name="point">路点的位置</param>
        /// <returns>是否有路点</returns>
        public bool GetNextPostion(out Vector3 point)
        {
            if(wayPoints == null || wayPoints.Count == 0)
            {
                point = Vector3.zero;
                return false;
            }
            Vector3 nowWayPoint = wayPoints[nowIndex];
            if((nowWayPoint - transform.position).sqrMagnitude < NearDistance)
            {
                if(wayPoints.Count == 1)
                {
                    point = Vector3.zero;
                    return false;
                }
                else
                {
                    nowIndex++; nowIndex %= wayPoints.Count;
                    point = wayPoints[nowIndex];
                    return true;
                }
            }
            else
            {
                point = wayPoints[nowIndex];
                return true;
            }
        }

        /// <summary>     /// 添加路点      /// </summary>
        public void AddNextPosition(Vector3 point)
        {
            if(wayPoints == null)
            {
                wayPoints = new List<Vector3>();
            }
            wayPoints.Add(point);
        }

        private void Start()
        {
            animate = GetComponent<AnimateManage>();
            motor = GetComponent<MotorBase>();
            skillManage = GetComponent<SkillManage>();
            characterInfo = GetComponent<Info.CharacterInfo>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            if(navMeshAgent != null)
                navMeshAgent.speed = characterInfo.walkSpeed;   //默认设置为行走速度

            if (beginState != null)
            {
                beginState.EnterState(this);
                nowState = beginState;
            }

        }

        /// <summary>    /// 逐固定帧运行当前状态机的行为     /// </summary>
        private void FixedUpdate()
        {
            if (nowState == null)
                return;
            nowState.OnFixedUpdate(this);

            StateMachineBase tempState = nowState.CheckState(this);
            if(tempState != null)
            {
                nowState.ExitState(this);
                tempState.EnterState(this);
                nowState = tempState;
            }
        }

        //开始运行时就加入列表中
        private void OnEnable()
        {
            AddEnemy(this);
        }
        //死亡关闭时移除
        private void OnDisable()
        {
            RemoveEnemy(this);
        }
        //被删除时也移除
        private void OnDestroy()
        {
            RemoveEnemy(this);
        }

    }
}