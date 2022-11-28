using UnityEngine;

namespace Common
{
    /// <summary> /// ���ݱ��棬ʹ�ö��̶߳�ȡ����д���ļ��� /// </summary>
    public class DataSave
    {
        private static DataSave instance;
        //private const string completeTaskName = "CompleteTask";
        //private const string runtimeTaskName = "RuntimeTask";
        //private const string obtainPackagesName = "ObtainPackages";

        public static DataSave Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new DataSave();
                }
                return instance;
            }
        }

        private bool isSaveComplete;
        public bool IsSaveComplete => isSaveComplete;
        private DataSave()
        {
        }

        /// <summary> /// �������ݵĺ��нӿ� /// </summary>
        public void SaveData()
        {
            isSaveComplete = false;
            AsyncLoad.Instance.AddAction(AsynSaveData);
        }

        //���̱߳�������
        private void AsynSaveData()
        {
            if (DataLoad.LoadAsset == null)
                return;
            SaveTaskData();
            SaveInfoData();
            isSaveComplete =true;
        }

        private void SaveTaskData()
        {
            string runtimeTask = Task.AsynTaskControl.Instance.GetRuntimeTaskData();
            string completeTask = Task.AsynTaskControl.Instance.GetCompleteTaskData();
            FileLoadAsset loadAsset = DataLoad.LoadAsset;
            string prefit = Application.streamingAssetsPath;
            FileReadAndWrite.WriteFile(prefit + loadAsset.FindPath(DataLoad.runtimeTaskName),
                runtimeTask);
            FileReadAndWrite.WriteFile(prefit + loadAsset.FindPath(DataLoad.completeTaskName),
                completeTask);
            Debug.Log("�������");
        }

        private void SaveInfoData()
        {
            string prefit = Application.streamingAssetsPath;
            string infoData = Info.InfoMap.Instance.GetSaveString();
            FileLoadAsset loadAsset = DataLoad.LoadAsset;
            FileReadAndWrite.WriteFile(prefit + 
                loadAsset.FindPath(DataLoad.playerInfo), infoData);
        }

        public static void ClearData()
        {
            FileLoadAsset loadAsset = Resources.Load<FileLoadAsset>("Common/FileLoadAsset");
            string prefit = Application.streamingAssetsPath;
            //������������
            FileReadAndWrite.WriteFile(prefit + 
                loadAsset.FindPath(DataLoad.runtimeTaskName), "");
            FileReadAndWrite.WriteFile(prefit + 
                loadAsset.FindPath(DataLoad.completeTaskName), "");
            //�����������
            FileReadAndWrite.WriteFile(prefit +
                loadAsset.FindPath(DataLoad.obtainPackagesName), "");
        }



    }
}