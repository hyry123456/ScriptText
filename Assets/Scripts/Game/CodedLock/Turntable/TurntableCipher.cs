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
        //������λ��
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
            sp, eventCamera, out local);

        Contain(local);     //�ж�ѡ�е�����˳��

        if (isLock)     //�����У����û�а��¾�ֹͣ����
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
                //������һ֡λ���ж�ƫ��ֵ
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
        else       //û���������ж��Ƿ���Ҫ����
        {
            if (currentCircle == null) return false;     //û��ѡ��ֱ���˳�

            if (Input.GetKey(KeyCode.Mouse0))       //��������
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
            currentCircle = circle0;    //���ⲿ��
            return;
        }
        else if(distance < Circle1 && distance > Circle2)
        {
            currentCircle = circle1;    //���ⲿ��
            return;
        }
        else if(distance < Circle2)
        {
            currentCircle = circle2;    //���ⲿ��
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
