using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common
{
    /// <summary>
    /// 场景控制类，用来控制场景切换，也就是切换到加载场景，
    /// 然后根据文本中的数据进行场景控制
    /// </summary>
    public class SceneControl : MonoBehaviour
    {
        private static SceneControl instance;
        public static SceneControl Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject gameObject = new GameObject("SceneChange");
                    gameObject.AddComponent<SceneControl>();
                }
                return instance;
            }
        }

        private string targetScene;
        AsyncOperation asyncStatic;

        bool isSceneChange = false;
        /// <summary>
        /// 用来判断是切换场景导致的重新加载还是本身场景重新加载，只有场景加载时才会调用该函数
        /// </summary>
        public bool IsSceneChange => isSceneChange;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ChangeScene(string targetSceneName)
        {
            isSceneChange = true;
            targetScene = targetSceneName;
            SceneManager.LoadScene("ChangeScene");
            waitTime = 0;
            SustainCoroutine.Instance.AddCoroutine(WaitLoad);
            //SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
            //StartCoroutine(AsynLoadScene());
        }

        public void ChangeSceneDirect(string targetSceneName)
        {
            isSceneChange = true;
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        }

        float waitTime = 0;

        bool WaitLoad()
        {
            waitTime += Time.deltaTime;
            if(waitTime >= 0.3f)
            {
                asyncStatic = SceneManager.LoadSceneAsync(targetScene);
                asyncStatic.allowSceneActivation = false;
                Common.SustainCoroutine.Instance.AddCoroutine(AsyLoadScene);
                return true;
            }

            return false;
        }

        bool AsyLoadScene()
        {
            if(asyncStatic.progress < 0.9f)
            {
                Debug.Log(asyncStatic.progress);
                return false;
            }
            SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
            return true;
        }

        //IEnumerator AsynLoadScene()
        //{
        //    //
        //}

        public float GetLoadProgress()
        {
            return asyncStatic.progress;
        }

        /// <summary>        /// 获得正在运行中的场景的名称        /// </summary>
        public string GetRuntimeSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        /// <summary>        /// 重新加载当前运行的场景        /// </summary>
        public void ReloadActiveScene()
        {
            isSceneChange = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GameExit()
        {
            Application.Quit();
        }
    }
}