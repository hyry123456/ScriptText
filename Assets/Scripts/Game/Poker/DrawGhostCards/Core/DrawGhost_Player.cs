using Common;
using UnityEngine;

public class DrawGhost_Player : DrawGhost_User
{

    public override void OnEnd(bool isDie)
    {
        if (isDie)
        {
            Debug.Log("主角失败");
        }
        else
        {
            Debug.Log("胜利");
        }
    }

    public override void BeginChoose(ISetOneParam<int> recall, DrawGhostCard drawGhost)
    {
        base.BeginChoose(recall, drawGhost);
        string str = "玩家当前牌";
        for (int i = 0; i < Cards.Count; i++)
        {
            str += Poker.GetCardName(Cards.GetValue(i)) + " ";
        }
        Debug.Log(str);
        str = "AI当前牌";
        for (int i = 0; i < Origin.AiCards.Count; i++)
        {
            str += Poker.GetCardName(Origin.AiCards.GetValue(i)) + " ";
        }
        Debug.Log(str);
    }
}
