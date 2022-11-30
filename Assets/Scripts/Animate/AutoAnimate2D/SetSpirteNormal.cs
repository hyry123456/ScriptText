using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ʵʱˢ��2D�ķ��ߣ���Ϊ2D��ɫ��Ҫ���ߣ����Ƕ���ֻ�ᴩ������
/// ��˷�����Ҫ���⴫�ݣ�����Sprite���Զ�ƥ��UV��ֻ��Ҫ���뷨�߼���
/// </summary>
public class SetSpirteNormal : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField]
    List<Texture> normal;

    Dictionary<string, Texture> spriteMaps;
    string currentName;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (normal == null) return;
        spriteMaps = new Dictionary<string, Texture>();
        for(int i=0; i<normal.Count; i++)
        {
            string name = normal[i].name.Substring(0, normal[i].name.Length - 7);
            spriteMaps[name] = normal[i];
        }
    }

    private void Update()
    {
        if (spriteMaps == null) return;
        string spriteName = spriteRenderer.sprite.name;
        string thisName = spriteName.Substring(0, spriteName.LastIndexOf('_'));
        if (thisName != currentName)
        {
            Texture texture;
            if(spriteMaps.TryGetValue(thisName, out texture))
            {
                spriteRenderer.material.SetTexture("_NormalMap", texture);
                currentName = thisName;
            }
            else
            {
                Debug.LogError(thisName + " " + spriteName);
            }
        }
    }
}
