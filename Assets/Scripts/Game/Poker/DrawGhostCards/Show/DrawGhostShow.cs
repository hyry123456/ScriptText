using System.Collections.Generic;
using UnityEngine;
using Common;

public abstract class DrawGhostShow : MonoBehaviour
{
    protected DrawGhostControl control;
    public DrawGhostControl Control
    {
        set { control = value; }
    }

    protected DrawGhost_User user;
    public DrawGhost_User User => user;

    public Vector2 itemSize;
    public Vector2 positionOffset;
    public float nearLength = -10;

    protected ClickRemain currentRemain;
    public ClickRemain CurrentRemain => currentRemain;

    protected List<ClickRemain> remainLists = new List<ClickRemain>();
    public List<ClickRemain> RemainLists => remainLists;
    public RectTransform GetRemainTransform(int index)
    {
        return remainLists[index].RectTransform;
    }

    public abstract void OnChangeChoose(ClickRemain newRemain);

    /// <summary> /// 游戏的初始化，用来第一次牌的初始化 /// </summary>
    /// <param name="cards"></param>
    /// <param name="beginPos"></param>
    public void InitializedCard(PoolingList<int> cards, Vector2 beginPos,
        DrawGhost_User user)
    {
        this.user = user;
        move.RemoveAll();
        //首先创建足够的牌子，设置位置，并初始化
        for(int i = 0; i < cards.Count; i++)
        {
            GameObject item = GameObject.Instantiate(control.PockerItem);
            item.transform.parent = transform;
            ClickRemain remain = item.AddComponent<ClickRemain>();
            remain.Image.color = Color.grey;
            remain.Image.sprite = control.GetPoker(cards.GetValue(i));
            remain.Index = i;
            remain.Origin = this;
            move.Add(new Pair<Vector2, Vector2>(beginPos, GetTargetPos(i)));
            item.GetComponent<RectTransform>().anchoredPosition = beginPos;
            remainLists.Add(remain);
        }

        SustainCoroutine.Instance.AddCoroutine(InitializedPos);
    }

    protected float time = 0;

    public bool InitializedPos()
    {
        time += Time.deltaTime;
        if(time > 2.0f)
        {
            MoveCard(1);
            user.HasChange = false;
            SustainCoroutine.Instance.AddCoroutine(Circle);
            return true;
        }
        MoveCard(time / 2.0f);
        return false;
    }

    public virtual void MoveCard(float radio01)
    {
        for(int i = 0; i < remainLists.Count; i++)
        {
            RectTransform rect = remainLists[i].RectTransform;
            rect.sizeDelta = itemSize;
            Pair<Vector2, Vector2> pair = move.GetValue(i);
            rect.anchoredPosition = Vector2.Lerp(
                pair.Key, pair.Value, radio01);
        }
    }

    public virtual Vector2 GetTargetPos(int index)
    {
        return new Vector2(itemSize.x * index + positionOffset.x
                + nearLength * index, itemSize.y + positionOffset.y);
    }


    protected bool Circle()
    {

        //if (user.HasChange)
        //{
        //    for (int i = 0; i < remainLists.Count && i < user.Cards.Count; i++)
        //    {
        //        remainLists[i].Origin = this;
        //        remainLists[i].Image.sprite = control.GetPoker(user.Cards.GetValue(i));
        //        remainLists[i].Index = i;
        //    }
        //    //多了就删除
        //    while (remainLists.Count > user.Cards.Count)
        //    {
        //        Destroy(remainLists[remainLists.Count - 1].gameObject);
        //        remainLists.RemoveAt(remainLists.Count - 1);
        //    }
        //    user.HasChange = false;
        //}

        if (user.Choosing)
        {
            OnChoosing();
        }
        return false;
    }

    protected abstract void OnChoosing();

    PoolingList<Pair<Vector2, Vector2>> move = 
        new PoolingList<Pair<Vector2, Vector2>>();

    //void InitializeMove()
    //{
    //    time = 0;
    //    move.RemoveAll();
    //    for(int i = 0; i < remainLists.Count; i++)
    //    {
    //        move.Add(new Pair<Vector2, Vector2>(
    //            remainLists[i].RectTransform.anchoredPosition,
    //            GetTargetPos(i)));
    //    }
    //    SustainCoroutine.Instance.AddCoroutine(CircleMovePocker);
    //}

    //bool CircleMovePocker()
    //{
    //    time += Time.deltaTime;
    //    if(time > 1.0f)
    //    {
    //        MoveCard(1);
    //        return true;
    //    }
    //    MoveCard(time / 1.0f);
    //    return false;
    //}
}
