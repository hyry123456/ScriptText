using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Info
{

    public class InfoMap
    {
        private static InfoMap instance;
        public static InfoMap Instance
        {
            get
            {
                if(instance == null)
                    instance = new InfoMap();
                return instance;
            }
        }

        Dictionary<string, string> infoMap;

        private InfoMap()
        {
        }

        /// <summary>
        /// ���ؽ�ɫ��Ϣ���洢��ʽΪ���������� �� ����ֵ
        /// </summary>
        /// <param name="infos">��Ϣ����</param>
        public void LoadInfoMap(List<KeyValuePair<string, string>> infos)
        {
            infoMap?.Clear();
            infoMap = new Dictionary<string, string>();
            if (infos == null) return;
            for(int i=0; i<infos.Count; i++)
            {
                infoMap.Add(infos[i].Key, infos[i].Value);
            }
        }

        /// <summary>   /// �ı�ĳ����Ϣ��ֵ    /// </summary>
        /// <param name="key">��Ϣ����</param>
        /// <param name="value">ֵ��С</param>
        public void ChangeInfo(string key, string value)
        {
            if (infoMap.ContainsKey(key))
            {
                infoMap[key] = value;
            }
            else
            {
                Debug.LogError(key + "������");
            }
        }

        /// <summary>  /// ����ַ������ݣ�Ҳ�����ڱ��в��Ҹ�����  /// </summary>
        /// <param name="key">��������</param>
        /// <returns>�����ݵ��ַ���</returns>
        public string GetStringData(string key)
        {
            if(infoMap == null)
            {
                Debug.Log("��δ���� " + key);
                return null;
            }
            string value;
            if(infoMap.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                Debug.LogError(key + " ������");
                return null;
            }
        }

        /// <summary>  /// ������ݣ���ת��Ϊ�����ʽ /// </summary>
        /// <param name="key">��������</param>
        /// <returns>�����ݵ��ַ���</returns>
        public float GetFloatData(string key)
        {
            if (infoMap == null)
            {
                Debug.Log("��δ���� " + key);
                return 0;
            }
            string value;
            if (infoMap.TryGetValue(key, out value))
            {
                float re;
                if(float.TryParse(value, out re))
                    return re;
                else
                {
                    Debug.LogError(value + " ����ת��");
                    return 0;
                }
            }
            else
            {
                Debug.LogError(key + " ������");
                return 0;
            }
        }

        /// <summary>  /// ������ݣ���ת��Ϊ���͸�ʽ /// </summary>
        /// <param name="key">��������</param>
        /// <returns>�����ݵ��ַ���</returns>
        public int GetIntData(string key)
        {
            if (infoMap == null)
            {
                Debug.Log("��δ���� " + key);
                return 0;
            }
            string value;
            if (infoMap.TryGetValue(key, out value))
            {
                int re;
                if (int.TryParse(value, out re))
                    return re;
                else
                {
                    Debug.LogError(value + " ����ת��");
                    return 0;
                }
            }
            else
            {
                Debug.LogError(key + " ������");
                return 0;
            }
        }

        /// <summary>     /// �õ���Ϣ�����ַ���      /// </summary>
        public string GetSaveString()
        {
            StringBuilder strs = new StringBuilder();
            foreach(var pair in infoMap)
            {
                strs.Append("<" + pair.Key + "=" + pair.Value +">\n");
            }
            return strs.ToString();
        }
    }
}