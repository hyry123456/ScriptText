using System.Collections.Generic;
using Common;
using UnityEngine;
public interface IBlackjack
{
    /// <summary>/// 开启选择权限/// </summary>
    /// <param name="recall">会调函数，用来确定选择了什么</param>
    public void BeginChoose(ISetOneParam<int> recall);

    /// <summary>
    ///  结束的调用方法，需要对爆掉和胜利进行区分
    /// </summary>
    /// <param name="isDie">是否爆掉</param>
    public void OnEnd(bool isDie);
}

/// <summary>
/// 21点游戏实现,管理全部游戏流程
/// </summary>
public class Blackjack : Poker
{
    private Banker _banker;
    private List<Blackjack_User> _users;
    private List<Blackjack_User> _finishs = new List<Blackjack_User>();  //停牌且未爆的人

    public List<Blackjack_User> Users => _users;
    public List<Blackjack_User> Finishs => _finishs;
    public Banker Banker => _banker;

    public void InitilizeGame(int aiCount)
    {
        _banker = new Banker();
        _users = new List<Blackjack_User>(aiCount);
        InitilizePoker(false, false);       //初始化所有牌
        Blackjack_Player player = new Blackjack_Player();
        player.AddCard(GetCard()); player.AddCard(GetCard());
        _users.Add(player);     //首先加入玩家

        for(int i = 0; i < aiCount; i++)
        {
            Blackjack_AI ai = new Blackjack_AI();
            ai.AddCard(GetCard());
            ai.AddCard(GetCard());
            _users.Add(ai);
        }

        //庄家抽1个明牌和1个暗牌
        _banker.WhiteCard = GetCard();
        _banker.BlackCard = GetCard();

        index = 0;        //初始化编号
        time = 0;
        SustainCoroutine.Instance.AddCoroutine(Circle);
    }

    /// <summary> /// 游戏结束，判断牌数  /// </summary>
    public void GameEnd()
    {
        int banker = _banker.Point;
        int maxPoint = 0;
        for(int i = 0; i < _finishs.Count; i++)
        {
            if (_finishs[i].Point > maxPoint)
            {
                maxPoint = _finishs[i].Point;
            }
        }
        if(banker > maxPoint)
        {
            Debug.Log("庄家胜利");
        }
        else
        {
            for(int i = 0; i < _finishs.Count; i++)
            {
                if (_finishs[i].Point == maxPoint)
                    _finishs[i].OnEnd(false);
                else
                    _finishs[i].OnEnd(true);
            }
        }
    }

    int index;
    float time = 0;


    void  Recall(int choose)
    {
        if(choose == 1)
        {
            _users[index].AddCard(GetCard());
            if (_users[index].Point > 21)
            {
                _users[index].OnEnd(true);
                _users.RemoveAt(index);
            }
            index++;
            if(_users.Count == 0)
            {
                //庄家揭牌
                _banker.BeginCircleGetCard(BankerRecall);
            }
            else
            {
                index %= _users.Count;
                time = 0;
                SustainCoroutine.Instance.AddCoroutine(Circle);
            }
        }
        else
        {
            if (_finishs == null) _finishs = new List<Blackjack_User>();
            _finishs.Add(_users[index]);
            _users.RemoveAt(index);
            index++;
            if (_users.Count == 0)
            {
                //庄家揭牌
                _banker.BeginCircleGetCard(BankerRecall);
            }
            else
            {
                index %= _users.Count;
                time = 0;
                SustainCoroutine.Instance.AddCoroutine(Circle);
            }
        }
    }

    void BankerRecall(int choose)
    {
        if (choose == 1)
        {
            _banker.AddCard(GetCard());
            if(_banker.Point > 21)
            {
                Debug.Log("庄家爆了");
                //直接赢
                return;
            }
            _banker.BeginCircleGetCard(BankerRecall);
        }
        else
        {
            Debug.Log("游戏结束");
            if (_banker.Point > 21)
            {
                Debug.Log("庄家爆了");
                //直接赢
                return;
            }
            GameEnd();
        }
    }

    /// <summary> /// 游戏循环 /// </summary>
    /// <returns></returns>
    private bool Circle()
    {
        time += Time.deltaTime;
        if(time > 1.0f)
        {
            Debug.Log("选择");
            _users[index].BeginChoose(Recall);
            return true;
        }
        return false;
    }
}
