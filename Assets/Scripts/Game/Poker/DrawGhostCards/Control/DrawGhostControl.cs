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

    bool isPlayer;
    int targetCardIndex;
    [SerializeField]
    float time;
    Pair<Vector3, Vector3> moveRange;
    ClickRemain chooseRemain;

    [Range(0, 1.1f)]
    public float radio = 0;

    public void ChangeCard(int targetCard, bool isPlayer)
    {
        this.isPlayer = isPlayer;
        targetCardIndex = targetCard;
        if (isPlayer)
        {
            moveRange = new Pair<Vector3, Vector3>(
                banker.GetRemainTransform(targetCard).position,
                player.GetTargetWorldPos(drawGhost.PlayerCards.Count));
            chooseRemain = banker.RemainLists[targetCard];
        }
        else
        {
            moveRange = new Pair<Vector3, Vector3>(
                player.GetRemainTransform(targetCard).position,
                banker.GetTargetWorldPos(drawGhost.AiCards.Count));
            chooseRemain = player.RemainLists[targetCard];
        }
        time = 0;
        SustainCoroutine.Instance.AddCoroutine(ChangeCard);
    }

    private bool ChangeCard()
    {
        time += Time.deltaTime;
        if (time > 2.0f)
        {
            MoveCard(1);
            if (isPlayer)
            {
                chooseRemain.transform.parent = player.transform;
                chooseRemain.Index = player.RemainLists.Count;
                player.RemainLists.Add(chooseRemain);
                banker.RemainLists.RemoveAt(targetCardIndex);
                drawGhost.Player.Cards.Add(drawGhost.AiCards.GetValue(targetCardIndex));
                drawGhost.AI.Cards.RemoveIndexSave(targetCardIndex);
            }
            else
            {
                chooseRemain.transform.parent = banker.transform;
                chooseRemain.Index = banker.RemainLists.Count;
                banker.RemainLists.Add(chooseRemain);
                player.RemainLists.RemoveAt(targetCardIndex);
                drawGhost.AiCards.Add(drawGhost.PlayerCards.GetValue(targetCardIndex));
                drawGhost.PlayerCards.RemoveIndexSave(targetCardIndex);
            }
            Check();


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

    private void Check()
    {
        HashSet<int> set = new HashSet<int>();
        for(int i = 0; i < drawGhost.PlayerCards.Count; i++)
        {
            if (set.Contains(drawGhost.PlayerCards.GetValue(i)))
            {
                Debug.Log("组内重复" +
                    DrawGhostCard.GetCardName(drawGhost.PlayerCards.GetValue(i)));
            }
            else
                set.Add(drawGhost.PlayerCards.GetValue(i));
        }

        for(int i = 0; i < drawGhost.AiCards.Count; i++)
        {
            if (set.Contains(drawGhost.AiCards.GetValue(i)))
            {
                Debug.Log("两边重复" +
                    DrawGhostCard.GetCardName(drawGhost.AiCards.GetValue(i)));
            }
            else
                set.Add(drawGhost.AiCards.GetValue(i));
        }
    }

    void MoveCard(float radio01)
    {
        chooseRemain.RectTransform.position =
            Vector3.Lerp(moveRange.Key, moveRange.Value, radio01);
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
                    player.GetTargetAnchoredPos(i)));
                player.RemainLists[i].Origin = player;
                player.RemainLists[i].Index = i;
            }
            for (int i = 0; i < banker.RemainLists.Count; i++)
            {
                moveBank.Add(new Pair<Vector2, Vector2>(
                    banker.RemainLists[i].RectTransform.anchoredPosition,
                    banker.GetTargetAnchoredPos(i)));
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
