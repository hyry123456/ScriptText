using UnityEngine;

namespace UI
{
    /// <summary>
    /// �ɽ����������ʾUI
    /// </summary>
    public class InteractionUI : MonoBehaviour
    {
        private static InteractionUI instance;
        public static InteractionUI Instance => instance;

        /// <summary>    /// �ܿ��Ƶ����壬�����ر��Լ�������ʾ     /// </summary>
        [SerializeField]
        GameObject control;
        /// <summary>
        /// �����ж��Ƿ�Ҫ����
        /// </summary>
        bool isShow;

        void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void Start()
        {
            control.SetActive(false);
            isShow = false;
        }

        public void ShowInteracte()
        {
            isShow = true;
        }

        public void CloseInteracte()
        {
            isShow= false;
        }

        private void FixedUpdate()
        {
            Control.PlayerControlBase player = 
                Control.PlayerControlBase.Instance;
            if (player == null || !player.UseControl)
            {
                control.SetActive(false);
                return;
            }

            control.SetActive(isShow);
        }
    }
}