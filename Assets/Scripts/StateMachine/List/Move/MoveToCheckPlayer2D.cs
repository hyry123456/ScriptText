using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "StateMachine/Move/MoveToCheckPlayer2D")]
    /// <summary> /// 2D的移动并检查主角 /// </summary>
    public class MoveToCheckPlayer2D : StateMachineBase
    {
        public float SeeDistance = 10,          //敌人的可视距离
            findDistance = 3,          //发现主角的距离
            checkTime = 3;              //在目标点检查的事件
        /// <summary>    /// 下一个攻击状态机    /// </summary>
        public StateMachineBase nextAttackState;
        /// <summary>     /// 可遮罩层，在该层中的都可以遮挡主角     /// </summary>
        public LayerMask shalterMask;

        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            Control.PlayerControlBase player =
                Control.PlayerControlBase.Instance;
            if (player == null) return manage.beginState;
            Vector3 direction = player.transform.position - manage.transform.position;

            if (CheckView(direction, manage))       //检查是否看见
            {
                //看见且在可攻击距离就进入攻击状态
                if (direction.sqrMagnitude < findDistance * findDistance)
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
            {
                return manage.beginState;
            }
            return null;
        }

        public override void EnterState(StateMachineManage manage)
        {
            //保存主角位置，走过去检查
            manage.SetSavePoints(0,
                Control.PlayerControlBase.Instance.transform.position);
            //设置检查比例
            manage.SetSaveRadios(0, 0);
            //manage.EnemyMotor.BeginAutoRotate();
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
            float distance = playerDuration.magnitude;
            //检查距离
            if (distance > SeeDistance)
            {
                return false;        //超过就退出
            }
            //判断遮挡
            if (Physics.Raycast(manage.transform.position, playerDuration, distance, shalterMask))
                return false;        //遮挡退出
            return true;
        }

        /// <summary>        /// 移动向目标位置        /// </summary>
        private void MoveToTarget(StateMachineManage manage)
        {
            //在可视距离外的话就走过去看看
            Vector3 target = manage.SavePoints[0];

            if ((target - manage.transform.position).sqrMagnitude < manage.NearDistance)
            {
                manage.EnemyMotor.Move(0, 0);
                //在近距离时执行近距离行为，也就是等待一下
                OnNearDistance(manage);
            }
            else
            {
                Vector3 dir = target - manage.transform.position;
                manage.EnemyMotor.Move(dir.x, 0);
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