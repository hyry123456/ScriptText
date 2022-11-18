using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{

    public class PlayerUnderAttack : SkillBase, Info.CharacterState
    {
        /// <summary> /// ���漼���ͷŶ���    /// </summary>
        SkillManage mana;
        AnimateManage animateManage;
        Motor.MotorBase motor;

        public PlayerUnderAttack()
        {
            skillType = SkillType.StaticState;
            nowTime = waitTime + upTime + 1;        //��ʼ��ʱ��
            releaseTime = 0.5f;
            currentWaveTime = maxWaveTime + 1;
        }

        public override bool OnSkillRelease(SkillManage mana)
        {
            this.mana = mana;
            Info.ExternalAction external = mana.GetComponent<Info.ExternalAction>();
            external.AddAction(new Info.ActionData
            {
                state = this
            });
            animateManage = mana.gameObject.GetComponent<AnimateManage>();
            motor = mana.gameObject.GetComponent<Motor.MotorBase>();
            return true;
        }

        //���ı�hp
        public int OnHPChange(int hp, Info.CharacterInfo info)
        {
            return hp;
        }

        public int OnSolidChange(int solid, Info.CharacterInfo info)
        {
            this.info = info;
            return solid;
        }

        public bool OnUpdate(Info.CharacterInfo info, int finalHp, int finalSolid)
        {
            if (animateManage == null)
                return true;

            if(finalHp < 0)
            {
                if(currentWaveTime > maxWaveTime && Motor.CameraBase.Instance != null)
                {
                    currentWaveTime = 0;
                    Common.SustainCoroutine.Instance.AddCoroutine(CameraWave);
                }
            }

            //����Ƿ��Ʒ�
            if (info.SolidRadio < 0)
            {
                //�ڵȴ��׶Σ�ֱ�Ӵ�϶���
                if (nowTime < waitTime)
                {
                    animateManage.ExitAnimate(1, releaseTime);
                    mana.SetEndTime(Time.time + releaseTime);
                    motor.Move(0, 0);
                }
                //�����׶���ȫ���壬�������
                else if (nowTime < waitTime + upTime)
                {
                    return false;
                }
                //��һ�α���ϣ�����Э��
                else
                {
                    animateManage.ExitAnimate(1, releaseTime);
                    mana.SetEndTime(Time.time + releaseTime);
                    nowTime = 0;
                    motor.Move(0, 0);
                    Common.SustainCoroutine.Instance.AddCoroutine(BeBreakWait);
                }
            }
            else
            {
                if (finalHp < 0)
                {
                    animateManage.ExitAnimate(1, 0.05f);
                    mana.SetEndTime(Time.time + 0.05f);
                    motor.Move(0, 0);
                }
            }
            return false;
        }

        float nowTime, waitTime = 5, upTime = 3;
        Info.CharacterInfo info;

        /// <summary>
        /// ����ֵͦ������ʱִ�еķ������ȴ�����ʱ��
        /// </summary>
        bool BeBreakWait()
        {
            nowTime += Time.deltaTime;
            if (nowTime >= waitTime)
            {
                Common.SustainCoroutine.Instance.AddCoroutine(AddSolid);
                return true;
            }
            return false;
        }

        /// <summary>     /// ���ָܻ�    /// </summary>
        bool AddSolid()
        {
            nowTime += Time.deltaTime;
            if (nowTime >= waitTime + upTime)
            {
                info.SetSolidRadio(info.MaxSolidRadio);
                return true;
            }
            info.SetSolidRadio((int)Mathf.Lerp(0,
                info.MaxSolidRadio, (nowTime - waitTime) / upTime));
            return false;
        }

        float currentWaveTime, maxWaveTime = 3, waveSpeed = 8;
        int waveCount = 3;


        //��Ļ����
        bool CameraWave()
        {
            currentWaveTime += Time.deltaTime;
            if(currentWaveTime > maxWaveTime)
            {
                Motor.CameraBase.Instance.BackToBegin();
                return true;
            }
            float radio = currentWaveTime * waveCount * 2;
            int count = (int)radio;
            radio -= count;     //ȡβ��
            if(count % 2 == 0)  //�����ƶ�
            {
                count /= 2; count = Mathf.Max(count, 1);
                float speed = Mathf.Lerp(waveSpeed / count, 0, radio) * Time.deltaTime;
                Motor.Camera2D.Instance.AdjustPosition(new Vector3(0, speed, 0));
            }
            else                //�����ƶ�
            {
                count /= 2; count = Mathf.Max(count, 1);
                float speed = Mathf.Lerp(waveSpeed / count, 0, radio) * Time.deltaTime;
                Motor.Camera2D.Instance.AdjustPosition(new Vector3(0, -speed, 0));
            }
            return false;
        }

        public float GetRunSpeed(Info.CharacterInfo info, float runSpeed)
        {
            return runSpeed;
        }

        public float GetWalkSpeed(Info.CharacterInfo info, float walkSpeed)
        {
            return walkSpeed;
        }

        public int GetAttack(Info.CharacterInfo info, int attack)
        {
            return attack;
        }

        public int GetDefense(Info.CharacterInfo info, int defense)
        {
            return defense;
        }
    }
}