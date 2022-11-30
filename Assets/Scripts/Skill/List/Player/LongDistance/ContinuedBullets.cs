using UnityEngine;

namespace Skill
{

    public class ContinuedBullets : SkillBase
    {

        GameObject originBullet;

        public ContinuedBullets()
        {
            maxSkillCount = -1;
            skillCount = maxSkillCount;
            preReleaseTime = 0;
            skillCount = maxSkillCount;
            releaseTime = 0.1f;
            coolTime = 0.05f;
            skillName = "子弹";
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