using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common
{
    /// <summary>
    /// ���������࣬�������Ƴ����л���Ҳ�����л������س�����
    /// Ȼ������ı��е����ݽ��г�������
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
        /// �����ж����л��������µ����¼��ػ��Ǳ��������¼��أ�ֻ�г�������ʱ�Ż���øú���
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

        /// <summary>        /// ������������еĳ���������        /// </summary>
        public string GetRuntimeSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        /// <summary>        /// ���¼��ص�ǰ���еĳ���        /// </summary>
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