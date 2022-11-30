﻿using UnityEngine;

namespace Motor
{
    /// <summary>
    /// 立体移动引擎
    /// </summary>
    public class Motor3D : MotorBase
    {
        //外界需要设置的数据
        #region Outside SetData  

        [SerializeField]
        /// <summary>    /// 加速度    /// </summary>
        private float groundAcceleration = 10f, airAcceleration = 5;

        [SerializeField]
        private float jumpHeight = 2f;
        [SerializeField]
        /// <summary>/// 最大空中跳跃次数，注意这个是空中跳，不包含地面跳跃/// </summary>
        private int maxAirJumps = 2;

        /// <summary>        /// 最大地面的倾斜夹角，以及楼梯倾斜夹角        /// </summary>
        [Range(0, 90), SerializeField]
        private float maxGroundAngle = 25f, maxStairAngle = 25;

        /// <summary> /// 攀爬检测的偏移数值，x是前方缩放，y是向上偏移,z是添加的力的大小 /// </summary>
        [SerializeField]
        Vector3 climbData = Vector3.one;

        /// <summary>    /// 判断时候可以贴地的速度，如果速度大于该值，不允许贴地    /// </summary>
        [SerializeField, Range(0, 100f)]
        float maxSnapSpeed = 100f;
        /// <summary>    /// 贴地的检测距离    /// </summary>
        [SerializeField, Range(0, 10f)]
        float probeDistance = 3f;

        /// <summary>    /// 贴地检查的层，以及楼梯检查层    /// </summary>
        [SerializeField]
        LayerMask probeMask, stairsMask = -1;

        /// <summary>    /// 输入空间，用来根据该空间来控制模型移动    /// </summary>
        [SerializeField]
        Transform playerInputSpace;

        #endregion

        //计算时需要的数据
        #region Compete Load Data

        Rigidbody body;
        GameObject connectObj, preConnectObj;
        private bool desiredJump = false;
        /// <summary>    /// 当前速度, 期望速度, 连接物体的速度    /// </summary>
        Vector3 velocity, desiredVelocity, connectionVelocity;

        private int airJumps = 0;       //当前空跳次数

        /// <summary>    /// 是否在地面上    /// </summary>
        private bool onGround = false;

        /// <summary> /// 陡峭面检测，如果人物被卡在陡峭面中，需要进行计数，判断是否要陡峭面跳跃/// </summary>
        private int steepContractCount = 0;
        /// <summary>    /// 字段OnSteep，确定是否在斜面上    /// </summary>
        private bool OnSteep => steepContractCount > 0;

        /// <summary>
        /// 接触面的法线，这个法线是平均法线，用来确定移动面的方向以及跳跃的方向
        /// </summary>
        Vector3 contactNormal, steepNormal;

        private float minGroundDot = 0, minStairsDot = 0;   //角度转化为的弧度值

        /// <summary>
        /// 用来确定此时离开地面的时间(stepSinceLastGround)，在地面时会变为0，
        /// 不在时会逐物理帧刷新
        /// </summary>
        int stepSinceLastGround = 0;
        /// <summary>    /// 用来确定跳跃的时间，当跳跃时会归零，在物理帧时逐帧增加    /// </summary>
        int stepSinceLastJump = 0;

        Info.CharacterInfo characterInfo;       //角色信息
        Vector3 connectionWorldPostion,         //上一次连接物体的位置
            prePosition;        //上一次的位置

        #endregion

        public override void Move(float horizontal, float vertical)
        {
            //实际上就是加速度是定值，角色输入值是目标速度，逐帧将速度变化为目标速度
            Vector2 playInput = new Vector2(vertical, horizontal);
            playInput = Vector2.ClampMagnitude(playInput, 1);
            //如果由传入的移动空间就根据这个空间的数值移动，不然就直接用世界坐标移动
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

        //初始化需要数据
        void Awake()
        {
            velocity = Vector3.zero;
            body = GetComponent<Rigidbody>();
            minGroundDot = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairsDot = Mathf.Cos(maxStairAngle * Mathf.Deg2Rad);
            characterInfo = GetComponent<Info.CharacterInfo>();
            if (characterInfo == null) Debug.LogError("角色信息为空");
        }
        private void FixedUpdate()
        {
            //更新数据，用来对这一个物理帧的数据进行更新之类的
            UpdateState();

            //确定在空中还是在地面
            AdjustVelocity();
            ClimbCheck();
            if (desiredJump)
            {
                Jump();
                desiredJump = false;
            }
            AutoRotate();

            //velocity = Vector3.ClampMagnitude(velocity, 40);

            body.velocity = velocity;
            ClearState();
        }

        void UpdateState()
        {
            stepSinceLastGround += 1;
            stepSinceLastJump += 1;
            velocity = body.velocity;

            //当不在地面时执行贴近地面方法
            if (onGround/*在地上*/ || SnapToGround()/*可以贴近地面，也就是刚刚未经过跳跃，但是飞了出去*/
                || CheckSteepContacts()/*在斜面上，且被斜面包围*/)
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

        /// <summary>/// 更新连接状态，用来当地面也会移动时进行贴合/// </summary>
        void UpdateConnectionState()
        {
            //只有物体相同，才有必要计算
            if (connectObj == preConnectObj)
            {
                Vector3 connectionMovment =
                    connectObj.transform.position - connectionWorldPostion;
                connectionVelocity = connectionMovment / Time.deltaTime;
            }
            connectionWorldPostion = connectObj.transform.position;
        }

        /// <summary> /// 实际上的跳跃方法  /// </summary>
        void Jump()
        {
            Vector3 jumpDirction;
            //确定跳跃方向
            if (onGround)
            {
                //在地上，直接根据接触方向
                jumpDirction = contactNormal;
            }
            else if (OnSteep)
            {
                //在斜面就用斜面方向，同时添加一个向上的方向，保证能够往上爬
                jumpDirction = (steepNormal + Vector3.up).normalized;
            }
            else if (airJumps < maxAirJumps)
            {
                //如果不在地上也不在斜面，并且可以在空中跳跃
                jumpDirction = Vector3.up;
            }
            //不能跳，退出
            else return;

            //根据重力的大小来确定移动速度，因此重力可变了
            float jumpSpeed = Mathf.Sqrt(2f * -Physics.gravity.y * jumpHeight);
            float aligneSpeed = Vector3.Dot(velocity, jumpDirction);
            if (aligneSpeed > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - aligneSpeed, 0);
            }
            velocity += jumpDirction * jumpSpeed;
            airJumps++;

            //跳跃时刷新跳跃时间，保证在前面这段时间不会进行贴地
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
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                float upDot = Vector3.Dot(Vector3.up, normal);
                if (upDot >= minDot)
                {
                    onGround = true;
                    //保证如果有多个接触面时能够正确的获取法线
                    contactNormal += normal;
                    connectObj = collision.gameObject;
                }
                //陡峭面移动控制，但是避免彻底的垂直面
                else if (upDot > -0.01f)
                {
                    steepContractCount++;
                    steepNormal += normal;
                    connectObj = collision.gameObject;
                }

            }
        }

        /// <summary>/// 调整移动方向，用来保证移动的方向是沿着平面的 /// </summary>
        void AdjustVelocity()
        {
            //因为速度用的也是世界坐标，因此移动时投影也依靠的是世界坐标，其中right控制x轴，foward控制Y轴
            //将1，0，0投影到接触平面上，
            Vector3 xAixs = ProjectDirectionOnPlane(Vector3.right, contactNormal);
            //将0，0，1投影到接触平面上
            Vector3 zAxis = ProjectDirectionOnPlane(Vector3.forward, contactNormal);

            Vector3 relativeVelocity = velocity - connectionVelocity;
            //确定实际上在这个平面上的X移动值
            float currentX = Vector3.Dot(relativeVelocity, xAixs);
            //确定实际上在这个平面上的Z移动值
            float currentZ = Vector3.Dot(relativeVelocity, zAxis);

            float acceleration = onGround ? groundAcceleration : airAcceleration;
            float maxSpeedChange = acceleration * Time.deltaTime;

            //确定根据期望得到的移动值
            float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
            float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

            //移动要根据这个平面的方向来移动，因此根据实际值与期望值的差确定要增加的速度大小，
            //然后乘以投影计算出来的X值以及Z值确定最后的移动值
            velocity += xAixs * (newX - currentX) + zAxis * (newZ - currentZ);
        }

        /// <summary>
        /// 清除数据，把一些数据归为初始化
        /// </summary>
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
        /// 用于贴近地面用的方法，减少移动时会飞出去的效果
        /// </summary>
        /// <returns>用来配合一些地面检测使用，因此有返回值</returns>
        bool SnapToGround()
        {
            //贴地行为只进行一次，同时用跳跃时间避免跳跃时贴地
            if (stepSinceLastGround > 1 || stepSinceLastJump <= 2)
            {
                return false;
            }
            float speed = velocity.magnitude;
            //大于最大速度，不进行贴地
            if (speed > maxSnapSpeed)
                return false;

            RaycastHit hit;
            if (!Physics.Raycast(body.position, -Vector3.up, out hit, probeDistance, probeMask))
                return false;

            float upDot = Vector3.Dot(Vector3.up, hit.normal);
            //如果射中的面不能作为可以站立的面，就不进行贴近
            if (upDot < GetMinDot(hit.collider.gameObject.layer))
                return false;

            contactNormal = hit.normal;

            //确定速度在法线上的大小
            float dot = Vector3.Dot(velocity, hit.normal);
            //保证只有速度朝上时才会往下压，不会减少下落速度
            if (dot > 0)
            {
                //根据速度的大小往平面上压
                velocity = (velocity - hit.normal * dot).normalized * speed;
            }
            connectObj = hit.collider.gameObject;
            return true;
        }

        float GetMinDot(int layer)
        {
            //判断是楼梯还是正常地面
            return (stairsMask & (1 << layer)) == 0 ?
                minGroundDot : minStairsDot;
        }

        /// <summary>
        /// 检查斜面，当被陡峭面围在一起时算是在地上，此时将接触方向就是围绕的法线方向
        /// </summary>
        /// <returns>是否被斜面包围且无法移动</returns>
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

        /// <summary>    /// 确定该方向投影到该平面上的方向值，进行过标准化的    /// </summary>
        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }

        /// <summary> /// 攀爬检测，进行攀爬  /// </summary>
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


        void AutoRotate()
        {
            Vector3 elure = transform.position - prePosition;
            elure.y = 0;
            //有一定速度才会旋转
            if (elure.magnitude > 0.0001f)
            {
                transform.LookAt(transform.position + Vector3.Lerp(
                    transform.forward, elure, Time.fixedDeltaTime * characterInfo.RotateSpeed
                    ));
            }
        }

        public override bool OnGround()
        {
            return false;
        }

        public override void BeginAutoRotate()
        {
        }

        public override void EndAutoRotate()
        {
        }
    }
}