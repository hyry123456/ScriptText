using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    [CreateAssetMenu(menuName = "Common/FileLoadAsset")]
    /// <summary> /// �ļ���ȡʱ��Ҫ��ӳ���ļ� /// </summary>
    public class FileLoadAsset : ScriptableObject
    {
        [SerializeField]
        /// <summary>/// ���е��ļ����� /// </summary>
        private List<string> names;
        [SerializeField]
        /// <summary>/// ���е��ļ�·�� /// </summary>
        private List<string> paths;

        private Dictionary<string, string> fileMaps;

        public string FindPath(string names)
        {
            if(fileMaps == null)
            {
                CreateDictionary();
            }
            string path;
            if(fileMaps.TryGetValue(names, out path))
            {
                return path;
            }
            else
            {
                Debug.Log(names + " is null");
                return null;
            }
        }

        private void CreateDictionary()
        {
            fileMaps?.Clear();
            fileMaps = new Dictionary<string, string>();
            for(int i=0; i<names.Count; i++)
            {
                fileMaps.Add(names[i], paths[i]);
            }
        }

    }
}