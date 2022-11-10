using UnityEngine;
using Common;
using System.Reflection;
using System.Collections.Generic;

namespace Info
{

     [System.Serializable]
    public abstract class CharacterInfo : MonoBehaviour
    {
        [SerializeField]
        /// <summary>  /// 默认技能，可以不赋予值   /// </summary>
        private string[] defaultSkills;


        public string characterName;
        [SerializeField]
        protected int hp = 10;
        public int maxHP = 10;
        /// <summary>        /// 跑步速度        /// </summary>
        public float runSpeed = 10;
        /// <summary>        /// 行走速度        /// </summary>
        public float walkSpeed = 5;
        public float rotateSpeed = 10;
        /// <summary>   /// 该人物可攻击到的敌人层，以及可遮挡层     /// </summary>
        public LayerMask attackLayer;
        [SerializeField]
        private int maxSolidRadio = 10;
        [SerializeField]
        private int solidRadio;

        /// <summary>   /// 敌人最大坚硬值    /// </summary>
        public int MaxSolidRadio => maxSolidRadio;
        public int SolidRadio => solidRadio;
        public void SetSolidRadio(int solid)
        {
            solidRadio = solid;
        }

        /// <summary>        /// 判断角色是否死亡        /// </summary>
        public bool isDie => hp <= 0;

        protected PoolingList<CharacterState> states = new PoolingList<CharacterState>();

        private ExternalAction externalActions;


        /// <summary>        /// 初始化方法        /// </summary>

        protected virtual void OnEnable()
        {
            hp = maxHP;
            solidRadio = maxSolidRadio;
            if (defaultSkills == null)
                return;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Skill.SkillManage skillManage = GetComponent<Skill.SkillManage>();
            externalActions = GetComponent<ExternalAction>();
            if (externalActions == null)
                externalActions = gameObject.AddComponent<ExternalAction>();
            if (skillManage == null) return;
            string prefit = "Skill.";
            for (int i = 0; i < defaultSkills.Length; i++)
            {
                Skill.SkillBase skillBase = (Skill.SkillBase)
                    assembly.CreateInstance(prefit + defaultSkills[i]);
                skillManage.AddSkill(skillBase);
            }
        }

        /// <summary>        /// 判断是否死亡        /// </summary>
        protected void CheckDead()
        {
            if(isDie)
                DealWithDeath();
        }


        /// <summary>        /// 死亡后的操作        /// </summary>
        protected abstract void DealWithDeath();
        
        protected virtual void Update()
        {
            //接收外部影响
            PoolingList<ActionData> actions = externalActions.GetActions;
            int hpChange = 0, solidChange = 0;
            Stack<ActionRecall> recalls = 
                new Stack<ActionRecall>(actions.Count);

            while (actions.Count > 0)       //计算所有状态，获得最终操作的HP值
            {
                ActionData action = actions.GetValue(0);
                hpChange += action.hp; solidChange += action.solide;
                actions.Remove(0);
                if (action.recall != null)
                    recalls.Push(action.recall);
                if (action.state != null)
                    states.Add(action.state);
            }
            //执行HP操作
            for (int i = 0; i < states.Count; i++)
            {
                hpChange = states.GetValue(i).OnHPChange(hpChange, this);
            }
            //检查死亡
            CheckDead();
            for (int i = 0; i < states.Count; i++)
            {
                solidChange = states.GetValue(i).OnSolidChange(solidChange, this);
            }

            for(int i=0; i<recalls.Count; i++)
            {
                ActionRecall recall = recalls.Pop();
                recall(this, hpChange, solidChange);
            }

            hp += hpChange;
            solidRadio += solidChange;

            //更新状态，此时两值可能为负，方便标识为第一次死亡或者破防
            for (int i = states.Count - 1; i >= 0; i--)
            {
                if (states.GetValue(i).OnUpdate(this, hpChange, solidChange))
                    states.Remove(i);
            }

            hp = Mathf.Max(hp, 0);
            solidRadio = Mathf.Max(solidRadio, 0);
        }


    }
}