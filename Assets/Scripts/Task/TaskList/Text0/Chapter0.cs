using UnityEngine;


namespace Task
{
    public class Chapter0 : AsynChapterBase
    {
        public Chapter0()
        {
            chapterName = "测试章节";
            taskPartCount = 1;
            chapterID = 0;
            targetPart = targetPart + "Chapter0Part";
            runtimeScene = "SimpleText";
        }

        //直接加载
        public override void CheckAndLoadChapter()
        {
            AsynTaskControl.Instance.AddChapter(this);
        }

        public override void CompleteChapter(bool isInThisScene)
        {
        }

        public override void ExitChapter()
        {
            Debug.Log("章节完成");
        }
    }
}