using UnityEngine;


namespace StateMachine
{
    /// <summary>
    /// 带有遮挡检测的待机状态，该状态会根据主角的遮挡情况判断是否进入搜索状态，
    /// 同时默认检测是视线检查，根据视角范围判断是否要进行检查
    /// </summary>
    [CreateAssetMenu(menuName = "StateMachine/Idle/IdleViewCheckWithShelter")]
    public class IdleViewCheckWithShelter : StateMachineBase
    {
        [Range(0, 180)]
        /// <summary>       /// 检查主角的角度        /// </summary>
        public float CheckAngle = 120, SeeDistance = 30;
        /// <summary>     /// 绘制一下可视范围     /// </summary>
        public bool ShowCheckAngle = false;
        /// <summary>    /// 下一个状态机    /// </summary>
        public StateMachineBase nextState;
        /// <summary>     /// 可遮罩层，在该层中的都可以遮挡主角     /// </summary>
        public LayerMask shalterMask;
        float checkRadian;      //检查的弧度，这个是计算需要的值

        //旋转矩阵
        Matrix4x4 rotateMatrix;

        public override void EnterState(StateMachineManage manage)
        {
            checkRadian = Mathf.Deg2Rad * CheckAngle;
            manage.EnemyMotor.BeginAutoRotate();
        }



        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            Control.PlayerControlBase player = 
                Control.PlayerControlBase.Instance;
            if (player == null) return null;
            Transform transform = manage.transform;
            Vector3 forward = transform.forward, 
                direction = player.transform.position - transform.position;
            float distance = direction.magnitude;
            //绘制检查范围
            if (ShowCheckAngle)
            {
                float cos = Mathf.Cos(checkRadian), sin = Mathf.Sin(checkRadian),
                    minCos = Mathf.Cos(-checkRadian), minSin = Mathf.Sin(-checkRadian);
                rotateMatrix = Matrix4x4.identity;
                //左边
                rotateMatrix.m00 = cos; rotateMatrix.m02 = -sin;
                rotateMatrix.m20 = sin; rotateMatrix.m22 = cos;
                Vector3 vector = rotateMatrix * forward;
                Debug.DrawRay(manage.transform.position, vector * SeeDistance);
                //右边
                rotateMatrix.m00 = minCos; rotateMatrix.m02 = -minSin;
                rotateMatrix.m20 = minSin; rotateMatrix.m22 = minCos;
                vector = rotateMatrix * forward;
                Debug.DrawRay(manage.transform.position, vector * SeeDistance);
                rotateMatrix = Matrix4x4.identity;      //清空数据
                rotateMatrix.m00 = 1;
                //上边
                rotateMatrix.m11 = cos; rotateMatrix.m12 = sin;
                rotateMatrix.m21 = -sin; rotateMatrix.m22 = cos;
                vector = rotateMatrix * forward;
                Debug.DrawRay(manage.transform.position, vector * SeeDistance);
                //下边
                rotateMatrix.m11 = minCos; rotateMatrix.m12 = minSin;
                rotateMatrix.m21 = -minSin; rotateMatrix.m22 = minCos;
                vector = rotateMatrix * forward;
                Debug.DrawRay(manage.transform.position, vector * SeeDistance);
            }

            //检查距离
            if (distance > SeeDistance)
                return null;        //超过就退出
            //检查角度
            if(Vector3.Dot(direction.normalized, forward) < Mathf.Cos(checkRadian))
                return null;        //超过角度退出
            //判断遮挡
            if(Physics.Raycast(transform.position, direction, distance, shalterMask))
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
            if(manage.GetNextPostion(out point))
            {
                if (manage.NavMeshAgent.isOnNavMesh)
                {
                    manage.NavMeshAgent.isStopped = false;   //继续移动
                    manage.NavMeshAgent.SetDestination(point);
                }
            }
            else
            {
                if(manage.NavMeshAgent.isOnNavMesh)
                    manage.NavMeshAgent.isStopped = true;
            }
            return;
        }
    }
}