using UnityEngine;


namespace Common
{
    /// <summary>
    /// 切换场景中，进行数据加载用的类
    /// </summary>
    public class DataLoad : MonoBehaviour
    {
        private static DataLoad instance;
        public static DataLoad Instance
        {
            get
            {
                return instance;
            }
        }

        private const string allTaskName = "AllTask";
        private const string completeTaskName = "CompleteTask";
        private const string runtimeTaskName = "RuntimeTask";
        private const string allPackagesName = "AllPackages";
        private const string obtainPackagesName = "ObtainPackages";

        [SerializeField]
        private FileLoadAsset asset;

        private static FileLoadAsset loadAsset;
        /// <summary>
        /// 文件保存时用到的映射表，该映射表可能为空，
        /// 因为只有正确读取数据时该值才会被赋予
        /// </summary>
        public static FileLoadAsset LoadAsset=>loadAsset;


        private bool isLoadComplete;
        public bool IsLoadComplete => isLoadComplete;

        private void Awake()
        {
            instance = this;
            if (loadAsset == null)
                loadAsset = asset;
            isLoadComplete = false;
            AsyncLoad.Instance.AddAction(LoadList);     //加载任务
        }

        private void OnDestroy()
        {
            instance = null;
        }

        /// <summary>
        /// 加载列表
        /// </summary>
        private void LoadList()
        {
            AsynLoadTask();
            AsynLoadPackage();

            isLoadComplete = true;      //表示加载完毕
        }

        /// <summary> /// 多线程加载任务  /// </summary>
        private void AsynLoadTask()
        {
            Task.TaskLoadData loadData = new Task.TaskLoadData();
            FileLoadMap loadMap = FileLoadMap.Instance(loadAsset);
            loadData.allChapter = loadMap.LoadFile(allTaskName);
            loadData.runtimeChapter = loadMap.LoadFileToPair(runtimeTaskName);
            loadData.completeChapter = loadMap.LoadFile(completeTaskName);
            Task.AsynTaskControl.Instance.LoadTaskData(loadData);
        }

        /// <summary>  /// 多线程加载背包 /// </summary>
        private void AsynLoadPackage()
        {
            Package.PackageLoadData loadData = new Package.PackageLoadData();
            FileLoadMap loadMap = FileLoadMap.Instance(loadAsset);
            loadData.packageClassNames = loadMap.LoadFile(allPackagesName);
            loadData.obtainItemAndCounts = loadMap.LoadFileToPair(obtainPackagesName);
            Package.PackageControl.Instance.LoadPackageData(loadData);
        }
    }
}