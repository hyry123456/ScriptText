using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Turntable : MonoBehaviour, IDragHandler
{
    public CircleCollider2D collider2D;

    private void Start()
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(name);
    }


}
