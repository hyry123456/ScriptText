
namespace Skill {
    public interface ISkill
    {
        /// <summary>        /// �����ͷŷ���        /// </summary>
        /// <param name="mana">����ӵ����</param>
        bool OnSkillRelease(SkillManage mana);
    }
}