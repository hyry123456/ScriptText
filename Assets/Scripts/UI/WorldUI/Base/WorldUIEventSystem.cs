using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 直接坐标的UI时间侦测类，用来注册以及侦测所有UI时间
    /// </summary>
    public class WorldUIEventSystem : MonoBehaviour
    {
        private static WorldUIEventSystem instance;
        private static bool ApplicanIsExit = false;
        public static WorldUIEventSystem Instance
        {
            get
            {
                if (ApplicanIsExit)
                    return instance;
                if (instance == null)
                {
                    GameObject go = new GameObject("WorldUIEvent");
                    go.AddComponent<WorldUIEventSystem>();
                }
                return instance;
            }
        }
        Camera _camera;

        List<IPointClick> pointClicks = new List<IPointClick>();
        List<IPointDrag> pointDrags = new List<IPointDrag>();
        List<IPointRelease> pointReleases = new List<IPointRelease>();

        PointData pointData = new PointData();

        [SerializeField]
        IPointDrag draging;
        [SerializeField]
        IPointRelease release;
        Vector2 preScreenPoint;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            ApplicanIsExit = false;
            instance = this;
            _camera = Camera.main;
        }

        private void OnDestroy()
        {
            ApplicanIsExit = true;
            instance = null;
            pointReleases.Clear();
            pointClicks.Clear();
            pointDrags.Clear();
        }

        private void Update()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null) return;
            }
            Vector3 mouseWorld = Input.mousePosition;
            pointData.screenPos = Input.mousePosition;

            float cameraZ = _camera.transform.position.z;

            if (draging == null)
            {
                //按下时侦测点击，同时判断下是否有拖拽物体被检查到
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //Debug.Log(worldMousePosition);
                    for (int i = 0; i < pointClicks.Count; i++)
                    {
                        Collider2D collider = pointClicks[i].GetCollider();
                        mouseWorld.z = collider.transform.position.z - cameraZ;
                        Vector3 worldMousePosition =
                            _camera.ScreenToWorldPoint(mouseWorld);
                        if (collider.OverlapPoint(worldMousePosition))
                        {
                            pointData.offset = Vector2.zero;
                            pointClicks[i].OnClick(pointData);
                            break;  //只检测第一个
                        }
                    }

                    for (int i = 0; i < pointDrags.Count; i++)
                    {
                        Collider2D collider = pointDrags[i].GetCollider();
                        mouseWorld.z = collider.transform.position.z - cameraZ;
                        Vector3 worldMousePosition =
                            _camera.ScreenToWorldPoint(mouseWorld);
                        if (collider.OverlapPoint(worldMousePosition))
                        {
                            pointData.offset = Vector2.zero;
                            draging = pointDrags[i];
                            break;     //找到被拖拽的物体后直接退出
                        }
                    }
                    for (int i = 0; i < pointReleases.Count; i++)
                    {
                        Collider2D collider = pointReleases[i].GetCollider();
                        mouseWorld.z = collider.transform.position.z - cameraZ;
                        Vector3 worldMousePosition =
                            _camera.ScreenToWorldPoint(mouseWorld);
                        if (collider.OverlapPoint(worldMousePosition))
                        {
                            release = pointReleases[i];
                            break;     //找到被拖拽的物体后直接退出
                        }
                    }
                }
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (release != null)
                    {
                        release.OnRelease(pointData);
                        release = null;
                    }
                    draging = null;
                }
                else
                {
                    Collider2D collider = draging.GetCollider();
                    mouseWorld.z = collider.transform.position.z - cameraZ;
                    Vector3 worldMousePosition =
                        _camera.ScreenToWorldPoint(mouseWorld);
                    Vector3 temPre = new Vector3(preScreenPoint.x,
                        preScreenPoint.y, mouseWorld.z);
                    temPre = _camera.ScreenToWorldPoint(temPre);
                    pointData.offset = worldMousePosition - temPre;
                    draging.OnDrag(pointData);
                }
            }
            preScreenPoint = Input.mousePosition;
        }

        public void Register(IPointClick pointClick)
        {
            pointClicks.Add(pointClick);
        }
        public void Register(IPointDrag pointDrag)
        {
            pointDrags.Add(pointDrag);
        }
        public void Register(IPointRelease pointRelease)
        {
            pointReleases.Add(pointRelease);
        }

        public void Logout(IPointClick pointClick)
        {
            pointClicks.Remove(pointClick);
        }

        public void Logout(IPointDrag pointDrag)
        {
            pointDrags.Remove(pointDrag);
        }
        public void Logout(IPointRelease pointRelease)
        {
            pointReleases.Remove(pointRelease);
        }
    }
}