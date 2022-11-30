using Common;
using UnityEngine;


public class Blackjack_Player : Blackjack_User
{

    private ISetOneParam<int> recall;

    public override void BeginChoose(ISetOneParam<int> recall)
    {
        this.recall = recall;
        string str = "玩家当前牌";
        for(int i = 0; i < Cards.Count; i++)
        {
            str += Poker.GetCardName(Cards[i]) + " ";
        }
        str += " 号码为" + Point.ToString();
        Debug.Log(str);
        SustainCoroutine.Instance.AddCoroutine(WaitChoose);
    }

    public override void OnEnd(bool isDie)
    {
        if (isDie)
        {
            Debug.Log("玩家爆掉");
        }
        else
        {
            Debug.Log("玩家胜利");
        }
    }

    private bool WaitChoose()
    {
        //再抽一张
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            recall(1);
            return true;
        }
        //停止抽牌
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            recall(2);
            return true;
        }
        return false;
    }
}
