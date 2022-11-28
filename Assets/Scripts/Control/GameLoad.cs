using UnityEngine;

namespace Common
{
    /// <summary>    
    /// 加载类，用来调用全部的加载方法，为了保证加载，每一个类场景都放一个，
    /// 这个类只是调用类，具体是否重复加载是看具体类实现的
    /// </summary>
    public class GameLoad : MonoBehaviour
    {
        [SerializeField]
        AudioClip sceneBackgroundAudio;

        /// <summary> /// 是否需要开场变黑 /// </summary>
        public bool isNeedBeginBlack = true;
        ///// <summary>    /// 进行开场变黑的特效    /// </summary>
        //public DefferedRender.PostFXSetting fXSetting;


        private void Awake()
        {
            Application.targetFrameRate = -1;
            SceneObjectMap objectMap = SceneObjectMap.Instance;

            Task.AsynTaskControl.Instance.RuntimeTask();        //初始化任务

            ////开局黑屏
            //if (isNeedBeginBlack)
            //{
            //    nowRadio = 0;
            //    SustainCoroutine.Instance.AddCoroutine(ChangeColorFilter);
            //}

            //if (sceneBackgroundAudio == null)
            //    return;
            //if(sceneBackgroundAudio != null &&
            //    !Audio.AudioControl.Instance.CheckAudioIsPlaying(sceneBackgroundAudio))
            //{
            //    Audio.AudioControl.Instance.ChangeBackgroundAduio(sceneBackgroundAudio);
            //}
        }

        float nowRadio = 0;

        //bool ChangeColorFilter()
        //{
        //    nowRadio += Time.deltaTime * 0.5f;
        //    if(nowRadio >= 1.0f)
        //    {
        //        fXSetting.SetColorFilter(Color.white);
        //        return true;
        //    }
        //    fXSetting.SetColorFilter(Color.Lerp(Color.black, Color.white, nowRadio));
        //    return false;
        //}

    }
}