﻿
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public class SkillManage : MonoBehaviour
    {
        [SerializeField]
        /// <summary>        /// 技能释放者的技能列表        /// </summary>
        protected List<SkillBase> skills;
        /// <summary>        /// 公开技能释放者的所有技能，但是不允许修改        /// </summary>
        public List<SkillBase> Skills
        {
            get
            {
                return skills;
            }
        }

        float endTime = 0;
        public float EndTime => endTime;
        public void SetEndTime(float endTime)
        {
            this.endTime = endTime;
        }
        /// <summary>   /// 用来判断技能是否释放中  /// </summary>
        public bool IsReleasing
        {
            get
            {
                if (endTime > Time.time)
                    return true;
                return false;
            }
        }

        private Info.CharacterInfo characterInfo;
        /// <summary>        /// 技能释放者的技能信息基类        /// </summary>
        public Info.CharacterInfo CharacterInfo
        {
            get
            {
                if(characterInfo == null)
                    characterInfo = GetComponentInChildren<Info.CharacterInfo>();
                return characterInfo;
            }
        }

        private AnimateManage animateManage;
        /// <summary>     /// 动画控制器     /// </summary>
        public AnimateManage AnimateManage
        {
            get
            {
                return animateManage;
            }
        }

        protected virtual void Start()
        {
            characterInfo = GetComponent<Info.CharacterInfo>();
            animateManage = GetComponent<AnimateManage>();
        }

        /// <summary>   /// 检查并释放该技能    /// <summary>   
        /// <param name="skill">技能对象</param>
        /// <returns>是否可以释放</returns>
        public bool CheckAndRelase(SkillBase skill)
        {
            if (skill == null) return false;
            //为0就是没有了，为负数表示无限
            if(skill.skillCount == 0) return false;
            if(Time.time - skill.preReleaseTime > skill.coolTime)
            {
                skill.preReleaseTime = Time.time;
                if(skill.OnSkillRelease(this))
                {
                    //有可能在运行中调整结束时间，所以取最大值
                    endTime = Mathf.Max(Time.time + skill.releaseTime, endTime);
                    skill.skillCount--;
                    return true;
                }
            }
            return false;
        }

        /// <summary>       /// 获得技能列表中的指定技能        /// </summary>
        /// <param name="index">技能编号</param>
        /// <returns>技能对象</returns>
        public SkillBase GetSkillByIndex(int index)
        {
            if(index > 0 && skills != null && skills.Count > index)
            {
                SkillBase skill = skills[index];
                return skill;
            }
            return null;
        }

        /// <summary>        /// 获得所有可以使用的技能        /// </summary>
        /// <returns>技能列表</returns>
        public List<SkillBase> GetCanUseSkill()
        {
            if (skills == null) return null;
            List<SkillBase> canUse = new List<SkillBase>(skills.Count);
            for(int i=0; i<skills.Count; i++)
            {
                if(skills[i].preReleaseTime <= 0)
                {
                    canUse.Add(skills[i]);
                }
            }
            return canUse;
        }

        /// <summary>
        /// 获得可以使用的技能，且技能类型与传入类型对应，需要注意的是支持多匹配，
        /// 也就是通过按位与可以匹配多种技能类型
        /// </summary>
        /// <param name="type">技能类型</param>
        /// <returns>可以使用的技能列表，注意为空的情况</returns>
        public List<SkillBase> GetCanUseSkillsByType(SkillType type)
        {
            if (skills == null) return null;
            List<SkillBase> canUse = new List<SkillBase>();
            for(int i=0; i<skills.Count; i++)
            {
                if((Time.time - skills[i].preReleaseTime) > skills[i].coolTime
                    && (skills[i].skillType & type) != 0)
                {
                    canUse.Add(skills[i]);
                }
            }
            return canUse;
        }

        /// <summary>        /// 通过标签随机数获得一个技能        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public SkillBase GetRandomSkillByType(SkillType type)
        {
            List<SkillBase> skills = GetCanUseSkillsByType(type);
            if(skills == null || skills.Count == 0) return null;
            return skills[Random.Range(0, skills.Count)];
        }

        /// <summary> /// 添加技能，根据名称进行技能剔除，避免重复添加   /// </summary>
        public void AddSkill(SkillBase skill)
        {
            //静态状态技能，在加入后会直接退出，不被管理，但需要被注册
            if (skill.skillType == SkillType.StaticState)
            {
                skill.OnSkillRelease(this);
                return;
            }

            if(skills == null)
            {
                skills = new List<SkillBase>();
                skills.Add(skill);
                return;
            }
            for(int i=0; i<skills.Count; i++)
            {
                if (skills[i].skillName == skill.skillName)
                    return;
            }
            skills.Add(skill);
        }
    }
}