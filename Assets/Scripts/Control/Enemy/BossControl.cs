﻿using System.Collections.Generic;
using UnityEngine;
using Skill;

namespace Control
{
    /// <summary> /// Boss的AI控制  /// </summary>
    public class BossControl : MonoBehaviour
    {
        Transform player;
        Motor.EnemyMotor motor;
        Info.EnemyInfo enemyInfo;
        SkillManage skillManage;
        AnimateManage animate;
        /// <summary>     
        /// boss的行为，用来判断是移动，旋转还是攻击，这些是基本行为
        /// </summary>
        int actionState;

        /// <summary>   /// boss状态，用来执行不同的攻击方式   /// </summary>
        int bossState;

        private void Start()
        {
            player = PlayerControlBase.Instance.transform;
            motor = GetComponent<Motor.EnemyMotor>();
            enemyInfo = GetComponent<Info.EnemyInfo>();
            skillManage = GetComponent<SkillManage>();
            animate = GetComponent<AnimateManage>();
            bossState = 0;
        }

        //Boss具体的攻击方式由技能系统决定，由于Boss位置的特殊性，暂时不给Boss加寻路
        //Boss的技能由技能管理类直接进行释放，并且Boss会简单的闪避，但是闪避技能时间有限，不能经常释放
        //Boss有多种攻击手段，且技能一旦是否后就由技能本身控制，也就是Boss只是一个释放器，具体技能行为由技能自己控制
        //同时Boss本身有阶段的区分，具体可以之后用一个switch来进行变化
        private void FixedUpdate()
        {




            switch (bossState)
            {
                case 0:
                    OnFirstState();
                    break;
            }
        }

        /// <summary>   
        /// 第一阶段，Boss只会选择简单的攻击，且Boss的攻击停止时间较长
        /// </summary>
        private void OnFirstState()
        {
            List<SkillBase> skills = skillManage.GetCanUseSkillsByType(SkillType.LongDisAttack);
            if (skills == null) return;
            for(int i=0; i<skills.Count; i++)
            {
                if (skillManage.CheckAndRelase(skills[i]))
                    break;
            }
        }

        private void SwitchActionState()
        {
            //switch (actionState)
            //{
            //    case 0:
            //        Vector3 enemyDir = transform.forward;
            //        Vector3 playerDir = (player.position - transform.position).normalized;
            //        motor.Move(playerDir.z, playerDir.x);
            //        animate.PlayAnimate(AnimateType.Move);
            //}
        }
    }
}