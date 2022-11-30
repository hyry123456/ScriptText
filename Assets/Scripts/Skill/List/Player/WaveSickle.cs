using UnityEngine;


namespace Skill
{

    /// <summary>  
    /// ���ǵĽ�ս�������ܣ�����һ����  
    /// </summary>
    public class WaveSickle : SkillBase
    {
        GameObject origin;  //���ݵ�ԭ����
        Vector3 begin, end;
        float nowRadio = 0;
        Info.CharacterInfo character;
        Sphere_Pooling useObj;  //ʵ��ʹ�õĶ���
        Transform manaTran;
        Camera cam;

        public WaveSickle()
        {
            maxSkillCount = -1;
            skillCount = maxSkillCount;
            preReleaseTime = 0;
            coolTime = 0.5f;
            skillName = "Wave Sickle";
            skillType = SkillType.NearDisAttack;
        }

        public override bool OnSkillRelease(SkillManage mana)
        {
            if (origin == null)
                origin = Resources.Load<GameObject>("Prefab/Sphere_Pooling");
            if (character == null)
                character = mana.GetComponent<Info.CharacterInfo>();
            cam = Camera.main;
            if (cam == null) return false;
            manaTran = mana.transform;

            begin = mana.transform.position + (cam.transform.forward +
                cam.transform.right + cam.transform.up);
            useObj = Common.SceneObjectPool.Instance.GetObject<Sphere_Pooling>(
                "Sphere_Pooling", origin, begin, manaTran.position);        //����������ߣ�������ײ���
            useObj.collsionEnter = (Collision collision) =>
            {
                Info.CharacterInfo character = collision.gameObject.GetComponent<Info.CharacterInfo>();
                if (character == null) return;
            };

            end = mana.transform.position + (cam.transform.forward + 
                -cam.transform.right + -cam.transform.up);
            nowRadio = 0;
            Common.SustainCoroutine.Instance.AddCoroutine(WaveSickleSustain);
            return true;
        }

        bool WaveSickleSustain()
        {
            nowRadio += Time.deltaTime * 5;
            if(!useObj.gameObject.activeSelf)
            {
                //useObj.CloseObject();
                return true;
            }
            else if(nowRadio > 1 || cam == null)
            {
                useObj.CloseObject();
                return true;
            }

            begin = manaTran.position + (cam.transform.forward +
                cam.transform.right + cam.transform.up);
            end = manaTran.position + (cam.transform.forward +
                -cam.transform.right + -cam.transform.up);

            useObj.transform.position = Vector3.Lerp(begin, end, nowRadio);
            return false;
        }
    }
}