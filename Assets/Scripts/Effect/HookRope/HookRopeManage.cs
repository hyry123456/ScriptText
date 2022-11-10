using UnityEngine;
using Common;
using DefferedRender;
using UnityEngine.Rendering;

public class HookRopeManage : GPUDravinBase
{
    public static HookRopeManage Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject go = new GameObject("HookRopeManage");
                go.AddComponent<HookRopeManage>();
            }
            return instance;
        }
    }
    private static HookRopeManage instance;
    

    /// <summary>    /// �洢�õ�����    /// </summary>
    PoolingList<Transform> poolingList;
    Transform target;   //�����Ŀ��
    Material material;  //��ʾ�õĲ���
    Material hookRopeMat;   //�����Ĳ���
    float particleSize; //���Ӵ�С����ȡ���ʵ�����
    float maxHookDistance = 30; //�����ļ����룬��Ҫ�޸ľ�ֱ�Ӹ�����

    bool isInsert;      //�Ƿ�������ջ��

    /// <summary>    /// �õ������Ŀ����󣬿���Ϊ��    /// </summary>
    public Transform Target
    {
        get
        {
            return target;
        }
    }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        poolingList = new PoolingList<Transform>();
        material = Resources.Load<Material>("EffectMaterial/SimpleParticle");
        hookRopeMat = Resources.Load<Material>("EffectMaterial/HookRopeMat");
        if (material == null)
            Debug.LogError("��Դ���ش���");
        particleSize = material.GetFloat("_ParticleSize");
        GPUDravinDrawStack.Instance.InsertRender(this);
        isInsert = true;
    }

    private void OnDestroy()
    {
        poolingList?.RemoveAll();

        if (isInsert)
        {
            GPUDravinDrawStack.Instance.RemoveRender(this);
            isInsert = false;
        }
    }


    /// <summary>
    /// ����Ҫ��Ϊ�����ڵ��ģ��λ�����괫�룬��Ϊ�ж�λ�õĸ���
    /// </summary>
    public void AddNode(Transform pos)
    {
        poolingList.Add(pos);
    }

    public void RemoveNode(Transform pos)
    {
        poolingList.Remove(pos);
    }


    private void FixedUpdate()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            target = null;
            return;
        }
        Transform camTran = camera.transform;
        Vector4[] planes = GetFrustumPlane(camera);
        int minIndex = -1;
        for(int i=0; i<poolingList.Count; i++)
        {
            for(int j=0; j < 6; j++)
            {
                Vector3 oriPos = poolingList.GetValue(i).transform.position;
                //����false��Ҳ������������˳�
                if (IsOutsideThePlane(planes[j], oriPos +
                    camTran.right * particleSize + camTran.up * -particleSize)
                    && IsOutsideThePlane(planes[j], oriPos +
                    camTran.right * -particleSize + camTran.up * -particleSize)
                    && IsOutsideThePlane(planes[j], oriPos +
                    camTran.right * -particleSize + camTran.up * particleSize)
                    && IsOutsideThePlane(planes[j], oriPos +
                    camTran.right * particleSize + camTran.up * particleSize))
                    break;
                
                if(j == 5)
                {
                    float newDis = (oriPos - camera.transform.position).sqrMagnitude;
                    if (newDis > maxHookDistance * maxHookDistance) continue;
                    if (minIndex == -1)
                        minIndex = i;
                    else if(newDis < (poolingList.GetValue(minIndex).transform.position
                        - camera.transform.position).sqrMagnitude)
                    {
                        minIndex = i;
                    }
                }
            }
        }
        if(minIndex != -1)
        {
            target = poolingList.GetValue(minIndex);
            return;
        }
        target = null;
    }


    public override void DrawByCamera(ScriptableRenderContext context, CommandBuffer buffer, ClustDrawType drawType, Camera camera)
    {
        if (material == null || Target == null)
            return;

        buffer.SetGlobalVector("_WorldPos", Target.position);
        buffer.DrawProcedural(Matrix4x4.identity, material, 0, MeshTopology.Points, 1);
        if (isLink && begin != null)
        {
            buffer.SetGlobalVector("_TargetPos", finalPos);
            buffer.SetGlobalVector("_BeginPos", begin.position + Vector3.down * 3);
            buffer.DrawProcedural(Matrix4x4.identity, hookRopeMat, 0, MeshTopology.Points, 1);
        }

        ExecuteBuffer(ref buffer, context);
        return;
    }

    Vector3 finalPos;
    Transform begin;
    bool isLink = false;

    /// <summary>    /// ��ʾ����ͼƬ    /// </summary>
    public void LinkHookRope(Vector3 target, Transform begin)
    {
        finalPos = target;
        this.begin = begin;
        isLink = true;
    }

    public void CloseHookRope()
    {
        isLink = false;
    }

    public override void DrawByProjectMatrix(ScriptableRenderContext context, CommandBuffer buffer, ClustDrawType drawType, Matrix4x4 projectMatrix)
    {
        return;
    }

    public override void DrawPreSSS(ScriptableRenderContext context, CommandBuffer buffer, Camera camera)
    {
        return;
    }

    public override void SetUp(ScriptableRenderContext context, CommandBuffer buffer, Camera camera)
    {
        return;
    }
}
