using UnityEngine;

namespace UI
{
    public struct PointData
    {
        public Vector2 screenPos;
        public Vector2 offset;
    }

    public interface IPointClick
    {
        public void OnClick(PointData pointData);
        public Collider2D GetCollider();
    }

    public interface IPointDrag
    {
        public void OnDrag(PointData pointData);
        public Collider2D GetCollider();
    }

    public interface IPointRelease
    {
        public void OnRelease(PointData pointData);
        public Collider2D GetCollider();
    }
}