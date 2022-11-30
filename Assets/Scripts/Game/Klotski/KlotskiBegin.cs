using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KlotskiBegin : MonoBehaviour
{
    public string path = "UI/";
    void Start()
    {
        KlotskiControl.Instance.BeginGame(path);
    }
}
