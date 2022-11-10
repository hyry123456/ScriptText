using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// 精英敌人状态机，用来描写一个智能的敌人
    /// </summary>
    [CreateAssetMenu(menuName = "StateMachine/Attack/EliteEnemy")]
    public class EliteEnemy : StateMachineBase
    {
        /// <summary>    /// 遮挡层，用来检查是否主角在被遮挡中    /// </summary>
        public LayerMask shelterMask;

        public float seeDistance,    //敌人的检查距离，如果大于该距离就会回到跟随状态
            moveWaitTime = 2,       //敌人左右移动的时间，给主角缓冲区
            shelterTime = 3;        //主角被遮挡超过该时间就会进入查找状态

        /// <summary>    /// 跟随主角用的状态机    /// </summary>
        public StateMachineBase followPlayer;
        private Skill.SkillManage playerSkillManage;

        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            Control.PlayerControlBase player = 
                Control.PlayerControlBase.Instance;
            if (player == null) return null;
            Vector3 duration = player.transform.position - manage.transform.position;
            float distance = duration.magnitude;
            if (distance > seeDistance)
            {
                //大于检查距离，去找到主角
                return followPlayer;
            }

            //被遮挡了，去找到主角
            if(manage.SaveRadios[2] > shelterTime)
                return followPlayer;

            if(Physics.Raycast(manage.transform.position, duration, distance, shelterMask))
            {
                manage.SetSaveRadios(2, manage.SaveRadios[2] + Time.deltaTime);
            }
            else
            {
                manage.SetSaveRadios(2, 0);
            }

            return null;
        }

        public override void EnterState(StateMachineManage manage)
        {
            GetShufflingData(manage);
            manage.NavMeshAgent.isStopped = true;
            manage.SetSaveRadios(0, 1);     //设置0位为随机方向
            manage.SetSaveRadios(1, moveWaitTime + 1);     //设置1位为随机移动时间
            manage.SetSaveRadios(2, 0);     //设置第三个值为遮挡检查值

            if(playerSkillManage == null)
            {
                playerSkillManage = Control.PlayerControlBase.
                    Instance.gameObject.GetComponent<Skill.SkillManage>();
            }

            //关闭自动旋转
            manage.EnemyMotor.EndAutoRotate();

            return;
        }

        public override void ExitState(StateMachineManage manage)
        {
            return;
        }

        public override void OnFixedUpdate(StateMachineManage manage)
        {
            Control.PlayerControlBase player = 
                Control.PlayerControlBase.Instance;
            if (player == null) return;
            OrientationPlayer(manage, player.transform);

            if (manage.SkillManage.IsReleasing)
            {
                return;
            }

            //判断移动
            if (IsShuffling(manage))
            {
                Shuffling(manage);
                return;
            }

            if(AttackPlayer(manage, player.transform))
            {
                manage.EnemyMotor.Move(0, 0);
                return;
            }
            else
            {
                GetShufflingData(manage);
            }
        }

        /// <summary>      /// 敌人朝向主角    /// </summary>
        private void OrientationPlayer(StateMachineManage manage, Transform player)
        {
            Vector3 targetDir = player.position - manage.transform.position;
            targetDir.y = 0;        //防止Y轴出错
            manage.transform.rotation =
                Quaternion.Lerp(manage.transform.rotation,
                Quaternion.LookRotation(targetDir, Vector3.up),
                manage.CharacterInfo.rotateSpeed * Time.deltaTime);
        }


        /// <summary>     /// 生成一个左右移动参数     /// </summary>
        private void GetShufflingData(StateMachineManage manage)
        {
            int radom = (Random.value < 0.5f) ? 1 : -1;
            manage.SetSaveRadios(0, radom);     //生成一个方向
            manage.SetSaveRadios(1, 0);         //初始化时间
        }
        /// <summary>     /// 进行左右移动     /// </summary>
        private void Shuffling(StateMachineManage manage)
        {
            Vector3 right = manage.transform.right * manage.SaveRadios[0];
            manage.EnemyMotor.Move(right.z, right.x);
        }
        /// <summary>    /// 判断是否正在左右移动，是就更新左右移动时间       /// </summary>
        private bool IsShuffling(StateMachineManage manage)
        {
            if (manage.SaveRadios[1] > moveWaitTime)
                return false;
            manage.SetSaveRadios(1, manage.SaveRadios[1] + Time.deltaTime);
            return true;
        }

        /// <summary>   /// 攻击主角，判断是否可以攻击   /// </summary>
        /// <returns>是否有释放技能</returns>
        private bool AttackPlayer(StateMachineManage manage, Transform player)
        {

            float sqrDis = (player.position - manage.transform.position).sqrMagnitude;
            //近战范围，检查近战技能
            if(sqrDis <= manage.NearAttackDistance)
            {
                Skill.SkillBase skill = manage.SkillManage.GetRandomSkillByType(
                    Skill.SkillType.NearDisAttack);
                if (skill != null)
                    if (manage.SkillManage.CheckAndRelase(skill))
                    {
                        Debug.Log("释放了 " + skill.skillName);
                        return true;
                    }

            }
            else
            {
                Skill.SkillBase skill = manage.SkillManage.GetRandomSkillByType(
                    Skill.SkillType.LongDisAttack);
                if (skill != null)
                {
                    if (manage.SkillManage.CheckAndRelase(skill))
                    {
                        //Debug.Log("释放了 " + skill.skillName);
                        return true;
                    }
                }
            }

            //主角释放技能中，执行防御技能
            if (playerSkillManage.IsReleasing)
            {
                Skill.SkillBase skill = manage.SkillManage.GetRandomSkillByType(
                    Skill.SkillType.Dodge);
                //有技能才释放
                if (skill != null)
                    if (manage.SkillManage.CheckAndRelase(skill))
                    {
                        return true;
                    }
            }

            return false;
        }

    }
}