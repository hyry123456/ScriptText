using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Motor
{
    public abstract class CameraBase : MonoBehaviour
    {
        private static CameraBase instance;
        public static CameraBase Instance => instance;

        /// <summary>   /// ֹͣ��������    /// </summary>
        public abstract void StopFollow();
        /// <summary>   /// ��ʼ��������    /// </summary>
        public abstract void BeginFollow();

        /// <summary>   /// ������������������Ļ����   /// </summary>
        /// <param name="offset">ƫ�Ƶķ���</param>
        public abstract void AdjustPosition(Vector3 offset);
        /// <summary>   /// �ص���ʼλ��    /// </summary>
        public abstract void BackToBegin();

        //����ע��
        protected virtual void Awake()
        {
            instance = this;
        }

        //ȡ��ע��
        protected virtual void OnDestroy()
        {
            instance = null;
        }



    }
}