using UnityEngine;

/// <summary>
/// 21�����Ϸ�����࣬����UI����ʾ�Լ���Ϸ�Ŀ���
/// </summary>
public class BlackjackControl : MonoBehaviour
{
    private Object[] sprits;
    [SerializeField]
    private GameObject pockerItem;
    public GameObject PockerItem => pockerItem;


    /// <summary>
    /// ���ݱ�Ŷ�ȡ����ͼƬ
    /// </summary>
    /// <param name="index">���Ʊ��</param>
    public Sprite GetPoker(int index)
    {
        int group = index / 13;
        int offset = index % 13;
        Object tex;     //��Ҫע�⣬��ȡ��sprites�е�һ�����ݲ���Sprite����Ҫ��1
        if (offset == 0)
            tex = sprits[group * 13 + 13 + 1];
        else
            tex = sprits[group * 13 + offset + 1];
        return (Sprite)tex;
    }

    Blackjack blackjack;    //21����Ϸ��

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
