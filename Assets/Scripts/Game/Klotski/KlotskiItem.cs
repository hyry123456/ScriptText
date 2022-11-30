using UnityEngine;
using UI;

[System.Serializable]
/// <summary>
/// 华容道中的单个物体，用来判断是否被点击，
/// 以及传递数据给控制判断是否可以移动
/// </summary>
public class KlotskiItem : MonoBehaviour, IPointDrag,
    IPointRelease, IPointClick
{
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer Render
    {
        get
        {
            if(spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            return spriteRenderer;
        }
    }
    public BoxCollider2D collider;
    private KlotskiControl control;
    public KlotskiControl Control
    {
        set { 
            control = value;
            int gridCount = control.GridCount;
            Vector2Int texSize = new Vector2Int(
                Render.sprite.texture.height / gridCount, 
                Render.sprite.texture.width / gridCount);
            if(collider == null)
                collider = GetComponent<BoxCollider2D>();
            collider.size = new Vector2(texSize.x / 100.0f, texSize.y / 100.0f);
        }
    }

    [SerializeField]
    int index, value;
    public int Index
    {
        get{ return index; }
        set{ index = value; }
    }
    public int Value
    {
        get{ return value; }
        set { this.value = value; }
    }

    private void Start()
    {
        if(collider == null)
            collider = GetComponent<BoxCollider2D>();
        WorldUIEventSystem.Instance.Register(GetComponent<IPointDrag>());
        WorldUIEventSystem.Instance.Register(GetComponent<IPointRelease>());
        WorldUIEventSystem.Instance.Register(GetComponent<IPointClick>());
    }

    private void OnDestroy()
    {
        WorldUIEventSystem worldUIEvent = WorldUIEventSystem.Instance;
        if (worldUIEvent == null) return;
        worldUIEvent.Logout(GetComponent<IPointRelease>());
        worldUIEvent.Logout(GetComponent<IPointDrag>());
        worldUIEvent.Register(GetComponent<IPointClick>());
    }

    public void OnDrag(PointData pointData)
    {
        control.DragItem(pointData.offset, this);
    }

    public Collider2D GetCollider()
    {
        return collider;
    }

    Vector2 target, begin;
    float time;

    public void OnRelease(PointData pointData)
    {
        control.CheckIndex(this);
        target = control.GetTargetPos(Index);
        begin = transform.localPosition;
        time = 0;
        Common.SustainCoroutine.Instance.AddCoroutine(AutoMove);
    }


    public bool AutoMove()
    {
        time += Time.deltaTime;
        if(time > 1.0f)
        {
            MoveByRadio(1);
            return true;
        }
        MoveByRadio(time);
        return false;
    }

    private void MoveByRadio(float radio01)
    {
        transform.localPosition =
            Vector2.Lerp(begin, target, radio01);
    }

    public void OnClick(PointData pointData)
    {
        Common.SustainCoroutine.Instance.RemoveCoroutine(AutoMove);
    }

}
