using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抓鬼牌的用户基类，定义一些通用的用户接口
/// </summary>
public abstract class DrawGhost_User
{
    private bool hasChange = false;

    private bool choosing;
    private DrawGhostCard origin;
    public DrawGhostCard Origin => origin;

    private ISetOneParam<int> recall;
    private PoolingList<int> cards = new PoolingList<int>();

    public bool Choosing => choosing;
    /// <summary>/// 用来判断卡片数组是否有变化用的数据 /// </summary>
    public bool HasChange
    {
        get { return hasChange; }
        set { hasChange = value; }
    }
    /// <summary>/// 该用户的所有牌号 /// </summary>
    public PoolingList<int> Cards
    {
        get { return cards; }
        set
        {
            hasChange = true;
            cards = value;
        }
    }

    public void AddCard(int card)
    {
        hasChange = true;
        cards.Add(card);
    }


    public abstract void OnEnd(bool isDie);

    /// <summary>  /// 改变选择的卡的值, 基类只表示卡的数据改变了  /// </summary>
    public void ChangeCard(int index, int value)
    {
        hasChange = true;
        cards.SetValue(index, value);
    }

    /// <summary>/// 开启选择权限/// </summary>
    /// <param name="recall">会调函数，用来确定交换的编号</param>
    public virtual void BeginChoose(ISetOneParam<int> recall,
        DrawGhostCard drawGhost)
    {
        this.recall = recall;
        origin = drawGhost;
        choosing = true;
    }

    /// <summary>
    /// 进行选中的函数，选择自己的一张以及别人的一张进行交换
    /// </summary>
    public void Recall(int targetIndex)
    {
        choosing = false;
        recall(targetIndex);
    }
}
