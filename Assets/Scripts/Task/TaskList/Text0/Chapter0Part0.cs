using Interaction;
using UnityEngine;

namespace Task
{
    public class Chapter0Part0 : ChapterPart
    {
        GameObject dialogOrigin;
        GameObject character;
        Common.NPC_Pooling nPC;
        UI.CharacterDialog dialog;

        int choseIndex = 0;

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            base.EnterTaskEvent(chapter, isLoaded);
            Common.SustainCoroutine.Instance.AddCoroutine(Wait);
        }

        private bool Wait()
        {
            if(dialogOrigin == null)
            {
                dialogOrigin = Resources.Load<GameObject>("Prefab/Dialog");
                return false;
            }
            if(character == null)
            {
                character = Resources.Load<GameObject>("Prefab/NPC");
                return false;
            }
            nPC = Common.SceneObjectPool.Instance.GetObject<Common.NPC_Pooling>
                ("NPC", character, new Vector3(3, 0.8f, 0), Quaternion.identity);
            InteracteDelegate interacte = nPC.gameObject.AddComponent<InteracteDelegate>();
            interacte.Delegate = (recall) =>
            {
                GameObject worldCanves = Common.SceneObjectMap.Instance.
                    FindControlObject("WorldCanvas");
                if (worldCanves == null)
                    return;
                dialog = Common.SceneObjectPool.Instance.GetObject<UI.CharacterDialog>
                ("Dialog", dialogOrigin, nPC.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                dialog.transform.parent = worldCanves.transform;
                dialog.BeginDialog("���\n��������", () =>
                {
                    Debug.Log("�Ի�����");
                    nPC.CloseObject();
                    recall();
                });
                
            };


            string[] strings = new string[]
            {
                chapter.GetDiglogText(0),
                chapter.GetDiglogText(1),
                chapter.GetDiglogText(2),
            };
            choseIndex = -1;

            UI.OptionUI.Instance.AddOptions(strings, (index) =>
            {
                Debug.Log("�����˵�" + index.ToString() + "ѡ��");
                choseIndex = index;
            });

            return true;
        }


        public override void ExitTaskEvent(Chapter chapter, bool isInterrupt)
        {
            if (isInterrupt)
            {
                //�������ʱ������������ڳ��оʹ��
                Common.SustainCoroutine.Instance.RemoveCoroutine(Wait);
            }
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            return true;
        }

        public override string GetNextPartString()
        {
            return null;
        }

        public override string GetThisPartString()
        {
            return "0";
        }
    }
}