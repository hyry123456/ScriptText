using UnityEngine;


namespace StateMachine
{
    /// <summary>
    /// 带有遮挡检测的待机状态，该状态会根据主角的遮挡情况判断是否进入搜索状态，
    /// 同时默认检测是视线检查，根据视角范围判断是否要进行检查
    /// </summary>
    [CreateAssetMenu(menuName = "StateMachine/Idle/IdleViewCheckWithShelter2D")]
    public class IdleViewCheckWithShelter2D : StateMachineBase
    {
        [Range(0, 180)]
        /// <summary>       /// 检查主角的角度        /// </summary>
        public float CheckAngle = 60, SeeDistance = 10;
        /// <summary>     /// 绘制一下可视范围     /// </summary>
        public bool ShowCheckAngle = false;
        /// <summary>    /// 下一个状态机    /// </summary>
        public StateMachineBase nextState;
        /// <summary>     /// 可遮罩层，在该层中的都可以遮挡主角     /// </summary>
        public LayerMask shalterMask;
        float checkRadian;      //检查的弧度，这个是计算需要的值

        public override void EnterState(StateMachineManage manage)
        {
            checkRadian = Mathf.Deg2Rad * CheckAngle;
            //manage.EnemyMotor.BeginAutoRotate();
        }



        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            Control.PlayerControlBase player =
                Control.PlayerControlBase.Instance;
            if (player == null) return null;
            Transform transform = manage.transform;
            Vector3 right = transform.right,
                direction = player.transform.position - transform.position;
            float distance = direction.magnitude;
            //绘制检查范围
            if (ShowCheckAngle)
            {
                Vector3 endDir = right * SeeDistance + transform.position;
                float cos = Mathf.Tan(checkRadian / 2);
                Vector3 upDis = cos * SeeDistance * transform.up;
                Debug.DrawLine(transform.position, endDir + upDis);
                Debug.DrawLine(transform.position, endDir - upDis);
            }

            //检查距离
            if (distance > SeeDistance)
                return null;        //超过就退出
            //检查角度
            if (Vector3.Dot(direction.normalized, right) < Mathf.Cos(checkRadian / 2))
                return null;        //超过角度退出
            //判断遮挡
            if (Physics.Raycast(transform.position, direction, distance, shalterMask))
                return null;        //遮挡退出

            //都已经满足，进入下一个状态
            return nextState;
        }


        public override void ExitState(StateMachineManage manage)
        {
            return;
        }

        public override void OnFixedUpdate(StateMachineManage manage)
        {
            Vector3 point;
            if (manage.GetNextPostion(out point))
            {
                Vector3 dir = point - manage.transform.position;
                manage.EnemyMotor.Move(dir.x, 0);
            }
            else
            {
                manage.EnemyMotor.Move(0, 0);
            }
            return;
        }
    }
}