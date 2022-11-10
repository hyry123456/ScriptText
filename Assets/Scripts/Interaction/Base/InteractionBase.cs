using System.Collections;
using UnityEngine;

namespace Interaction
{
    /// <summary>    /// ������Ϣ�࣬�����洢��������    /// </summary>
    public struct InteracteInfo
    {
        public string data;
        public int id;
    }

    /// <summary>    /// ��������࣬���������ϣ�����ִ�м򵥵Ľ���    /// </summary>
    public abstract class InteractionBase : MonoBehaviour
    {
        [HideInInspector]
        public InteractionType interactionType;
        /// <summary>        /// ��Ҫע�⣬����IDһ�㲻�������ر�ָ����������һЩ��������½�����ʱ��ֵ�õ�
        /// Ҳ����˵��ʼ������Ҫ��ֵ������/// </summary>
        public int interactionID = 0;
        /// <summary>        /// ��ʼ�������ƣ�˳����Ϊ��������ʾ��Ϣ        /// </summary>
        public string interactionName;

        /// <summary>
        /// ��ʼ��InteractionType����interactionName
        /// </summary>
        protected abstract void OnEnable();

        /// <summary>        /// �ý�����Ϊ��Ҫ�ɵ�����        /// </summary>
        public abstract void InteractionBehavior();

    }
}