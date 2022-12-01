using UnityEngine;

/// <summary>/// �ܴ���Ӱ���������/// </summary>
public class MagnetItem : MonoBehaviour
{
    public float gravity;
    public MagnetOrigin magnetOrigin;
    public Vector3 normal;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;      //�������ⲿ����
    }

    private void FixedUpdate()
    {
        Vector3 duration = magnetOrigin.
            GetMagnetDuration(transform.position);
        duration += Vector3.down * gravity;
        rb.velocity += duration * Time.fixedDeltaTime;
    }
}
