
using System.Collections.Generic;
using UnityEngine;

/// <summary>/// 21点的庄家类/// </summary>
public class Banker
{
    //暗牌
    private int blackCard;
    //明牌
    private int whiteCard;

    private bool showBlack;
    public bool ShowBlack
    {
        get { return showBlack; }
        set { showBlack = value; }
    }

    public int BlackCard
    {
        get { return blackCard; }
        set { blackCard = value; }
    }
    public int WhiteCard
    {
        set { whiteCard = value; }
        get { return whiteCard; }
    }

    private List<int> cards = new List<int>();
    public List<int> Cards => cards;

    public void AddCard(int card)
    {
        if(cards == null)
            cards = new List<int>();
        cards.Add(card);
    }

    public int Point
    {
        get
        {
            int point = 0;
            int aCount = 0;
            if (blackCard % 13 != 0)
                point += Mathf.Min((blackCard % 13), 10);
            else
                aCount++;
            if (whiteCard % 13 != 0)
                point += Mathf.Min((whiteCard % 13), 10);
            else
                aCount++;
            for (int i = 0; i < cards.Count; i++)
            {
                int tem = cards[i] % 13;
                if (tem == 0)
                {
                    aCount++;
                    continue;
                }
                point += Mathf.Min((cards[i] % 13), 10);
            }
            int distance = 21 - point;  //差距
            //判断最好的A
            while (aCount > 0)
            {
                if (distance <= 0)
                {
                    point += aCount;
                    aCount = 0;
                }
                else
                {
                    if (distance >= (aCount - 1) + 11)
                    {
                        point += 11;
                        aCount--;
                    }
                    else
                    {
                        point += 1;
                        aCount--;
                    }
                }
            }

            return point;
        }
    }

    Common.ISetOneParam<int> recall;

    /// <summary> /// 庄持续拿牌 /// </summary>
    public void BeginCircleGetCard(Common.ISetOneParam<int> recall)
    {
        showBlack = true;       //拿牌时直接显示隐藏牌
        time = 0;
        this.recall = recall;
        string str = "庄家当前牌";
        str += Poker.GetCardName(whiteCard) + " ";
        str += Poker.GetCardName(blackCard) + " ";
        for (int i = 0; i < Cards.Count; i++)
        {
            str += Poker.GetCardName(Cards[i]) + " ";
        }
        str += " 号码为" + Point.ToString();
        Debug.Log(str);
        Common.SustainCoroutine.Instance.AddCoroutine(CircleGetCard,
            true, true);
    }

    float time;

    private bool CircleGetCard()
    {
        time += Time.deltaTime;
        if(time > 1.0f)
        {
            if(Point < 17)
            {
                Debug.Log("庄家拿牌");
                recall(1);
                return true;
            }
            else
            {
                Debug.Log("庄家停牌");
                recall(2);
                return true;
            }
        }
        return false;
    }
}
