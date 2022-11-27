using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;

/// <summary>
/// 用来显示21点的棋牌，挂在一个Layout上，显示分配给他的棋牌,
/// 由于21点只有加牌操作，只需要自动更新牌即可
/// </summary>
public abstract class BlackjackShow : MonoBehaviour
{
    private BlackjackControl control;
    private Blackjack_User user;

    public BlackjackControl Control
    {
        set { control = value; }
    }
    public Blackjack_User User
    {
        set { user = value; }
    }
    protected virtual void Start()
    {
    }

    private List<Pair<int, Image>> sprites = 
        new List<Pair<int, Image>>();

    /// <summary> /// 父类只实现自动更新图片 /// </summary>
    protected virtual void Update()
    {
        //更新
        if(user.Cards.Count != sprites.Count)
        {
            for(int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].Key != user.Cards[i])
                {
                    //更换图片
                    sprites[i].Value.sprite = control.GetPoker(user.Cards[i]);
                }
            }

            //添加图片
            for (int i = sprites.Count; i < user.Cards.Count; i++)
            {
                GameObject pocker = GameObject.Instantiate(control.PockerItem);
                pocker.transform.parent = transform;        //设置为子物体
                Image image = pocker.GetComponent<Image>();
                image.sprite = control.GetPoker(user.Cards[i]);
                sprites.Add(new Pair<int, Image>(user.Cards[i], image));
            }
        }
    }
}
