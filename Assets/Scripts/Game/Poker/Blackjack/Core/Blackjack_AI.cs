using Common;
using UnityEngine;

/// <summary>/// 21点的玩家AI/// </summary>
public class Blackjack_AI : Blackjack_User
{
    private ISetOneParam<int> recall;

    float time = 0;

    public override void BeginChoose(ISetOneParam<int> recall)
    {
        this.recall = recall;
        time = 0;
        string str = "AI当前牌";
        for (int i = 0; i < Cards.Count; i++)
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
            Debug.Log("AI 爆掉");
        }
        else
        {
            Debug.Log("AI 胜利");
        }
    }

    private bool WaitChoose()
    {
        time += Time.deltaTime;
        if(time > 1.0f)
        {
            if(Point < 13)
                recall(1);
            else
                recall(0);
            return true;
        }
        return false;
    }


}
