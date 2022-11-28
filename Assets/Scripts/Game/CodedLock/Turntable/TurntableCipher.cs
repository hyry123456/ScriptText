using UnityEngine;
using UnityEngine.EventSystems;

public class TurntableCipher : MonoBehaviour, 
    ICanvasRaycastFilter
{
    public float Circle0 = 90;
    public float Circle1 = 70;
    public float Circle2 = 50;
    public GameObject circle0;
    public GameObject circle1;
    public GameObject circle2;

    public GameObject currentCircle;
    GameObject nearCircle;

    private RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public bool isLock = false;
    private Vector2 preMove;

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        Vector2 local;
        //获得鼠标位置
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
            sp, eventCamera, out local);

        Contain(local);     //判断选中的球体顺便

        if (isLock)     //锁定中，如果没有按下就停止锁定
        {
            if(!Input.GetKey(KeyCode.Mouse0))
            {
                nearCircle = currentCircle;
                Common.SustainCoroutine.Instance.AddCoroutine(NearIntergate);
                isLock = false; return false;
            }
            else
            {
                Common.SustainCoroutine.Instance.RemoveCoroutine(NearIntergate);
                //根据上一帧位置判断偏移值
                Vector3 elur = currentCircle.transform.eulerAngles;
                float max;
                Vector2 offset = preMove - local;
                if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
                    max = offset.x;
                else
                    max = -offset.y;
                elur.z += 500.0f * Time.deltaTime * max;
                elur.x = elur.y = 0;
                currentCircle.transform.rotation = Quaternion.Euler(elur);
                preMove = local;
            }
        }
        else       //没有锁定，判断是否需要锁定
        {
            if (currentCircle == null) return false;     //没有选择直接退出

            if (Input.GetKey(KeyCode.Mouse0))       //开启锁定
            {
                isLock = true;
                preMove = local; 
            }
        }

        return false;
    }

    public float distance;

    void Contain(Vector2 local)
    {
        if (isLock) return;
        distance = local.magnitude;
        if (distance < Circle0 && distance > Circle1)
        {
            currentCircle = circle0;    //最外部的
            return;
        }
        else if(distance < Circle1 && distance > Circle2)
        {
            currentCircle = circle1;    //最外部的
            return;
        }
        else if(distance < Circle2)
        {
            currentCircle = circle2;    //最外部的
            return;
        }
        currentCircle = null;
        return;
    }



    private bool NearIntergate()
    {
        Vector3 elur = nearCircle.transform.eulerAngles;
        float floor = Mathf.Floor(elur.z / 36.0f);
        float target = floor;
        target = target * 36.0f;
        if(Mathf.Abs(elur.z - target) < 0.5f)
        {
            elur.z = target;
            nearCircle.transform.eulerAngles = elur;
            return true;
        }
        elur.z = Mathf.Lerp(elur.z, target, Time.deltaTime * 3);
        nearCircle.transform.eulerAngles = elur;
        return false;
    }

}
