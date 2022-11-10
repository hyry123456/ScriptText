using UnityEngine;


namespace Interaction
{
    public class SaveInteracte : InteractionBase
    {
        public override void InteractionBehavior()
        {
            Common.DataSave.Instance.SaveData();
        }

        protected override void OnEnable()
        {
        }
    }
}