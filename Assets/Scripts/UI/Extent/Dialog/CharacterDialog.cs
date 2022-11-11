using UnityEngine;
using Common;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// ��ɫ�Ի��Ҳ������ʾ�ڽ�ɫͷ�ϵĶԻ���
    /// </summary>
    public class CharacterDialog : ObjectPoolBase
    {
        //�ص�����
        private INonReturnAndNonParam recall;

        Queue<string> readyStrings;
        /// <summary>  /// �洢�����ı��õĽṹ   /// </summary>
        protected StringBuilder sb;
        /// <summary>  /// ��ǰ�ı��е�������ɫ  /// </summary>
        Color changeColor;
        /// <summary>  /// ��ǰ͸���е��ı�   /// </summary>
        protected string alphaChar;
        /// <summary> /// ����ӵ��ַ��ڵ�ǰ��ʾ���ı��ı��   /// </summary>
        protected int nowIndex;
        /// <summary>   /// ÿһ���ַ���ʾʱ��Ҫ��ʱ��   /// </summary>
        float perCharWaitTime = 0.05f;
        /// <summary>  /// ��ǰ��ʾ���ַ�   /// </summary>
        protected StringBuilder nowShowString;

        Text textUI;

        private float nowWaitTime = 0;

        protected override void OnEnable()
        {
            recall = null;
        }


        public void BeginDialog(string dialogs, INonReturnAndNonParam recall)
        {
            this.recall = recall;
            string[] strs = dialogs.Split('\n');
            nowIndex = 0;
            readyStrings = new Queue<string>();
            for (int i = 0; i < strs.Length; i++)
            {
                readyStrings.Enqueue(strs[i]);
            }
            alphaChar = null;

            sb = new StringBuilder(readyStrings.Dequeue());
            nowShowString = new StringBuilder();
            nowIndex = 0;
            textUI = GetComponentInChildren<Text>();

            SustainCoroutine.Instance.AddCoroutine(CircleShowDialog, false);
        }

        private bool CircleShowDialog()
        {
            if (sb == null)     //��ǰ��ʾ�ı�������
            {
                //Ϊ�������ֿ��ܣ�һ���ǻ��ڵȴ�������
                if (readyStrings != null && readyStrings.Count > 0)
                {
                    if (WaitEnd())
                    {
                        sb = new StringBuilder(readyStrings.Dequeue());
                        nowShowString = new StringBuilder("");
                        nowIndex = 0;           //��ʼ����ȡ����֧��
                        nowWaitTime = 0;        //����ȴ�ʱ��
                    }
                    else return false;  //������ʾ���У������иı�
                }
                //һ���ǽ����ˣ���Ҫ����
                else
                {
                    if (WaitEnd())
                    {
                        CloseObject();
                        return true;    //����ֹͣЭ����
                    }
                    return false;
                }
            }

            if (alphaChar == null)
            {
                changeColor = textUI.color; changeColor.a = 0;
                if (nowIndex >= sb.Length)      //������ʾ�����ˣ��Ƴ�����
                {
                    sb = null;
                    return false;
                }
                alphaChar = sb[nowIndex].ToString();    //��ʾ��һ���ַ�
                nowIndex++;
            }

            changeColor.a += Time.deltaTime * (1.0f / perCharWaitTime);
            if (changeColor.a >= 1)     //͸����ʾ��������֧�����뵽����ʾ�õ��ַ���
            {
                nowShowString.Append(alphaChar);
                alphaChar = null;
                textUI.text = nowShowString.ToString();
            }
            else
            {
                textUI.text = nowShowString + "<color=\"#" + ColorUtility.ToHtmlStringRGBA(changeColor)
                    + "\">" + alphaChar + "</color>";
            }
            return false;
        }


        private bool WaitEnd()
        {
            nowWaitTime += Time.deltaTime;
            if (nowWaitTime > nowShowString.Length * perCharWaitTime * 10)
                return true;
            return false;
        }

        public override void CloseObject()
        {
            base.CloseObject();
            if (recall != null) recall();
            textUI.text = "";       //�����������
        }
    }
}