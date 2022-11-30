
namespace Skill
{
    [System.Serializable]
    public abstract class SkillBase : ISkill
    {
        public abstract bool OnSkillRelease(SkillManage mana);
        /// <summary>    /// �����������,������ʾ����     /// </summary>
        public int maxSkillCount;
        /// <summary>        /// ���ܵ�ǰ����       /// </summary>
        public int skillCount;
        /// <summary>        /// ��ǰ��ȴʱ�䣬���������ܿ������жϼ����ܲ����ͷ�        /// </summary>
        public float preReleaseTime;
        /// <summary>        /// ������ȴʱ�䣬��ȴʱ��û�н���������ֹͣ����        /// </summary>
        public float coolTime;
        /// <summary>       /// ���ܵ��ͷ�ʱ�䣬�������Ƽ����ͷ�       /// </summary>
        public float releaseTime;
        /// <summary>        /// ��������        /// </summary>
        public string skillName;
        /// <summary>        /// �������ͣ���������        /// </summary>
        public SkillType skillType;

        /// <summary>
        /// ��ʼ������
        /// </summary>
        //public SkillBase()
        //{
        //    maxSkillCount = 10;
        //    nowCoolTime = 0;
        //    skillCount = maxSkillCount;
        //    releaseTime = 0;
        //    coolTime = 0;
        //    skillName = "Skill Name;
        //    skillType = SkillType.TYPE;
        //}
    }
}