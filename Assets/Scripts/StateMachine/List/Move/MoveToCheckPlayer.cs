using UnityEngine;


namespace StateMachine
{
    /// <summary>  /// 移动过去检查主角  /// </summary>
    [CreateAssetMenu(menuName = "StateMachine/Move/MoveToCheckPlayer")]
    public class MoveToCheckPlayer : StateMachineBase
    {
        public float CheckAngle = 120,  //敌人的可视角度
            SeeDistance = 30f,          //敌人的可视距离
            findDistance = 10,          //发现主角的距离
            checkTime = 3;              //在目标点检查的事件
        public float radius = 1, height = 2;
        /// <summary>    /// 下一个攻击状态机    /// </summary>
        public StateMachineBase nextAttackState;
        /// <summary>     /// 可遮罩层，在该层中的都可以遮挡主角     /// </summary>
        public LayerMask shalterMask;
        private float cosCheck;

        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            Control.PlayerControlBase player = 
                Control.PlayerControlBase.Instance;
            if (player == null) return manage.beginState;
            Vector3 direction = player.transform.position - manage.transform.position;

            if (CheckView(direction, manage))
            {
                if(direction.sqrMagnitude < findDistance * findDistance)
                {
                    return nextAttackState;
                }
                else
                {
                    //刷新检查位置
                    manage.SetSavePoints(0, 
                        Control.PlayerControlBase.Instance.transform.position);
                    //设置检查比例
                    manage.SetSaveRadios(0, 0);
                }
            }
            MoveToTarget(manage);
            if (manage.SaveRadios[0] > checkTime)
                return manage.beginState;
            return null;
        }

        public override void EnterState(StateMachineManage manage)
        {
            //保存主角位置，走过去检查
            manage.SetSavePoints(0, 
                Control.PlayerControlBase.Instance.transform.position);
            //设置检查比例
            manage.SetSaveRadios(0, 0);
            manage.EnemyMotor.BeginAutoRotate();

            cosCheck = Mathf.Cos(Mathf.Deg2Rad * CheckAngle);
        }

        public override void ExitState(StateMachineManage manage)
        {
        }

        public override void OnFixedUpdate(StateMachineManage manage)
        {
            //本质上只有检查，既然只有检查，且检查的运算量较大，就全部扔到检查部分了
        }

        /// <summary>
        /// 检查是否在视角、距离范围，且不被遮挡
        /// </summary>
        /// <param name="playerDuration">主角的方向，包含距离的方向</param>
        /// <returns>是否被检查到</returns>
        private bool CheckView(Vector3 playerDuration, StateMachineManage manage)
        {
            //检查距离
            if (playerDuration.sqrMagnitude > SeeDistance * SeeDistance)
                return false;
            //检查角度
            if(Vector3.Dot(playerDuration.normalized, manage.transform.forward) < cosCheck)
                return false;
            //检查遮挡
            if (Physics.CapsuleCast(
                manage.transform.position, manage.transform.position + Vector3.up * height, 
                radius, playerDuration.normalized, playerDuration.magnitude, shalterMask))
            {
                return false;        //遮挡退出
            }
            return true;
        }

        /// <summary>        /// 移动向目标位置        /// </summary>
        private void MoveToTarget(StateMachineManage manage)
        {
            //在可视距离外的话就走过去看看
            Vector3 target = manage.SavePoints[0];
            if ((target - manage.transform.position).sqrMagnitude < manage.NearDistance)
            {
                manage.NavMeshAgent.isStopped = true;   //停止移动
                //在近距离时执行近距离行为
                OnNearDistance(manage);
            }
            else
            {
                manage.NavMeshAgent.isStopped = false;   //继续移动
                //导航向目标点
                manage.NavMeshAgent.SetDestination(manage.SavePoints[0]);
            }
        }

        /// <summary>     /// 当处于近距离时执行的方法，比如播放动画之类的    /// </summary>
        private void OnNearDistance(StateMachineManage manage)
        {
            //添加检查的比例
            manage.SetSaveRadios(0, manage.SaveRadios[0] + Time.deltaTime);
        }
    }
}