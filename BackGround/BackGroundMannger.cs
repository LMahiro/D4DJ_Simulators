using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMannger : MonoBehaviour
{
    private static BackGroundMannger instance;
    public static BackGroundMannger Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }

    [Header("拖入背景动画父物体")]
    public GameObject[] backgroundAnimationOBJ;
    // 0 Normal  1 Edit  2 Black  3 Anime

    void InactivatedObject()
    {
        // 失活物体
        for (int i = 0; i < backgroundAnimationOBJ.Length; i++)
        {
            backgroundAnimationOBJ[i].SetActive(false);
        }
    }
    public void SwitchTo_Normal()
    {
        InactivatedObject();
        backgroundAnimationOBJ[0].SetActive(true);
    }
    public void SwitchTo_Edit()
    {
        InactivatedObject();
        backgroundAnimationOBJ[1].SetActive(true);
    }
    public void SwitchTo_Black()
    {
        InactivatedObject();
        backgroundAnimationOBJ[2].SetActive(true);
    }
    public void SwitchTo_Anime()
    {
        InactivatedObject();
        backgroundAnimationOBJ[3].SetActive(true);
    }
}
