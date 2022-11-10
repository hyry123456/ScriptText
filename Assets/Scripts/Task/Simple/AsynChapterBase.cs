using System.Reflection;

namespace Task
{
    /// <summary>
    /// 多线程任务加载类案例，也是用来继承的类
    /// </summary>
    public abstract class AsynChapterBase : Chapter
    {
        protected string targetPart = "Task.";

        //初始化案例
        //public AsynChapterBase()
        //{
        //    chapterName = "AsynChapterBase";
        //    chapterTitle = "一个测试章节";
        //    taskPartSize = 2;
        //    chapterID = 0;
        //    //任务文件用编号命名
        //    chapterSavePath = Application.streamingAssetsPath + "/" + "Task/Chapter/0.task";
        //    targetPart = targetPart +"赋值子章节名称";
        //    runtimeScene = "simpleScene";
        //}


        public override void ChangeTask()
        {
            currentPartIndex++;
            part.ExitTaskEvent(this);       //退出当前任务

            if (currentPartIndex == taskPartCount)     //章节完成
            {
                //保存完成的任务
                AsynTaskControl.Instance.CompleteChapter(this);
                return;
            }
            //未完成就搜索子章节
            string targetPartStr = targetPart + currentPartIndex.ToString();
            Assembly assembly = Assembly.GetExecutingAssembly();
            part = (ChapterPart)assembly.CreateInstance(targetPartStr);
            part.EnterTaskEvent(this, false);
        }

        public override void CheckTask(Interaction.InteracteInfo info)
        {
            if (part.IsCompleteTask(this, info))
            {
                ChangeTask();
            }
        }

        /// <summary>        /// 开始章节前先初始化        /// </summary>
        public override void BeginChapter()
        {
            string targetPartStr = targetPart + '0';
            Assembly assembly = Assembly.GetExecutingAssembly();
            part = (ChapterPart)assembly.CreateInstance(targetPartStr);
            part.EnterTaskEvent(this, false);
            currentPartIndex = 0;
        }

        /// <summary>
        /// 执行加载过的任务，也就是根据当前编号映射，
        /// 且传入的已加载过的值为true
        /// </summary>
        public override void ExecuteNowPart()
        {
            string targetPartStr = targetPart + currentPartIndex.ToString();
            Assembly assembly = Assembly.GetExecutingAssembly();
            part = (ChapterPart)assembly.CreateInstance(targetPartStr);
            part.EnterTaskEvent(this, true);
        }
    }
}