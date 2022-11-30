using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary> /// ѡ��UI/// </summary>
    public class OptionUI : MonoBehaviour
    {
        private static OptionUI instance;
        public static OptionUI Instance
        {
            get { return instance; }
        }

        [SerializeField]
        GameObject optionBarOrigin;
        Common.PoolingList<OptionBarPooling> optionBars;
        public Common.PoolingList<OptionBarPooling> OptionBars => optionBars;
        Common.ISetOneParam<int> recall;

        private void Awake()
        {
            instance = this;
            optionBars = new Common.PoolingList<OptionBarPooling>();
        }

        private void OnDestroy()
        {
            //�Ƴ����е�ѡ������������ʱѡ��Ͳ���Ҫ��
            while(optionBars.Count > 0)
            {
                optionBars.GetValue(0).CloseObject();
                optionBars.RemoveAt(0);
            }
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        void BarRecall(int index)
        {
            recall(index);
            optionBars.RemoveAll();
            gameObject.SetActive(false);
        }

        /// <summary>/// ����ѡ��/// </summary>
        /// <param name="options">ѡ������</param>
        /// <param name="reCall">�ص�����������˵��ѡ�����ĸ�</param>
        public void AddOptions(string[] options, Common.ISetOneParam<int> recall)
        {
            //�����ǰ�Լ���ֵ��ֱ���˳�
            if (optionBars.Count != 0)
            {
                Debug.LogError("�ظ�");
                return;
            }

            gameObject.SetActive(true);
            this.recall = recall;
            for (int i=0; i<options.Length; i++)
            {
                OptionBarPooling optionBar = Common.SceneObjectPool.
                    Instance.GetObject<OptionBarPooling>("OptionBar", 
                    optionBarOrigin, Vector3.zero, Quaternion.identity);
                optionBar.SetText(options[i], i, BarRecall);
                optionBars.Add(optionBar);
                optionBar.transform.parent = transform;
            }
        }

        public void RemoveNowOptions()
        {
            //�Ƴ����е�ѡ������������ʱѡ��Ͳ���Ҫ��
            while (optionBars.Count > 0)
            {
                optionBars.GetValue(0).CloseObject();
                optionBars.RemoveAt(0);
            }
        }
    }
}