using UnityEngine;


namespace Motor
{

    public class Motor2D : MotorBase
    {
        //�����Ҫ���õ�����
        #region Outside SetData  

        [SerializeField]
        /// <summary>    /// ���ٶ�    /// </summary>
        private float groundAcceleration = 10f, airAcceleration = 5;

        [SerializeField]
        private float jumpHeight = 2f;
        [SerializeField]
        /// <summary>/// ��������Ծ������ע������ǿ�������������������Ծ/// </summary>
        private int maxAirJumps = 2;

        /// <summary>        /// ���������б�нǣ��Լ�¥����б�н�        /// </summary>
        [Range(0, 90), SerializeField]
        private float maxGroundAngle = 25f, maxStairAngle = 25;

        /// <summary> /// ��������ƫ����ֵ��x��ǰ�����ţ�y������ƫ��,z����ӵ����Ĵ�С /// </summary>
        [SerializeField]
        Vector3 climbData = Vector3.one;

        /// <summary>    /// �ж�ʱ��������ص��ٶȣ�����ٶȴ��ڸ�ֵ������������    /// </summary>
        [SerializeField, Range(0, 100f)]
        float maxSnapSpeed = 100f;
        /// <summary>    /// ���صļ�����    /// </summary>
        [SerializeField, Range(0, 10f)]
        float probeDistance = 3f;

        /// <summary>    /// ���ؼ��Ĳ㣬�Լ�¥�ݼ���    /// </summary>
        [SerializeField]
        LayerMask probeMask, stairsMask = -1;

        /// <summary>    /// ����ռ䣬�������ݸÿռ�������ģ���ƶ�    /// </summary>
        [SerializeField]
        Transform playerInputSpace;

        /// <summary>  /// ���ƽ�ɫ��z�ռ�λ��  /// </summary>
        [SerializeField]
        private float ClampZ = 0;

        #endregion

        //����ʱ��Ҫ������
        #region Compete Load Data

        Rigidbody body;
        GameObject connectObj, preConnectObj;
        private bool desiredJump = false;
        [SerializeField]
        /// <summary>    /// ��ǰ�ٶ�, �����ٶ�, ����������ٶ�    /// </summary>
        Vector3 velocity, desiredVelocity, connectionVelocity;

        private int airJumps = 0;       //��ǰ��������

        /// <summary>    /// �Ƿ��ڵ�����    /// </summary>
        private bool onGround = false;

        /// <summary> /// �������⣬������ﱻ���ڶ������У���Ҫ���м������ж��Ƿ�Ҫ��������Ծ/// </summary>
        private int steepContractCount = 0, groundCount = 0;
        /// <summary>    /// �ֶ�OnSteep��ȷ���Ƿ���б����    /// </summary>
        private bool OnSteep => steepContractCount > 0;

        /// <summary>
        /// �Ӵ���ķ��ߣ����������ƽ�����ߣ�����ȷ���ƶ���ķ����Լ���Ծ�ķ���
        /// </summary>
        Vector3 contactNormal, steepNormal;

        private float minGroundDot = 0, minStairsDot = 0;   //�Ƕ�ת��Ϊ�Ļ���ֵ

        /// <summary>
        /// ����ȷ����ʱ�뿪�����ʱ��(stepSinceLastGround)���ڵ���ʱ���Ϊ0��
        /// ����ʱ��������֡ˢ��
        /// </summary>
        int stepSinceLastGround = 0;
        /// <summary>    /// ����ȷ����Ծ��ʱ�䣬����Ծʱ����㣬������֡ʱ��֡����    /// </summary>
        int stepSinceLastJump = 0;

        Info.CharacterInfo characterInfo;       //��ɫ��Ϣ
        Vector3 connectionWorldPostion,         //��һ�����������λ��
            prePosition;        //��һ�ε�λ��

        #endregion

        //�ٶȲ���������ţ����ֵһ��ͻ��и�����ƶ�ֵ
        public override void Move(float horizontal, float vertical)
        {
            //ʵ���Ͼ��Ǽ��ٶ��Ƕ�ֵ����ɫ����ֵ��Ŀ���ٶȣ���֡���ٶȱ仯ΪĿ���ٶ�
            Vector2 playInput = new Vector2(vertical, horizontal);
            //playInput = Vector2.ClampMagnitude(playInput, 1);
            //����ɴ�����ƶ��ռ�͸�������ռ����ֵ�ƶ�����Ȼ��ֱ�������������ƶ�
            if (playerInputSpace)
                desiredVelocity = playerInputSpace.forward * playInput.x + playerInputSpace.right * playInput.y;
            else
                desiredVelocity = Vector3.forward * playInput.x + Vector3.right * playInput.y;
            desiredVelocity = desiredVelocity * characterInfo.RunSpeed;
        }

        public override void DesireJump()
        {
            desiredJump = true;
        }

        //��ʼ����Ҫ����
        void Awake()
        {
            velocity = Vector3.zero;
            body = GetComponent<Rigidbody>();
            minGroundDot = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairsDot = Mathf.Cos(maxStairAngle * Mathf.Deg2Rad);
            characterInfo = GetComponent<Info.CharacterInfo>();
            if (characterInfo == null) Debug.LogError("��ɫ��ϢΪ��");
            prePosition = transform.position;
        }
        private void FixedUpdate()
        {
            //�������ݣ���������һ������֡�����ݽ��и���֮���
            UpdateState();

            //ȷ���ڿ��л����ڵ���
            AdjustVelocity();
            ClimbCheck();
            if (desiredJump)
            {
                Jump();
                desiredJump = false;
            }
            AutoRotate();

            //velocity = Vector3.ClampMagnitude(velocity, 40);
            Vector3 temp = transform.position;
            temp.z = ClampZ; transform.position = temp;
            body.velocity = velocity;
            ClearState();
        }

        void UpdateState()
        {
            stepSinceLastGround += 1;
            stepSinceLastJump += 1;
            velocity = body.velocity;

            //�����ڵ���ʱִ���������淽��
            if (onGround/*�ڵ���*/ || SnapToGround()/*�����������棬Ҳ���Ǹո�δ������Ծ�����Ƿ��˳�ȥ*/
                || CheckSteepContacts()/*��б���ϣ��ұ�б���Χ*/)
            {
                airJumps = 0;

                stepSinceLastGround = 0;

                contactNormal.Normalize();
            }
            else
                contactNormal = Vector3.up;


            if (connectObj && connectObj.tag == "CheckMove")
            {
                UpdateConnectionState();
            }
        }

        /// <summary>/// ��������״̬������������Ҳ���ƶ�ʱ��������/// </summary>
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

        /// <summary> /// ʵ���ϵ���Ծ����  /// </summary>
        void Jump()
        {
            Vector3 jumpDirction;
            //ȷ����Ծ����
            if (onGround)
            {
                //�ڵ��ϣ�ֱ�Ӹ��ݽӴ�����
                jumpDirction = contactNormal;
            }
            else if (OnSteep)
            {
                //��б�����б�淽��ͬʱ���һ�����ϵķ��򣬱�֤�ܹ�������
                jumpDirction = (steepNormal + Vector3.up).normalized;
            }
            else if (airJumps < maxAirJumps)
            {
                //������ڵ���Ҳ����б�棬���ҿ����ڿ�����Ծ
                jumpDirction = Vector3.up;
            }
            //���������˳�
            else return;

            //���������Ĵ�С��ȷ���ƶ��ٶȣ���������ɱ���
            float jumpSpeed = Mathf.Sqrt(2f * -Physics.gravity.y * jumpHeight);
            float aligneSpeed = Vector3.Dot(velocity, jumpDirction);
            if (aligneSpeed > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - aligneSpeed, 0);
            }
            velocity += jumpDirction * jumpSpeed;
            airJumps++;

            //��Ծʱˢ����Ծʱ�䣬��֤��ǰ�����ʱ�䲻���������
            stepSinceLastJump = 0;
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
            if (collision.contactCount <= 0)
                groundCount = 0;
            else groundCount = 1;
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
                    steepContractCount++;
                    steepNormal += normal;
                    connectObj = collision.gameObject;
                }

            }
        }

        /// <summary>/// �����ƶ�����������֤�ƶ��ķ���������ƽ��� /// </summary>
        void AdjustVelocity()
        {
            //��Ϊ�ٶ��õ�Ҳ���������꣬����ƶ�ʱͶӰҲ���������������꣬����right����x�ᣬfoward����Y��
            //��1��0��0ͶӰ���Ӵ�ƽ���ϣ�
            Vector3 xAixs = ProjectDirectionOnPlane(Vector3.right, contactNormal);
            //��0��0��1ͶӰ���Ӵ�ƽ����
            //Vector3 zAxis = ProjectDirectionOnPlane(Vector3.forward, contactNormal);

            Vector3 relativeVelocity = velocity - connectionVelocity;
            //ȷ��ʵ���������ƽ���ϵ�X�ƶ�ֵ
            float currentX = Vector3.Dot(relativeVelocity, xAixs);
            //ȷ��ʵ���������ƽ���ϵ�Z�ƶ�ֵ
            //float currentZ = Vector3.Dot(relativeVelocity, zAxis);

            float acceleration = onGround ? groundAcceleration : airAcceleration;
            float maxSpeedChange = acceleration * Time.deltaTime;

            //ȷ�����������õ����ƶ�ֵ
            float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
            //float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

            //�ƶ�Ҫ�������ƽ��ķ������ƶ�����˸���ʵ��ֵ������ֵ�Ĳ�ȷ��Ҫ���ӵ��ٶȴ�С��
            //Ȼ�����ͶӰ���������Xֵ�Լ�Zֵȷ�������ƶ�ֵ
            //velocity += xAixs * (newX - currentX) + zAxis * (newZ - currentZ);
            velocity += xAixs * (newX - currentX);
        }

        /// <summary> /// ������ݣ���һЩ���ݹ�Ϊ��ʼ��   /// </summary>
        void ClearState()
        {
            onGround = false;
            contactNormal = steepNormal = connectionVelocity = Vector3.zero;
            steepContractCount = 0;
            preConnectObj = connectObj;
            connectObj = null;
            prePosition = transform.position;
        }

        /// <summary>
        /// �������������õķ����������ƶ�ʱ��ɳ�ȥ��Ч��
        /// </summary>
        /// <returns>�������һЩ������ʹ�ã�����з���ֵ</returns>
        bool SnapToGround()
        {
            //������Ϊֻ����һ�Σ�ͬʱ����Ծʱ�������Ծʱ����
            if (stepSinceLastGround > 1 || stepSinceLastJump <= 2)
            {
                return false;
            }
            float speed = velocity.magnitude;
            //��������ٶȣ�����������
            if (speed > maxSnapSpeed)
                return false;

            RaycastHit hit;
            if (!Physics.Raycast(body.position, -Vector3.up, out hit, probeDistance, probeMask))
                return false;

            float upDot = Vector3.Dot(Vector3.up, hit.normal);
            //������е��治����Ϊ����վ�����棬�Ͳ���������
            if (upDot < GetMinDot(hit.collider.gameObject.layer))
                return false;

            contactNormal = hit.normal;

            //ȷ���ٶ��ڷ����ϵĴ�С
            float dot = Vector3.Dot(velocity, hit.normal);
            //��ֻ֤���ٶȳ���ʱ�Ż�����ѹ��������������ٶ�
            if (dot > 0)
            {
                //�����ٶȵĴ�С��ƽ����ѹ
                velocity = (velocity - hit.normal * dot).normalized * speed;
            }
            connectObj = hit.collider.gameObject;
            return true;
        }

        float GetMinDot(int layer)
        {
            //�ж���¥�ݻ�����������
            return (stairsMask & (1 << layer)) == 0 ?
                minGroundDot : minStairsDot;
        }

        /// <summary>
        /// ���б�棬����������Χ��һ��ʱ�����ڵ��ϣ���ʱ���Ӵ��������Χ�Ƶķ��߷���
        /// </summary>
        /// <returns>�Ƿ�б���Χ���޷��ƶ�</returns>
        bool CheckSteepContacts()
        {
            if (steepContractCount > 1)
            {
                steepNormal.Normalize();
                contactNormal = steepNormal;
                float upDot = Vector3.Dot(Vector3.up, steepNormal);
                if (upDot >= minGroundDot)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>    /// ȷ���÷���ͶӰ����ƽ���ϵķ���ֵ�����й���׼����    /// </summary>
        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }

        /// <summary> /// ������⣬��������  /// </summary>
        void ClimbCheck()
        {
            Vector3 beginPos, forward;
            if (playerInputSpace)
                forward = playerInputSpace.forward;
            else
                forward = transform.forward;
            forward.y = 0;
            forward = forward.normalized * climbData.x + Vector3.up * climbData.y;
            beginPos = transform.position + forward;

            //Debug.DrawRay(beginPos, Vector3.down, Color.red);

            if (Physics.Raycast(beginPos, Vector3.down, out RaycastHit hit, 0.5f, probeMask))
            {
                if (hit.normal.y > minGroundDot && velocity.y > 0)
                    velocity += Vector3.up * climbData.z;
            }
        }

        /// <summary>  /// �Զ���ת   /// </summary>
        void AutoRotate()
        {
            //��һ���ٶȲŻ���ת
            if (Mathf.Abs(desiredVelocity.x) > 0.01f)
            {
                transform.right = desiredVelocity.x > 0? Vector3.right : Vector3.left;
            }
        }

        public override bool OnGround()
        {
            return groundCount > 0;
        }

        public override void BeginAutoRotate()
        {
        }

        public override void EndAutoRotate()
        {
        }
    }
}