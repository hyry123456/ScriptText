using UnityEngine;
using System;
using System.Collections.Generic;

namespace Package
{
    /// <summary>/// ������ļ�����Ҫ�����ݽṹ��/// </summary>
    public struct PackageLoadData
    {
        /// <summary>   /// ���еı�����������     /// </summary>
        public List<string> packageClassNames;
        /// <summary>   /// ���л�ȡ������Լ�����   /// </summary>
        public List<KeyValuePair<string, string>> obtainItemAndCounts;
    }

    /// <summary>
    /// ���������࣬���еı������ݶ���������п���
    /// </summary>
    public class PackageControl
    {
        private static PackageControl instance;
        public static PackageControl Instance
        {
            get
            {
                if(instance == null)
                    instance = new PackageControl();
                return instance;
            }
        }
        private PackageControl()
        {

        }
        /// <summary>
        /// ���б�����ӳ���<����������>����ʱ������������
        /// </summary>
        private Dictionary<string, int> packagesMap;

        public void LoadPackageData(PackageLoadData packageData)
        {
            SetAllPackage(packageData.packageClassNames);
            SetItemCount(packageData.obtainItemAndCounts);
        }

        /// <summary> /// ���ñ������ݣ�Ҳ���Ǹ�����ӳ�����г�ʼ��/// </summary>
        /// <param name="packageClassNames">���б���������������</param>
        private void SetAllPackage(List<string> packageClassNames)
        {
            packagesMap?.Clear();
            if (packageClassNames == null) return;
            packagesMap = new Dictionary<string, int>(packageClassNames.Count);
            for(int i=0; i < packageClassNames.Count; i++)
            {
                packagesMap.Add(packageClassNames[i], 0);
            }
        }

        /// <summary>  /// ����ÿһ���Ѿ���ȡ�����������    /// </summary>
        /// <param name="itemsData">���л�ȡ�����������</param>
        private void SetItemCount(List<KeyValuePair<string, string>> itemsData)
        {
            if (itemsData == null) return;

            foreach (var item in itemsData)
            {
                packagesMap[item.Key] = int.Parse(item.Value);
            }
        }

    }
}