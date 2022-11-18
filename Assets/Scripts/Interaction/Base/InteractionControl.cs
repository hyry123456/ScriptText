
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField]
        GameObject interacteRemain;
        Text remainText;

        private bool isInteracting = false;
        //public bool IsInteracting
        //{
        //    get { return isInteracting; }
        //    set { isInteracting = value; }
        //}

        private InteractionControl() { }

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
            if (interacteRemain != null)
                remainText = interacteRemain.GetComponentInChildren<Text>();
        }

        /// <summary> /// ����Ƿ��п��Խ����Ķ���  /// </summary>
        protected void FixedUpdate()
        {
            RaycastHit hit;
#if UNITY_EDITOR
            Debug.DrawRay(PlayerControl.transform.position, PlayerControl.GetLookatDir() * interacteCheckDistance);
#endif
            if (Physics.Raycast(playerControl.transform.position, playerControl.GetLookatDir(), out hit, interacteCheckDistance))
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
            if(interacteRemain != null)
            {
                if(nowInteractionInfo == null || isInteracting)
                    interacteRemain.SetActive(false);
                else
                {
                    interacteRemain.SetActive(true);
                    interacteRemain.transform.position = nowInteractionInfo.
                        transform.position + Vector3.up * 0.5f;
                    remainText.text = nowInteractionInfo.GetInteracteRemain();
                }
            }
        }

        /// <summary>        /// ���н����¼�        /// </summary>
        /// <param name="interactionInfo">�����Ľ����¼�</param>
        private void RunInteraction(InteractionBase interactionInfo)
        {
            Debug.Log("����");
            if (interactionInfo == null) { 
                Debug.Log("�����������");
            }
            //���н�����Ϊ
            if (!isInteracting)
            {
                isInteracting = true;
                interactionInfo.InteractionBehavior(ReCall);
            }
        }

        private void ReCall()
        {
            isInteracting = false;
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