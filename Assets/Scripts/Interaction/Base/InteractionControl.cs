
using UnityEngine;

namespace Interaction
{
    public class InteractionControl : MonoBehaviour
    {
        private static InteractionControl instance;

        public static InteractionControl Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("intreraction");
                    instance = go.AddComponent<InteractionControl>();
                    go.hideFlags = HideFlags.HideAndDontSave;
                }
                return instance;
            }
        }

        private Control.PlayerControlBase playerControl;
        public Control.PlayerControlBase PlayerControl
        {
            get
            {
                if (playerControl == null)
                    playerControl = Control.PlayerControlBase.Instance;
                return playerControl;
            }
        }

        /// <summary>        /// ��ǰ���Դ����Ľ�����Ϣ        /// </summary>
        public InteractionBase nowInteractionInfo;
        /// <summary>        /// ���߼��ľ���        /// </summary>
        public float interacteCheckDistance = 3f;

        private InteractionControl() { }

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
        }

        /// <summary> /// ����Ƿ��п��Խ����Ķ���  /// </summary>
        protected void FixedUpdate()
        {
            RaycastHit hit;
#if UNITY_EDITOR
            Debug.DrawRay(PlayerControl.transform.position, PlayerControl.GetLookatDir() * 3);
#endif
            if (Physics.Raycast(playerControl.transform.position, playerControl.GetLookatDir() * 3, out hit, interacteCheckDistance))
            {
                InteractionBase hitInfo = hit.transform.GetComponent<InteractionBase>();
                if (hitInfo != null && !hit.collider.isTrigger)
                {
                    nowInteractionInfo = hitInfo;
                }
                else
                    nowInteractionInfo = null;
            }
            else
                nowInteractionInfo = null;
            //if(nowInteractionInfo == null)
            //    UI.InteractionUI.Instance.CloseInteracte();
            //else
            //    UI.InteractionUI.Instance.ShowInteracte();
        }

        /// <summary>        /// ���н����¼�        /// </summary>
        /// <param name="interactionInfo">�����Ľ����¼�</param>
        public void RunInteraction(InteractionBase interactionInfo)
        {
            Debug.Log("����");
            if (interactionInfo == null) { 
                Debug.Log("�����������");
            }
            //���н�����Ϊ
            interactionInfo.InteractionBehavior();
        }

        /// <summary>
        /// ���е�ǰ���ڽ����Ľ����¼�
        /// </summary>
        public void RunInteraction()
        {
            if (nowInteractionInfo == null) return;
            RunInteraction(nowInteractionInfo);
        }
    }
}