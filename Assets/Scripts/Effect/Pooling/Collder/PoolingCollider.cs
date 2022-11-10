using UnityEngine;

namespace Common
{
    public delegate void CollisionEnterEvent(Collider collider);

    /// <summary> /// �ػ���������ײ��   /// </summary>
    class PoolingSphere : ObjectPoolBase
    {
        private CollisionEnterEvent enterEvent;
        protected SphereCollider sphere;

        protected override void OnEnable()
        {
            if (sphere == null)
                sphere = GetComponent<SphereCollider>();
        }

        /// <summary>  /// ��ʼ��������ײ�������������꣬��Ϊ�������ⲿ����     /// </summary>
        /// <param name="radius">����Ƕ�</param>
        /// <param name="enterEvent">ִ�е���ײ�¼�</param>
        public void InitializeSphere(float radius, CollisionEnterEvent enterEvent)
        {
            sphere.radius = radius;
            this.enterEvent = enterEvent;
        }

        private void OnCollisionEnter(Collision collision)
        {

        }

        private void OnTriggerEnter(Collider collider)
        {
            if (enterEvent != null)
                enterEvent(collider);
        }

        public override void CloseObject()
        {
            base.CloseObject();
            enterEvent = null;      //����¼�
        }
    }

    /// <summary>  /// �ػ��ķ�����ײ�� /// </summary>
    class PoolingBox : ObjectPoolBase
    {
        private CollisionEnterEvent enterEvent;
        protected BoxCollider box;

        protected override void OnEnable()
        {
            if (box == null)
                box = GetComponent<BoxCollider>();
        }

        /// <summary>  
        /// ��ʼ��������ײ���������������Լ��Ƕȣ����ⲿ����
        /// </summary>
        /// <param name="size">xyz�Ĵ�С</param>
        /// <param name="enterEvent">ִ�е���ײ�¼�</param>
        public void InitializeBox(Vector3 size, CollisionEnterEvent enterEvent)
        {
            box.size = size;
            this.enterEvent = enterEvent;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (enterEvent != null)
                enterEvent(collider);
        }

        public override void CloseObject()
        {
            base.CloseObject();
            enterEvent = null;      //����¼�
        }
    }

    /// <summary>  /// Ϊ�˷���ػ�ʹ�ã�ʹ�ô��봴������ /// </summary>
    class PoolingCollisionOrigin
    {
        private static PoolingSphere poolingSphere;
        public static PoolingSphere PoolingSphereOrigin
        {
            get
            {
                //û�оʹ���һ��
                if(poolingSphere == null)
                {
                    GameObject game = new GameObject("PoolingSphere");
                    poolingSphere = game.AddComponent<PoolingSphere>();
                    SphereCollider sphere = game.AddComponent<SphereCollider>();
                    sphere.isTrigger = true;
                    game.layer = LayerMask.NameToLayer("Effect");
                    game.AddComponent<Rigidbody>();
                    GameObject.DontDestroyOnLoad(game);
                    game.SetActive(false);
                }
                return poolingSphere;
            }
        }

        private static PoolingBox poolingBox;
        /// <summary>
        /// �����ػ�������ײ��ĸ�������
        /// </summary>
        public static PoolingBox PoolingBoxOrigin
        {
            get
            {
                //û�оʹ���һ��
                if (poolingBox == null)
                {
                    GameObject game = new GameObject("PoolingBox");
                    poolingBox = game.AddComponent<PoolingBox>();
                    BoxCollider box = game.AddComponent<BoxCollider>();
                    box.isTrigger = true;
                    game.layer = LayerMask.NameToLayer("Effect");
                    game.AddComponent<Rigidbody>();
                    GameObject.DontDestroyOnLoad(game);
                    game.SetActive(false);
                }
                return poolingBox;
            }
        }
    }
}