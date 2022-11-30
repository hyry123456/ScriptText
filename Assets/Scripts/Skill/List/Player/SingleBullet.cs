
using UnityEngine;

namespace Skill
{
    public class SingleBullet : SkillBase
    {
        GameObject originBullet;

        public SingleBullet()
        {
            maxSkillCount = -1;
            skillCount = maxSkillCount;
            preReleaseTime = 0;
            releaseTime = 3;
            coolTime = 1;
            skillName = "Single Bullet";
            skillType = SkillType.LongDisAttack;
            originBullet = Resources.Load<GameObject>("Prefab/poolingBullet");
        }

        public override bool OnSkillRelease(SkillManage mana)
        {
            Camera camera = Camera.main;
            if (camera == null) return false;
            Bullet_Pooling bullet_Pooling = 
                Common.SceneObjectPool.Instance.GetObject<Bullet_Pooling>("Pooling_Bullet", 
                originBullet, mana.transform.position + camera.transform.forward, 
                camera.transform.rotation);
            bullet_Pooling.attackLayer = mana.CharacterInfo.attackLayer;
            return true;
        }
    }
}