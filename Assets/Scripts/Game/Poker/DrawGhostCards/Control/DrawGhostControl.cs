using UnityEngine;
using Common;
using System.Collections.Generic;

/// <summary>
/// 抽鬼牌控制类
/// </summary>
public class DrawGhostControl : MonoBehaviour
{
    private Object[] sprits;
    [SerializeField]
    private GameObject pockerItem;
    public GameObject PockerItem => pockerItem;

    private DrawGhostShow_Player player;
    private DrawGhostShow_AI banker;
    public DrawGhostShow_AI Banker => banker;
    public DrawGhostShow_Player Player => player;
    public ClickRemain PlayerChoose
    {
        get
        {
            return player.CurrentRemain;
        }
    }
    public ClickRemain BankerChoose
    {
        get { return banker.CurrentRemain; }
    }

    DrawGhostCard drawGhost;

    /// <summary>/// 根据编号读取棋牌图片/// </summary>
    /// <param name="index">棋牌编号</param>
    public Sprite GetPoker(int index)
    {
        int group = index / 13;
        int offset = index % 13;
        Object tex;     //需要注意，读取的sprites中第一个数据不是Sprite，故要加1
        if(group >= 4)
        {
            tex = sprits[group * 13 + offset + 2];
            return (Sprite)tex;
        }
        if (offset == 0)
        {
            if (group * 13 + 13 >= sprits.Length)
                Debug.Log(group);
            tex = sprits[group * 13 + 13];
        }
        else
            tex = sprits[group * 13 + offset + 1];
        return (Sprite)tex;
    }

    private void Start()
    {
        sprits = Resources.LoadAll<Object>("UI/Pocker");
        banker = GetComponentInChildren<DrawGhostShow_AI>();
        player = GetComponentInChildren<DrawGhostShow_Player>();
        banker.Control = this; player.Control = this;
        drawGhost = new DrawGhostCard();
        drawGhost.InitilizeGame(this);
        banker.InitializedCard(drawGhost.AiCards, Vector2.zero, drawGhost.AI);
        player.InitializedCard(drawGhost.PlayerCards, Vector2.zero, drawGhost.Player);
    }

    [SerializeField]
    Pair<int, int> change;
    [SerializeField]
    float time;
    Pair<Vector3, Vector3> begin;
    Pair<Vector3, Vector3> target;

    [Range(0, 1.1f)]
    public float radio = 0;

    public void ChangeCard(Pair<int, int> change, bool isPlayer)
    {
        if (isPlayer)
        {
            this.change = change;
            target = new Pair<Vector3, Vector3>(
                banker.GetRemainTransform(change.Value).position,
                player.GetRemainTransform(change.Key).position);
            begin = new Pair<Vector3, Vector3>(
                player.GetRemainTransform(change.Key).position,
                banker.GetRemainTransform(change.Value).position);
        }
        else
        {
            this.change = new Pair<int, int>(change.Value, change.Key);
            target = new Pair<Vector3, Vector3>(
                banker.GetRemainTransform(change.Key).position,
                player.GetRemainTransform(change.Value).position);
            begin = new Pair<Vector3, Vector3>(
                player.GetRemainTransform(change.Value).position,
                banker.GetRemainTransform(change.Key).position);
        }
        this.change = change;
        time = 0;
        SustainCoroutine.Instance.AddCoroutine(ChangeCard);
    }

    private bool ChangeCard()
    {
        time += Time.deltaTime;
        if (time > 2.0f)
        {
            MoveCard(0);
            int key = drawGhost.Player.Cards.GetValue(change.Key);
            int value = drawGhost.AI.Cards.GetValue(change.Value);
            drawGhost.Player.ChangeCard(change.Key, value);
            drawGhost.AI.ChangeCard(change.Value, key);
            player.RemainLists[change.Key].Image.sprite = GetPoker(value);
            banker.RemainLists[change.Value].Image.sprite = GetPoker(key);
            RemoveEqualAndReset();
            return true;
        }
        MoveCard(time / 2.0f);

        //if (radio > 1.0f)
        //{
        //    MoveCard(1);
        //    int key = drawGhost.Player.Cards.GetValue(change.Key);
        //    int value = drawGhost.AI.Cards.GetValue(change.Value);
        //    drawGhost.Player.ChangeCard(change.Key, value);
        //    drawGhost.AI.ChangeCard(change.Value, key);
        //    drawGhost.AI.Cards = drawGhost.RemoveSample(drawGhost.AI.Cards);
        //    drawGhost.Player.Cards = drawGhost.RemoveSample(drawGhost.Player.Cards);
        //    drawGhost.ContinueCircle();
        //    radio = 0.0f;
        //    return true;
        //}
        //MoveCard(radio);
        return false;
    }

    void MoveCard(float radio01)
    {
        RectTransform rect_Play = player.GetRemainTransform(change.Key);
        RectTransform rect_Bank = banker.GetRemainTransform(change.Value);
        rect_Play.position = Vector3.Lerp(
            begin.Key, target.Key, radio01);
        rect_Bank.position = Vector2.Lerp(
            begin.Value, target.Value, radio01);
    }

    //待移除的数组
    List<int> playRemove, bankRemove;


    void RemoveEqualAndReset()
    {
        playRemove = DrawGhostCard.GetRemove(drawGhost.PlayerCards);
        bankRemove = DrawGhostCard.GetRemove(drawGhost.AiCards);
        time = 0;
        if(playRemove.Count == 0 && bankRemove.Count == 0)
        {
            drawGhost.ContinueCircle();
        }
        else
            SustainCoroutine.Instance.AddCoroutine(Remove);
    }

    List<Pair<Vector2, Vector2>> movePlay = new List<Pair<Vector2, Vector2>>();
    List<Pair<Vector2, Vector2>> moveBank = new List<Pair<Vector2, Vector2>>();

    //等待扑克牌移除
    bool Remove()
    {
        time += Time.deltaTime;
        if(time > 3.0f)
        {
            for(int i = playRemove.Count - 1; i >= 0; i--)
            {
                Debug.Log("移除了Player" + DrawGhostCard.
                    GetCardName(drawGhost.PlayerCards.GetValue(playRemove[i])));
                Destroy(player.RemainLists[playRemove[i]].gameObject);
                player.RemainLists.RemoveAt(playRemove[i]);
                drawGhost.PlayerCards.RemoveIndexSave(playRemove[i]);
            }
            for(int i = bankRemove.Count - 1; i >= 0; i--)
            {
                Debug.Log("移除了Banker" + DrawGhostCard.
                    GetCardName(drawGhost.AiCards.GetValue(bankRemove[i])));
                Destroy(banker.RemainLists[bankRemove[i]].gameObject);
                banker.RemainLists.RemoveAt(bankRemove[i]);
                drawGhost.AiCards.RemoveIndexSave(bankRemove[i]);
            }

            moveBank.Clear(); movePlay.Clear();
            for(int i = 0; i < player.RemainLists.Count; i++)
            {
                movePlay.Add(new Pair<Vector2, Vector2>(
                    player.RemainLists[i].RectTransform.anchoredPosition,
                    player.GetTargetPos(i)));
                player.RemainLists[i].Origin = player;
                player.RemainLists[i].Index = i;
            }
            for (int i = 0; i < banker.RemainLists.Count; i++)
            {
                moveBank.Add(new Pair<Vector2, Vector2>(
                    banker.RemainLists[i].RectTransform.anchoredPosition,
                    banker.GetTargetPos(i)));
                banker.RemainLists[i].Origin = banker;
                banker.RemainLists[i].Index = i;
            }
            time = 0;
            SustainCoroutine.Instance.AddCoroutine(MovePocker);
            return true;
        }
        Color color = Color.gray;
        color.a = 1.0f - time / 3.0f;
        for(int i = 0; i < playRemove.Count; i++)
            player.RemainLists[playRemove[i]].Image.color = color;
        for (int i = 0; i < bankRemove.Count; i++)
            banker.RemainLists[bankRemove[i]].Image.color = color;
        return false;
    }

    bool MovePocker()
    {
        time += Time.deltaTime;
        if(time > 2.0f)
        {
            MovePocker(1);
            drawGhost.ContinueCircle();
            return true;
        }
        MovePocker(time / 2.0f);
        return false;
    }

    void MovePocker(float radio01)
    {
        for (int i = 0; i < player.RemainLists.Count; i++)
        {
            player.RemainLists[i].RectTransform.anchoredPosition
                = Vector2.Lerp(movePlay[i].Key, movePlay[i].Value, radio01);
        }
        for (int i = 0; i < banker.RemainLists.Count; i++)
        {
            banker.RemainLists[i].RectTransform.anchoredPosition
                = Vector2.Lerp(moveBank[i].Key, moveBank[i].Value, radio01);
        }
    }
}
