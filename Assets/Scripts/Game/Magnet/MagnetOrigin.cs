using UnityEngine;

/// <summary>
/// 磁力源类，用来得到磁力的来源，顺便动态确定磁力的大小以及力度等
/// </summary>
public class MagnetOrigin : MonoBehaviour
{
    /// <summary> /// 磁力强度 /// </summary>
    [Min(0.01f)]
    public float magnetIntensity = 1;
    /// <summary> /// 磁力范围 /// </summary>
    [Min(0.01f)]
    public float magnetRange = 1;

    /// <summary> /// 获得该位置上的磁力强度以及方向 /// </summary>
    /// <param name="position">世界坐标位置</param>
    /// <returns>包含强度的方向</returns>
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
