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

        public override void EnterTaskEvent(Chapter chapter, bool isLoaded)
        {
            base.EnterTaskEvent(chapter, isLoaded);
            Debug.Log("章节进入");
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
                dialog.BeginDialog("你好\n我是马云", () =>
                {
                    Debug.Log("对话结束");
                    nPC.CloseObject();
                    recall();
                });
                
            };
            return true;
        }


        public override void ExitTaskEvent(Chapter chapter)
        {
            Debug.Log("章节退出");
        }

        public override bool IsCompleteTask(Chapter chapter, InteracteInfo info)
        {
            return true;
        }
    }
}