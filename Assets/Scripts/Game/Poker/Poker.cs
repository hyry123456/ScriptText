using UnityEngine;
using Common;

/// <summary>
/// 扑克牌基类，用来定义通用的获取生成扑克牌，获取扑克牌，
/// 得到具体扑克牌名称的方法
/// </summary>
public class Poker
{
    /// <summary>
    /// 分为4组，每组13个牌，组名为：黑桃，红桃，梅花，方块
    /// 具体每组有13个牌A，2之类的，然后如果需要会有52小王，53大王
    /// </summary>
    private PoolingList<int> cards;
    /// <summary>
    /// 初始化游戏，初始化牌数
    /// </summary>
    /// <param name="smallKing">是否有小王</param>
    /// <param name="bigKing">是否有大王</param>
    protected void InitilizePoker(bool smallKing, bool bigKing)
    {
        cards = new PoolingList<int>(54);
        //赋值牌号，方便之后抽取
        for (int i = 0; i < 52; i++)
        {
            cards.Add(i);
        }
        if (smallKing) cards.Add(52);
        if (bigKing) cards.Add(53);

    }

    /// <summary>
    /// 抽取一张排，返回值是一个int编号，需要进行取余求偏移得到具体是哪张排
    /// </summary>
    /// <returns></returns>
    protected int GetCard()
    {
        int index = Random.Range(0, cards.Count);
        int re = cards.GetValue(index);
        cards.RemoveIndex(index);
        return re;
    }

    /// <summary>/// 根据牌的编号转化为名称，测试用的/// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string GetCardName(int index)
    {
        int group = index / 13;
        int offset = index % 13;
        if(group >= 4)
        {
            if (index == 52) return "小王";
            else return "大王";
        }
        string groupName = "";
        if (group == 0) groupName = "黑桃";
        else if (group == 1) groupName = "红桃";
        else if (group == 2) groupName = "梅花";
        else if (group == 3) groupName = "方格";
        else
            Debug.LogError(group);

        switch (offset)
        {
            case 0:
                groupName += "A";
                break;
            case 10:
                groupName += "J";
                break;
            case 11:
                groupName += "Q";
                break;
            case 12:
                groupName += "K";
                break;
            default:
                groupName += (offset + 1).ToString();
                break;
        }
        return groupName;
    }
}
