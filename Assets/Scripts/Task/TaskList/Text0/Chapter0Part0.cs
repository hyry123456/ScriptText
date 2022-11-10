using Interaction;
using UnityEngine;

namespace Task
{
    public class Chapter0Part0 : ChapterPart
    {
        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            base.EnterTaskEvent(chapter, isLoaded);
            Debug.Log("�½ڽ���");
            Common.SustainCoroutine.Instance.AddCoroutine(Wait);
        }

        float nowTime = 0;
        private bool Wait()
        {
            nowTime += Time.deltaTime;
            if (nowTime > 2)
            {
                AsynTaskControl.Instance.CheckChapter(chapter.ChapterID,
                new InteracteInfo
                {

                });
                return true;
            }
            return false;
        }


        public override void ExitTaskEvent(Chapter chapter)
        {
            Debug.Log("�½��˳�");
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            return true;
        }
    }
}