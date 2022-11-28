using Common;
using UnityEngine;

public class DrawGhost_Player : DrawGhost_User
{

    public override void OnEnd(bool isDie)
    {
        if (isDie)
        {
            Debug.Log("����ʧ��");
        }
        else
        {
            Debug.Log("ʤ��");
        }
    }

    public override void BeginChoose(ISetOneParam<int> recall, DrawGhostCard drawGhost)
    {
        base.BeginChoose(recall, drawGhost);
        string str = "��ҵ�ǰ��";
        for (int i = 0; i < Cards.Count; i++)
        {
            str += Poker.GetCardName(Cards.GetValue(i)) + " ";
        }
        Debug.Log(str);
        str = "AI��ǰ��";
        for (int i = 0; i < Origin.AiCards.Count; i++)
        {
            str += Poker.GetCardName(Origin.AiCards.GetValue(i)) + " ";
        }
        Debug.Log(str);
    }
}
