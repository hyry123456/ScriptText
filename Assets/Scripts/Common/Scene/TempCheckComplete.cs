using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TempCheckComplete : MonoBehaviour
{
    public string targetScene;
    void Update()
    {
        if (Common.DataLoad.Instance.IsLoadComplete)
        {
            Common.SceneControl.Instance.ChangeSceneDirect(targetScene);
            gameObject.SetActive(false);
        }
    }
}
