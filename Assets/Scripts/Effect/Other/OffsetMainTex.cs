using UnityEngine;

enum ModeXY
{
    X,Y
}

/// <summary>
/// ƫ��ͼƬ�࣬�������������ͼƬ����ƫ��
/// </summary>
public class OffsetMainTex : MonoBehaviour
{
    public float speed = 0;
    [SerializeField]
    ModeXY mode = ModeXY.Y;
    Vector2 currentOffset;
    Material setMat;

    private void Start()
    {
        setMat = gameObject.GetComponent<MeshRenderer>().material;
        currentOffset = setMat.GetTextureOffset("_MainTex");
    }

    private void Update()
    {
        switch (mode)
        {
            case ModeXY.Y:
                currentOffset.y += Time.deltaTime * speed;
                break;
            case ModeXY.X:
                currentOffset.x += Time.deltaTime * speed;
                break;
        }
        setMat.SetTextureOffset("_MainTex", currentOffset);
    }


}
