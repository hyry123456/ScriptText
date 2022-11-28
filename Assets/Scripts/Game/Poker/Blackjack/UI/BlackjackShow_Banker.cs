using Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackShow_Banker : MonoBehaviour
{
    private BlackjackControl control;
    private Banker banker;

    public BlackjackControl Control
    {
        set { control = value; }
    }
    public Banker Banker
    {
        set { banker = value; }
    }
    protected virtual void Start()
    {
    }

    private List<Pair<int, Image>> sprites =
        new List<Pair<int, Image>>();
    private Image white;
    private Image black;

    /// <summary> /// 父类只实现自动更新图片 /// </summary>
    protected virtual void Update()
    {
        //更新
        if (banker.Cards.Count != sprites.Count)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].Key != banker.Cards[i])
                {
                    //更换图片
                    sprites[i].Value.sprite = control.GetPoker(banker.Cards[i]);
                }
            }

            //添加图片
            for (int i = sprites.Count; i < banker.Cards.Count; i++)
            {
                GameObject pocker = GameObject.Instantiate(control.PockerItem);
                pocker.transform.parent = transform;        //设置为子物体
                Image image = pocker.GetComponent<Image>();
                image.sprite = control.GetPoker(banker.Cards[i]);
                sprites.Add(new Pair<int, Image>(banker.Cards[i], image));
            }
        }

        if(white == null)
        {
            GameObject pocker = GameObject.Instantiate(control.PockerItem);
            pocker.transform.parent = transform;        //设置为子物体
            Image image = pocker.GetComponent<Image>();
            image.sprite = control.GetPoker(banker.WhiteCard);
            white = image;
        }
        if(banker.ShowBlack && black == null)
        {
            GameObject pocker = GameObject.Instantiate(control.PockerItem);
            pocker.transform.parent = transform;        //设置为子物体
            Image image = pocker.GetComponent<Image>();
            image.sprite = control.GetPoker(banker.BlackCard);
            black = image;
        }
    }
}
