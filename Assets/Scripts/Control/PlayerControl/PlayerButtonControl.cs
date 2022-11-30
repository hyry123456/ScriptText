using UnityEngine;

namespace Control
{
    /// <summary>
    /// 角色按键控制类，由外界调用进行所有按键的管理
    /// </summary>
    public class PlayerButtonControl
    {
        private static PlayerButtonControl instance;
        public static PlayerButtonControl Instance
        {
            get
            {
                if(instance == null)
                    instance = new PlayerButtonControl();
                return instance;
            }
        }
        Common.ResetInput.MyInput myInput;
        private PlayerButtonControl()
        {
            myInput = Common.ResetInput.MyInput.Instance;
        }

        private const string verticalName = "Vertical";
        private const string horizontalName = "Horizontal";
        private const string jumpName = "Jump";
        private const string escName = "ESC";
        private const string interacteName = "Interaction";
        private const string skillName = "Skill";


        public void OnFixUpdate(Motor.MotorBase motor, PlayerSkillControl skillControl)
        {
            ControlMotor(motor);

            bool esc = myInput.GetButtonDown(escName);
            bool interacte = myInput.GetButtonDown(interacteName);
            bool skill = myInput.GetButtonDown(skillName);

            if (interacte)
                Interaction.InteractionControl.Instance.RunInteraction();
            if (skill)
                skillControl.ReleaseChooseSkill();
        }
        private void ControlMotor(Motor.MotorBase motor)
        {
            float vertical = myInput.GetAsis(verticalName);
            float horizontal = myInput.GetAsis(horizontalName);
            bool jump = myInput.GetButtonDown(jumpName);

            motor.Move(horizontal, vertical);       //移动
            if (jump)
                motor.DesireJump();         //跳跃
        }

        public void OnUpdate(PlayerSkillControl skillControl)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UI.UISettings.Instance.ShowOrCloseUISettings();
            }

            if (Input.GetMouseButton(0))
                skillControl.ReleaseChooseSkill();
        }

    }
}