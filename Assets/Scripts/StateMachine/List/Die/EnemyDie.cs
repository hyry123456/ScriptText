using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine {

    [CreateAssetMenu(menuName = "StateMachine/Die/EnemyDie")]
    /// <summary>  ///��������  /// </summary>
    public class EnemyDie : StateMachineBase
    {


        public override StateMachineBase CheckState(StateMachineManage manage)
        {
            return null;
        }

        public override void EnterState(StateMachineManage manage)
        {
            //Ӧ����Ҫ�ر�һЩ����
            //manage.AnimateManage.PlayAnimate(AnimateType.Die);//���Ŷ���
        }

        public override void ExitState(StateMachineManage manage)
        {
            
        }

        public override void OnFixedUpdate(StateMachineManage manage)
        {
            
        }

    }


}


