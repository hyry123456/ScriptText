using UnityEngine;
using System.Collections.Generic;
using Common;

/// <summary>/// 华容道的控制类/// </summary>
public class KlotskiControl : MonoBehaviour
{
    private static KlotskiControl instance;
    public static KlotskiControl Instance => instance;
    [SerializeField]
    List<List<KlotskiItem>> klotskiItems;
    /// <summary> /// 创建时根据的物体 /// </summary>
    public GameObject originItem;
    public Vector2 gridSize = Vector2.one;
    int gridCount;
    public int GridCount => gridCount;

    /// <summary>/// 在每次启动时注册，一个场景只放一个就够了 /// </summary>
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private Object[] objects;
    float time;

    //逐渐显示9宫格
    bool Show()
    {
        time += Time.deltaTime;
        if (time > 2.0f)
        {
            SetItemsColor(Color.white);
        }
        Color color = Color.white * (time / 2.0f);
        SetItemsColor(color);
        return false;
    }

    void SetItemsColor(Color color)
    {
        for (int i = 0; i < klotskiItems.Count; i++)
        {
            for (int j = 0; j < klotskiItems[i].Count; j++)
            {
                if (klotskiItems[i][j] != null)
                    klotskiItems[i][j].Render.color = color;
            }
        }
    }


    /// <summary>
    /// 根据传入的文件路径读取图片后启动华容道游戏
    /// </summary>
    public void BeginGame(string texPath)
    {
        objects = Resources.LoadAll<Object>(texPath);
        int gridCount = (int)Mathf.Sqrt(objects.Length - 1);
        klotskiItems = new List<List<KlotskiItem>>(gridCount);
        Color begCol = Color.white;
        begCol.a = 0;
        //PoolingList<int> indexRange = new PoolingList<int>(gridCount * gridCount);
        //for(int i = 0; i < gridCount * gridCount; i++)
        //    indexRange.Add(i);
        List<int> value = new List<int>{ 1, 0, 2, 3, 4, 5, 6, 7, 8 };
        //抽一个删除
        int cloumn = Random.Range(0, gridCount);
        int row = Random.Range(0, gridCount);
        this.gridCount = gridCount;
        for (int i = 0; i < gridCount; i++)
        {
            List<KlotskiItem> itemList = new List<KlotskiItem>(gridCount);
            for(int j = 0; j < gridCount; j++)
            {
                int index = i * gridCount + j;
                if (value[index] == 0)
                {
                    itemList.Add(null);
                }
                //if(i == cloumn && j == row)
                //{
                //itemList.Add(null);
                //}
                else
                {
                    GameObject item = GameObject.Instantiate(originItem);
                    item.transform.parent = transform;
                    item.transform.localPosition = GetTargetPos(index);
                    KlotskiItem klotskiItem = item.AddComponent<KlotskiItem>();
                    klotskiItem.Render.color = begCol;
                    klotskiItem.Render.sprite = (Sprite)objects[value[index] + 1];
                    //int index = Random.Range(0, indexRange.Count);
                    //klotskiItem.Render.sprite = 
                    //    (Sprite)objects[indexRange.GetValue(index) + 1];
                    //klotskiItem.Index = i * gridCount + j;
                    klotskiItem.Index = index;
                    //klotskiItem.Value = indexRange.GetValue(index);
                    klotskiItem.Value = value[index];
                    klotskiItem.Control = this;
                    //indexRange.RemoveAt(index);
                    itemList.Add(klotskiItem);
                }

            }
            klotskiItems.Add(itemList);
        }
        time = 0;

        SustainCoroutine.Instance.AddCoroutine(Show);
    }

    public void DragItem(Vector2 offset, KlotskiItem item)
    {
        Vector4 range = GetMoveRange(item.Index);
        Vector3 target = item.transform.localPosition;
        target.x += offset.x;
        target.y += offset.y;
        target.x = Mathf.Clamp(target.x, range.x, range.z);
        target.y = Mathf.Clamp(target.y, range.y, range.w);
        item.transform.localPosition = target;
    }

    /// <summary>
    /// 获得该标签的棋牌的
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector4 GetMoveRange(int index)
    {
        int row = index / klotskiItems.Count;
        int column = index % klotskiItems.Count;
        Vector2 begin = GetTargetPos(index);
        Vector2 end = begin;
        int[] ans = { -1, 0, 1, 0, -1 };
        for(int i = 1; i < 5; i++)
        {
            int col = column + ans[i - 1], ro = row + ans[i];
            if (col >= 0 && col < klotskiItems.Count
                && ro >= 0 && ro < klotskiItems.Count)
            {
                if (klotskiItems[ro][col] == null)
                {
                    end = GetTargetPos(ro * gridCount + col);
                    break;
                }
            }
        }

        return new Vector4(
            Mathf.Min(begin.x, end.x),
            Mathf.Min(begin.y, end.y),
            Mathf.Max(begin.x, end.x),
            Mathf.Max(begin.y, end.y));
    }

    public Vector2 GetTargetPos(int index)
    {
        int column = index % gridCount;
        int row = index / gridCount;
        int addCount = gridCount / 2;
        return new Vector2(column * gridSize.x
            - gridSize.x * addCount, 
            -row * gridSize.y + gridSize.y * addCount);
    }

    public int PositionToIndex(Vector2 localPos)
    {
        int row = (int)(-localPos.y / gridSize.y);
        int col = (int)(localPos.x / gridSize.x);
        row += gridCount / 2;
        col += gridCount / 2;
        return row * gridCount + col;
    }

    public void CheckIndex(KlotskiItem item)
    {
        Vector3 pos = item.transform.localPosition;
        int newIndex = PositionToIndex(pos);
        int row = newIndex / gridCount, col = newIndex % gridCount;
        if(newIndex != item.Index)
        {
            int oldRow = item.Index / gridCount;
            int oldCol = item.Index % gridCount;
            KlotskiItem tem = klotskiItems[oldRow][oldCol];
            klotskiItems[oldRow][oldCol] = null;
            klotskiItems[row][col] = tem;
            item.Index = newIndex;
        }
        if (Check())
        {
            Debug.Log("游戏结束");
        }
    }

    private bool Check()
    {
        int current = 0;
        for (int i = 1; i < GridCount; i++)
        {
            if (klotskiItems[0][i] == null)
            {
                current++;
                continue;
            }
            if (klotskiItems[0][i].Value == current + 1)
                current++;
            else
                return false;
        }
        for (int i = 1; i < GridCount; i++)
        {
            for(int j = 0; j < GridCount; j++)
            {
                if (klotskiItems[i][j] == null)
                {
                    current++;
                    continue;
                }
                if (klotskiItems[i][j].Value == current + 1)
                    current++;
                else
                    return false;
            }
        }
        return true;
    }
}
