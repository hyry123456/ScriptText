using UnityEngine;

/// <summary>
/// ����Դ�࣬�����õ���������Դ��˳�㶯̬ȷ�������Ĵ�С�Լ����ȵ�
/// </summary>
public class MagnetOrigin : MonoBehaviour
{
    /// <summary> /// ����ǿ�� /// </summary>
    [Min(0.01f)]
    public float magnetIntensity = 1;
    /// <summary> /// ������Χ /// </summary>
    [Min(0.01f)]
    public float magnetRange = 1;

    /// <summary> /// ��ø�λ���ϵĴ���ǿ���Լ����� /// </summary>
    /// <param name="position">��������λ��</param>
    /// <returns>����ǿ�ȵķ���</returns>
    public Vector3 GetMagnetDuration(Vector3 position)
    {
        float intensity = Mathf.Clamp01( (position - transform.position).sqrMagnitude
            / (magnetRange * magnetRange) );
        return (transform.position - position).normalized * (1.0f - intensity)
            * magnetIntensity;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magnetRange);
    }
#endif
}
