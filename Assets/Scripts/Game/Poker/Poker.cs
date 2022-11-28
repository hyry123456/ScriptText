using UnityEngine;
using Common;

/// <summary>
/// �˿��ƻ��࣬��������ͨ�õĻ�ȡ�����˿��ƣ���ȡ�˿��ƣ�
/// �õ������˿������Ƶķ���
/// </summary>
public class Poker
{
    /// <summary>
    /// ��Ϊ4�飬ÿ��13���ƣ�����Ϊ�����ң����ң�÷��������
    /// ����ÿ����13����A��2֮��ģ�Ȼ�������Ҫ����52С����53����
    /// </summary>
    private PoolingList<int> cards;
    /// <summary>
    /// ��ʼ����Ϸ����ʼ������
    /// </summary>
    /// <param name="smallKing">�Ƿ���С��</param>
    /// <param name="bigKing">�Ƿ��д���</param>
    protected void InitilizePoker(bool smallKing, bool bigKing)
    {
        cards = new PoolingList<int>(54);
        //��ֵ�ƺţ�����֮���ȡ
        for (int i = 0; i < 52; i++)
        {
            cards.Add(i);
        }
        if (smallKing) cards.Add(52);
        if (bigKing) cards.Add(53);

    }

    /// <summary>
    /// ��ȡһ���ţ�����ֵ��һ��int��ţ���Ҫ����ȡ����ƫ�Ƶõ�������������
    /// </summary>
    /// <returns></returns>
    protected int GetCard()
    {
        int index = Random.Range(0, cards.Count);
        int re = cards.GetValue(index);
        cards.RemoveIndex(index);
        return re;
    }

    /// <summary>/// �����Ƶı��ת��Ϊ���ƣ������õ�/// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string GetCardName(int index)
    {
        int group = index / 13;
        int offset = index % 13;
        if(group >= 4)
        {
            if (index == 52) return "С��";
            else return "����";
        }
        string groupName = "";
        if (group == 0) groupName = "����";
        else if (group == 1) groupName = "����";
        else if (group == 2) groupName = "÷��";
        else if (group == 3) groupName = "����";
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
