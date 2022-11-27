using Common;
using System.Collections.Generic;
using UnityEngine;
public abstract class Blackjack_User : IBlackjack
{
    private List<int> cards;
    public List<int> Cards => cards;
    public int Point
    {
        get
        {
            int point = 0;
            int aCount = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                int tem = cards[i] % 13;
                if(tem == 0)
                {
                    aCount++;
                    continue;
                }
                point += Mathf.Min(tem + 1, 10);
            }
            int distance = 21 - point;  //差距
            //判断最好的A
            while(aCount > 0)
            {
                if(distance <= 0)
                {
                    point += aCount;
                    aCount = 0;
                }
                else
                {
                    if(distance >= (aCount - 1) + 11)
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

    public void AddCard(int card)
    {
        if (cards == null)
            cards = new List<int>();
        cards.Add(card);
    }

    public abstract void BeginChoose(ISetOneParam<int> recall);

    public abstract void OnEnd(bool isDie);
}
