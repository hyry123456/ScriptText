
using UnityEngine;

namespace ScriptAnimate
{
    [System.Serializable]
    public abstract class ScriptAnimateBase : ScriptableObject
    {
        /// <summary>
        /// �������Ž���ʱ���е���Ϊ�������ж��Ƿ���Ҫ�л�����һ��Ϊ
        /// </summary>
        /// <param name="animateControl">����������������ȡ��Ϸ����</param>
        /// <returns>�Ƿ��˳����Ǿͻ������һ��Ϊ�����������һ֡�ٵ���һ��</returns>
        public abstract bool EndAnimate(ScriptAnimateControl animateControl);

        /// <summary>      /// �������ſ�ʼʱִ�е���Ϊ������������Ϊ�ĳ�ʼ��      /// </summary>
        /// <param name="animateControl">����������</param>
        public abstract void BeginAnimate(ScriptAnimateControl animateControl);
    }
}