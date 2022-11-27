using Common;
using UnityEngine;

/// <summary>/// 抽鬼牌的AI，本质上就是庄家/// </summary>
public class DrawGhost_AI : DrawGhost_User
{
    public override void OnEnd(bool isDie)
    {

    }

    public override void BeginChoose(ISetOneParam<Pair<int, int>> recall, DrawGhostCard drawGhost)
    {
        base.BeginChoose(recall, drawGhost);
        //string str = "AI当前牌";
        //for (int i = 0; i < Cards.Count; i++)
        //{
        //    str += Poker.GetCardName(Cards.GetValue(i)) + " ";
        //}
        //Debug.Log(str);
    }
}
