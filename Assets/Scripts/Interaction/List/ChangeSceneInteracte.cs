using Common;
using UnityEngine;

namespace Interaction
{
    /// <summary> /// �ı䳡���õĽ���  /// </summary>
    public class ChangeSceneInteracte : InteractionBase
    {
        public string sceneName;
        public int targetIndex;

        public override void InteractionBehavior(INonReturnAndNonParam recall)
        {
            //���س���
            Common.SceneControl.Instance.ChangeScene(sceneName, targetIndex);
        }

        public override string GetInteracteRemain()
        {
            return "����" + Common.ResetInput.MyInput.
                Instance.GetAxisKey("Interaction").ToString() + "�л�����";
        }

        protected override void OnEnable()
        {
        }
    }
}