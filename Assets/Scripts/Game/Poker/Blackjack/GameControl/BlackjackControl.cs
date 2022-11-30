using UnityEngine;

/// <summary>
/// 21点的游戏控制类，决定UI的显示以及游戏的开启
/// </summary>
public class BlackjackControl : MonoBehaviour
{
    private Object[] sprits;
    [SerializeField]
    private GameObject pockerItem;
    public GameObject PockerItem => pockerItem;


    /// <summary>
    /// 根据编号读取棋牌图片
    /// </summary>
    /// <param name="index">棋牌编号</param>
    public Sprite GetPoker(int index)
    {
        int group = index / 13;
        int offset = index % 13;
        Object tex;     //需要注意，读取的sprites中第一个数据不是Sprite，故要加1
        if (offset == 0)
            tex = sprits[group * 13 + 13 + 1];
        else
            tex = sprits[group * 13 + offset + 1];
        return (Sprite)tex;
    }

    Blackjack blackjack;    //21点游戏类

    private void Start()
    {
        sprits = Resources.LoadAll<Object>("UI/Pocker");
        blackjack = new Blackjack();
        BlackjackShow_AI[] blackjacks = GetComponentsInChildren<BlackjackShow_AI>();
        BlackjackShow_Player player = GetComponentInChildren<BlackjackShow_Player>();
        BlackjackShow_Banker banker = GetComponentInChildren<BlackjackShow_Banker>();
        blackjack.InitilizeGame(blackjacks.Length);
        player.Control = this;
        player.User = blackjack.Users[0];

        banker.Control = this;
        banker.Banker = blackjack.Banker;

        for (int i = 0; i < blackjacks.Length; i++)
        {
            blackjacks[i].User = blackjack.Users[i + 1];
            blackjacks[i].Control = this;
        }


    }
}
