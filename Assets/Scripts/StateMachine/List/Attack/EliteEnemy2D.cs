using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// 精英敌人状态机2D，用来描述一个2D的智能敌人
    /// </summary>
    [CreateAssetMenu(menuName = "StateMachine/Attack/EliteEnemy2D")]
    public class EliteEnemy2D : StateMachineBase
    {
        /// <summary>    /// 遮挡层，用来检查是否主角在被遮挡中    /// </summary>
        public LayerMask shelterMask;

        public float seeDistance = 5,    //敌人的检查距离，如果大于该距离就会回到跟随状态
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
                Debug.Log("Max");
                //大于检查距离，去找到主角
                return followPlayer;
            }

            //被遮挡了，去找到主角
            if (manage.SaveRadios[2] > shelterTime)
                return followPlayer;

            if (Physics.Raycast(manage.transform.position, duration, distance, shelterMask))
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
            GetWaitingData(manage);
            manage.SetSaveRadios(0, 1);     //设置0位为随机方向
            manage.SetSaveRadios(1, moveWaitTime + 1);     //设置1位为随机移动时间
            manage.SetSaveRadios(2, 0);     //设置第三个值为遮挡检查值
            manage.EnemyMotor.Move(0, 0);
            manage.EnemyMotor.EndAutoRotate();

            if (playerSkillManage == null)
            {
                playerSkillManage = Control.PlayerControlBase.
                    Instance.gameObject.GetComponent<Skill.SkillManage>();
            }
            return;
        }

        public override void ExitState(StateMachineManage manage)
        {
            manage.EnemyMotor.BeginAutoRotate();
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
            if (IsShuffling(manage, player.transform))
            {
                Shuffling(manage, player.transform);
                return;
            }

            if (AttackPlayer(manage, player.transform))
            {
                return;
            }
            else
            {
                GetWaitingData(manage);
            }
        }


        /// <summary>     /// 生成一个待机时间     /// </summary>
        private void GetWaitingData(StateMachineManage manage)
        {
            manage.SetSaveRadios(1, 0);         //初始化时间
        }
        /// <summary>     /// 进行前后移动     /// </summary>
        private void Shuffling(StateMachineManage manage, Transform player)
        {
            Vector3 targetDir = player.position - manage.transform.position;
            manage.EnemyMotor.Move(targetDir.x / 
                manage.CharacterInfo.walkSpeed, 0);
        }
        /// <summary>    /// 判断是否正在待机，是就更新待机时间       /// </summary>
        private bool IsShuffling(StateMachineManage manage, Transform player)
        {
            if (manage.SaveRadios[1] > moveWaitTime)
                return false;
            Vector3 targetDir = player.position - manage.transform.position;
            //太近就停止移动
            if(Mathf.Abs(targetDir.x) < manage.NearDistance)
            {
                manage.SetSaveRadios(1, moveWaitTime + 1);
                manage.EnemyMotor.Move(0, 0);
                return false;
            }
            manage.SetSaveRadios(1, manage.SaveRadios[1] + Time.deltaTime);
            return true;
        }

        /// <summary>   /// 攻击主角，判断是否可以攻击   /// </summary>
        /// <returns>是否有释放技能</returns>
        private bool AttackPlayer(StateMachineManage manage, Transform player)
        {
            float sqrDis = (player.position - manage.transform.position).sqrMagnitude;
            //近战范围，检查近战技能
            if (sqrDis <= manage.NearAttackDistance)
            {
                Skill.SkillBase skill = manage.SkillManage.GetRandomSkillByType(
                    Skill.SkillType.NearDisAttack);
                if (skill != null)
                    if (manage.SkillManage.CheckAndRelase(skill))
                    {
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


        /// <summary>      /// 敌人朝向主角    /// </summary>
        private void OrientationPlayer(StateMachineManage manage, Transform player)
        {
            Vector3 targetDir = player.position - manage.transform.position;
            targetDir.y = 0;        //防止Y轴出错
            manage.transform.right = targetDir.x > 0 ? Vector3.right : Vector3.left;
        }

    }
}