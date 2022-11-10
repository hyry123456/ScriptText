using UnityEngine;


namespace Control
{
    public class PlayerControl_2D : PlayerControlBase
    {

        /// <summary> /// ʱʱˢ�µĿ������ԵĴ��λ��  /// </summary>
        private void Update()
        {
            if (!useControl) return;
            PlayerButtonControl.Instance.OnUpdate(skillControl);
        }

        /// <summary>
        /// ����֡ˢ�µ����Լ���λ�ã�һЩû�б�Ҫ��֡����Ŀ�����������м���
        /// </summary>
        private void FixedUpdate()
        {
            if (!useControl) return;
            if(skillManage.IsReleasing)     //���ƶ�������������
            {
                motor.Move(0, 0);
                return;
            }
            PlayerButtonControl.Instance.OnFixUpdate(motor, skillControl);
        }

        //3D�����ƶ���ֱ�����������ǰ������
        public override Vector3 GetLookatDir()
        {
            return transform.right * 0.1f;
        }
    }
}