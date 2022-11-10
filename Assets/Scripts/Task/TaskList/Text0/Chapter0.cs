using UnityEngine;


namespace Task
{
    public class Chapter0 : AsynChapterBase
    {
        public Chapter0()
        {
            chapterName = "�����½�";
            taskPartCount = 1;
            chapterID = 0;
            targetPart = targetPart + "Chapter0Part";
            runtimeScene = "SimpleText";
        }

        //ֱ�Ӽ���
        public override void CheckAndLoadChapter()
        {
            AsynTaskControl.Instance.AddChapter(this);
        }

        public override void CompleteChapter(bool isInThisScene)
        {
        }

        public override void ExitChapter()
        {
            Debug.Log("�½����");
        }
    }
}