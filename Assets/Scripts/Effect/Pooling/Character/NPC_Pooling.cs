using UnityEngine;

namespace Common
{
    /// <summary> /// �ػ�NPC�ı�׼��ʽ��������֤��NPC��ɾ���󲻻�ʣ�½��� /// </summary>
    public class NPC_Pooling : ObjectPoolBase
    {
        protected override void OnEnable()
        {
        }

        /// <summary> /// �������Ľ��� /// </summary>
        public override void CloseObject()
        {
            base.CloseObject();
            Interaction.InteractionBase[]
                interactions = GetComponents<Interaction.InteractionBase>();
            for(int i=0; i<interactions.Length; i++)
            {
                Destroy(interactions[i]);
            }
        }
    }
}