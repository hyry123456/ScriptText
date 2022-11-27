using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ������ѣ��������߿������������
/// </summary>
public class ClickRemain : MonoBehaviour, IPointerClickHandler
{
    private DrawGhostShow origin;
    public DrawGhostShow Origin
    {
        set { origin = value; }
    }

    private Image image;
    public Image Image
    {
        set { image = value; }
        get { 
            if(image == null)
                image = GetComponent<Image>();
            return image; 
        }
    }
    private RectTransform rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if(rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    private int index;
    public int Index
    {
        get { return index; }
        set { index = value; }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //��ʾΪ�л�����ǰ����
        origin.OnChangeChoose(this);
    }
}
