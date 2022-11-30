using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    /// <summary>  /// 测试用的世界UI  /// </summary>
    public class SimpleWorldUI : MonoBehaviour, IPointClick,
        IPointDrag, IPointRelease
    {
        Collider2D collider;
        private void Start()
        {
            collider = GetComponent<Collider2D>();
            WorldUIEventSystem.Instance.Register(GetComponent<IPointClick>());
            WorldUIEventSystem.Instance.Register(GetComponent<IPointDrag>());
            WorldUIEventSystem.Instance.Register(GetComponent<IPointRelease>());
        }

        public Collider2D GetCollider()
        {
            return collider;
        }

        public void OnClick(PointData pointData)
        {
            Debug.Log("点击");
        }

        public void OnDrag(PointData pointData)
        {
            Debug.Log("拖拽中");
        }

        public void OnRelease(PointData pointData)
        {
            Debug.Log("释放");
        }
    }
}