using UnityEngine;


namespace Task
{
    public class Chapter0 : AsynChapterBase
    {
        public Chapter0()
        {
            chapterName = "�����½�";
            chapterID = 0;
            targetPart = targetPart + "Chapter0Part";
            chapterSavePath = Application.streamingAssetsPath + "/Task/Chapter/0.task";
            runtimeScene = "SimpleText";
        }

        //ֱ�Ӽ���
        public override void CheckAndLoadChapter()
        {
            AsynTaskControl.Instance.AddChapter(this);
        }

        public override void CompleteChapter(bool isInThisScene)
        {
            Debug.Log("�½����");
        }

        public override void ExitChapter()
        {
        }
    }
}