using Common;
using System.Collections.Generic;
using UnityEngine;
using System;



public class DrawGhostCard : Poker
{
    private DrawGhost_AI ai;
    private DrawGhost_Player player;
    public DrawGhost_AI AI => ai;
    public DrawGhost_Player Player => player;

    public PoolingList<int> AiCards => ai.Cards;
    public PoolingList<int> PlayerCards => player.Cards;
    float time;
    int index;

    /// <summary>
    /// 抽鬼牌的控制类，用于所有UI在该类上，需要呼叫其调换UI
    /// </summary>
    private DrawGhostControl control;

    public void InitilizeGame(DrawGhostControl control)
    {
        this.control = control;
        ai = new DrawGhost_AI();
        player = new DrawGhost_Player();
        InitilizePoker(true, false);
        //抽26张牌给玩家
        for(int i = 0; i < 27; i++)
        {
            player.AddCard(GetCard());
        }
        for (int i = 0; i < 26; i++)
        {
            ai.AddCard(GetCard());
        }
        var playCards = player.Cards;
        var aiCards = ai.Cards;
        RemoveSample(ref playCards);
        RemoveSample(ref aiCards);

        Array.Sort(player.Cards.Arrays, 0, player.Cards.Count);
        Array.Sort(ai.Cards.Arrays, 0, ai.Cards.Count);

        Debug.Log("开始游戏");

        time = 0; index = 0;
        SustainCoroutine.Instance.AddCoroutine(Circle);
    }


    /// <summary>  /// 移除重复的部分 /// </summary>
    public static void RemoveSample(ref PoolingList<int> cards)
    {
        Dictionary<int, int> map = new Dictionary<int, int>();

        for(int i = 0; i < cards.Count; i++)
        {
            //跳过王
            if (cards.GetValue(i) >= 52)
            {
                map.Add(14, i);
                continue;
            }
            int value = cards.GetValue(i);
            //确定其偏移号
            int current = value % 13;
            if (map.ContainsKey(current))  //存在偏移号重复的
            {
                map.Remove(current);
            }
            else
            {
                map.Add(current, i);
            }
        }

        for(int i = cards.Count - 1; i >= 0; i--)
        {
            if (!map.ContainsValue(i))
                cards.RemoveIndex(i);
        }
        return;
    }

    /// <summary>/// 确定需要移除的部分 /// </summary>
    /// <returns>需要移除的编号</returns>
    public static List<int> GetRemove(PoolingList<int> cards)
    {
        Dictionary<int, int> map = new Dictionary<int, int>();

        for (int i = 0; i < cards.Count; i++)
        {
            //跳过王
            if (cards.GetValue(i) >= 52)
            {
                map.Add(14, i);
                continue;
            }
            int value = cards.GetValue(i);
            //确定其偏移号
            int current = value % 13;
            if (map.ContainsKey(current))  //存在偏移号重复的
            {
                map.Remove(current);
            }
            else
            {
                map.Add(current, i);
            }
        }
        List<int> re = new List<int>();

        for (int i = 0; i < cards.Count; i++)
        {
            if (!map.ContainsValue(i))
                re.Add(i);
        }
        return re;
    }

    void Recall(Pair<int, int> pair)
    {
        control.ChangeCard(pair, index == 0);
        //if (index == 0)          //主角
        //{
        //    int key = player.Cards.GetValue(pair.Key);
        //    int value = ai.Cards.GetValue(pair.Value);
        //    Debug.Log(Poker.GetCardName(key) + "<=>"
        //        + Poker.GetCardName(value));
        //    player.ChangeCard(pair.Key, value);
        //    ai.ChangeCard(pair.Value, key);
        //}
        //else
        //{
        //    int key = ai.Cards.GetValue(pair.Key);
        //    int value = player.Cards.GetValue(pair.Value);
        //    Debug.Log(Poker.GetCardName(key) + "<=>"
        //        + Poker.GetCardName(value));
        //    ai.ChangeCard(pair.Key, value);
        //    player.ChangeCard(pair.Value, key);
        //}
        //player.Cards = RemoveSample(player.Cards);
        //ai.Cards = RemoveSample(ai.Cards);


    }

    //动画播放结束后的调用，继续后续循环
    public void ContinueCircle()
    {
        if (player.Cards.Count == 0 || ai.Cards.Count == 0)
        {
            player.OnEnd(player.Cards.Count == 0 ? false : true);
            ai.OnEnd(ai.Cards.Count == 0 ? false : true);
            return;
        }

        index++;
        index %= 2;
        time = 0;
        SustainCoroutine.Instance.AddCoroutine(Circle);
    }

    bool Circle()
    {
        time += Time.deltaTime;
        if(time > 1.0f)
        {
            if(index == 0)      //玩家
            {
                player.BeginChoose(Recall, this);
            }
            else
            {
                ai.BeginChoose(Recall, this);
            }
            return true;
        }
        return false;
    }


}
