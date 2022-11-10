using UnityEngine;

namespace Common
{
    /// <summary>    
    /// �����࣬��������ȫ���ļ��ط�����Ϊ�˱�֤���أ�ÿһ���ೡ������һ����
    /// �����ֻ�ǵ����࣬�����Ƿ��ظ������ǿ�������ʵ�ֵ�
    /// </summary>
    public class GameLoad : MonoBehaviour
    {
        private static GameLoad instance;
        public static GameLoad Instance => instance;
        private string sceneName;
        public string SceneName => sceneName;
        [SerializeField]
        AudioClip sceneBackgroundAudio;

        /// <summary> /// �Ƿ���Ҫ������� /// </summary>
        public bool isNeedBeginBlack = true;
        /// <summary>    /// ���п�����ڵ���Ч    /// </summary>
        public DefferedRender.PostFXSetting fXSetting;

        private void Awake()
        {
            instance = this;
            sceneName = Common.SceneControl.Instance.GetRuntimeSceneName();

            SustainCoroutine sustain = SustainCoroutine.Instance; //����Э��
            Application.targetFrameRate = -1;
            SceneObjectMap objectMap = SceneObjectMap.Instance;

            Task.AsynTaskControl.Instance.RuntimeTask();        //��ʼ������

            //���ֺ���
            if (isNeedBeginBlack)
            {
                nowRadio = 0;
                SustainCoroutine.Instance.AddCoroutine(ChangeColorFilter);
            }

            if (sceneBackgroundAudio == null)
                return;
            if(sceneBackgroundAudio != null &&
                !Audio.AudioControl.Instance.CheckAudioIsPlaying(sceneBackgroundAudio))
            {
                Audio.AudioControl.Instance.ChangeBackgroundAduio(sceneBackgroundAudio);
            }
        }

        float nowRadio = 0;

        bool ChangeColorFilter()
        {
            nowRadio += Time.deltaTime * 0.5f;
            if(nowRadio >= 1.0f)
            {
                fXSetting.SetColorFilter(Color.white);
                return true;
            }
            fXSetting.SetColorFilter(Color.Lerp(Color.black, Color.white, nowRadio));
            return false;
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}