using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseAnime : MonoBehaviour
{
    [Header("拖入切换系列的各个父物体")]
    public GameObject[] AnimationSeriesOBJ;

    [Header("拖入GO按钮后需要创建的预制体")]
    public GameObject[] AnimationCollectionPrefab;

    int nowSeries = 0;
    int allSeries;
    private void Awake()
    {
        allSeries = AnimationSeriesOBJ.Length - 1;
    }

    public void _AddSeriesIndex()
    {
        nowSeries++;
        SwitchSeries();
    }
    public void _RemoveSeriesIndex()
    {
        nowSeries--;
        SwitchSeries();
    }
    void SwitchSeries()
    {
        // 切换显示的系列

        // 失活全部物体
        InactivatedObject();

        if (allSeries < nowSeries)
        {
            // 右超出界限
            nowSeries = 0;
        }
        else if (nowSeries < 0)
        {
            // 左超出界限
            nowSeries = allSeries;
        }

        // 激活选中物体
        AnimationSeriesOBJ[nowSeries].SetActive(true);
    }
    void InactivatedObject()
    {
        // 失活物体
        for (int i = 0; i < AnimationSeriesOBJ.Length; i++)
        {
            AnimationSeriesOBJ[i].SetActive(false);
        }
    }

    public void _ConfirmSelection()
    {
        // 确认选择系列
        PanelMannger.Instance.SwitchTo_NewPane(AnimationCollectionPrefab[nowSeries]);
    }
}
