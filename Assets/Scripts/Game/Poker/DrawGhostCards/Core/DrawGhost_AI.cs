using Common;
using UnityEngine;

/// <summary>/// ����Ƶ�AI�������Ͼ���ׯ��/// </summary>
public class DrawGhost_AI : DrawGhost_User
{
    public override void OnEnd(bool isDie)
    {

    }

    public override void BeginChoose(ISetOneParam<Pair<int, int>> recall, DrawGhostCard drawGhost)
    {
        base.BeginChoose(recall, drawGhost);
        //string str = "AI��ǰ��";
        //for (int i = 0; i < Cards.Count; i++)
        //{
        //    str += Poker.GetCardName(Cards.GetValue(i)) + " ";
        //}
        //Debug.Log(str);
    }
}
