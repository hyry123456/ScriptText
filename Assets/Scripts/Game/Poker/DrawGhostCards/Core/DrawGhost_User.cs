using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ץ���Ƶ��û����࣬����һЩͨ�õ��û��ӿ�
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
    /// <summary>/// �����жϿ�Ƭ�����Ƿ��б仯�õ����� /// </summary>
    public bool HasChange
    {
        get { return hasChange; }
        set { hasChange = value; }
    }
    /// <summary>/// ���û��������ƺ� /// </summary>
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

    /// <summary>  /// �ı�ѡ��Ŀ���ֵ, ����ֻ��ʾ�������ݸı���  /// </summary>
    public void ChangeCard(int index, int value)
    {
        hasChange = true;
        cards.SetValue(index, value);
    }

    /// <summary>/// ����ѡ��Ȩ��/// </summary>
    /// <param name="recall">�������������ȷ�������ı��</param>
    public virtual void BeginChoose(ISetOneParam<int> recall,
        DrawGhostCard drawGhost)
    {
        this.recall = recall;
        origin = drawGhost;
        choosing = true;
    }

    /// <summary>
    /// ����ѡ�еĺ�����ѡ���Լ���һ���Լ����˵�һ�Ž��н���
    /// </summary>
    public void Recall(int targetIndex)
    {
        choosing = false;
        recall(targetIndex);
    }
}
