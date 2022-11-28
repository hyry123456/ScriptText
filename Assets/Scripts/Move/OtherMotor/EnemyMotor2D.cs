using UnityEngine;

namespace Motor
{
    public class EnemyMotor2D : MotorBase
    {
        [SerializeField]
        /// <summary>    /// ��ǰ�ٶ�, �����ٶ�, ����������ٶ�    /// </summary>
        Vector3 velocity, desiredVelocity, connectionVelocity;
        /// <summary>    /// ���ٶ�    /// </summary>
        public float groundAcceleration = 10f, airAcceleration = 5;

        Rigidbody body;
        GameObject connectObj, preConnectObj;

        /// <summary>    /// �Ƿ��ڵ�����    /// </summary>
        private bool onGround = false;

        /// <summary>        /// ���������б�нǣ��Լ�¥����б�н�        /// </summary>
        [Range(0, 90)]
        public float maxGroundAngle = 25f, maxStairAngle = 25;
        private float minGroundDot = 0, minStairsDot = 0;

        /// <summary>
        /// �Ӵ���ķ��ߣ����������ƽ�����ߣ�����ȷ���ƶ���ķ���
        /// </summary>
        Vector3 contactNormal;

        /// <summary>    /// ���ؼ��Ĳ㣬�Լ�¥�ݼ���    /// </summary>
        [SerializeField]
        LayerMask stairsMask = -1;

        Info.CharacterInfo characterInfo;
        Vector3 connectionWorldPostion;

        /// <summary>  /// ���ƽ�ɫ��z�ռ�λ��  /// </summary>
        [SerializeField]
        private float ClampZ = 0;

        int groundCount = 0;

        void Awake()
        {
            velocity = Vector3.zero;
            body = GetComponent<Rigidbody>();
            minGroundDot = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairsDot = Mathf.Cos(maxStairAngle * Mathf.Deg2Rad);
            characterInfo = GetComponent<Info.CharacterInfo>();
            if (characterInfo == null) Debug.LogError("��ɫ��ϢΪ��");
        }


        private void FixedUpdate()
        {
            //�������ݣ���������һ������֡�����ݽ��и���֮���
            UpdateState();

            //ȷ���ڿ��л����ڵ���
            AdjustVelocity();
            if(isAutoRotate)
                AutoRotate();
            //velocity = Vector3.ClampMagnitude(velocity, 40);

            Vector3 temp = transform.position;
            temp.z = ClampZ; transform.position = temp;

            body.velocity = velocity;
            ClearState();
        }

        void UpdateState()
        {
            velocity = body.velocity;
            //�����ڵ���ʱִ���������淽��
            if (onGround/*�ڵ���*/)
            {
                contactNormal.Normalize();
            }
            else
                contactNormal = Vector3.up;

            if (connectObj && connectObj.tag == "CheckMove")
            {
                UpdateConnectionState();
            }
        }

        void UpdateConnectionState()
        {
            //ֻ��������ͬ�����б�Ҫ����
            if (connectObj == preConnectObj)
            {
                Vector3 connectionMovment =
                    connectObj.transform.position - connectionWorldPostion;
                connectionVelocity = connectionMovment / Time.deltaTime;
            }
            connectionWorldPostion = connectObj.transform.position;
        }

        private void OnCollisionExit(Collision collision)
        {
            EvaluateCollision(collision);
        }


        private void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void EvaluateCollision(Collision collision)
        {
            float minDot = GetMinDot(collision.gameObject.layer);
            if (collision.contactCount > 0)
                groundCount = 1;
            else groundCount = 0;
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                float upDot = Vector3.Dot(Vector3.up, normal);
                if (upDot >= minDot)
                {
                    onGround = true;
                    //��֤����ж���Ӵ���ʱ�ܹ���ȷ�Ļ�ȡ����
                    contactNormal += normal;
                    connectObj = collision.gameObject;
                }
                //�������ƶ����ƣ����Ǳ��⳹�׵Ĵ�ֱ��
                else if (upDot > -0.01f)
                {
                    connectObj = collision.gameObject;
                }

            }
        }


        /// <summary>
        /// �����ƶ�����������֤�ƶ��ķ���������ƽ���
        /// </summary>
        void AdjustVelocity()
        {
            //��Ϊ�ٶ��õ�Ҳ���������꣬����ƶ�ʱͶӰҲ���������������꣬����right����x�ᣬfoward����Y��
            //��1��0��0ͶӰ���Ӵ�ƽ���ϣ�
            Vector3 xAixs = ProjectDirectionOnPlane(Vector3.right, contactNormal);

            Vector3 relativeVelocity = velocity - connectionVelocity;
            //ȷ��ʵ���������ƽ���ϵ�X�ƶ�ֵ
            float currentX = Vector3.Dot(relativeVelocity, xAixs);

            float acceleration = onGround ? groundAcceleration : airAcceleration;
            float maxSpeedChange = acceleration * Time.fixedDeltaTime;

            //ȷ�����������õ����ƶ�ֵ
            float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);

            //�ƶ�Ҫ�������ƽ��ķ������ƶ�����˸���ʵ��ֵ������ֵ�Ĳ�ȷ��Ҫ���ӵ��ٶȴ�С��
            //Ȼ�����ͶӰ���������Xֵ�Լ�Zֵȷ�������ƶ�ֵ
            velocity += xAixs * (newX - currentX);
        }

        /// <summary>
        /// ������ݣ���һЩ���ݹ�Ϊ��ʼ��
        /// </summary>
        void ClearState()
        {
            onGround = false;
            contactNormal = connectionVelocity = Vector3.zero;
            preConnectObj = connectObj;
            connectObj = null;
        }

        float GetMinDot(int layer)
        {
            //�ж���¥�ݻ�����������
            return (stairsMask & (1 << layer)) == 0 ?
                minGroundDot : minStairsDot;
        }


        /// <summary>    /// ȷ���÷���ͶӰ����ƽ���ϵķ���ֵ�����й���׼����    /// </summary>
        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }

        [SerializeField]
        bool isAutoRotate = true;

        /// <summary>    /// �Զ�����ģ���ƶ�������ת�Ƕ�     /// </summary>
        void AutoRotate()
        {
            Vector3 veloiryDir = -velocity;
            veloiryDir.y = 0; veloiryDir.z = 0;
            //��һ���ٶȲŻ���ת
            if (Mathf.Abs( veloiryDir.x) > 1f)
            {
                transform.right = veloiryDir.x > 0 ? Vector3.left : Vector3.right;
            }
        }


        public override void Move(float horizontal, float vertical)
        {
            //ʵ���Ͼ��Ǽ��ٶ��Ƕ�ֵ����ɫ����ֵ��Ŀ���ٶȣ���֡���ٶȱ仯ΪĿ���ٶ�
            Vector2 playInput = new Vector2(vertical, horizontal);
            playInput = Vector2.ClampMagnitude(playInput, 1);
            //����ɴ�����ƶ��ռ�͸�������ռ����ֵ�ƶ�����Ȼ��ֱ�������������ƶ�
            desiredVelocity = Vector3.forward * playInput.x + Vector3.right * playInput.y;
            desiredVelocity = desiredVelocity * characterInfo.RunSpeed;
        }

        public override void DesireJump()
        {

        }

        public override bool OnGround()
        {
            return groundCount > 0;
        }

        public override void BeginAutoRotate()
        {
            isAutoRotate = true;
        }

        public override void EndAutoRotate()
        {
            isAutoRotate = false;
        }
    }
}