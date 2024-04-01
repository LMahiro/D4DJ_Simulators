using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectManager : MonoBehaviour
{
    private static TapEffectManager instance;
    public static TapEffectManager Instance => instance;
    private void Start()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        // 池子最多装5个
        for (int i = 0; i < 5; i++)
        {
            GameObject tapEffect = Instantiate(tapEffect_OBJ, parentOBJ.transform);
            tapEffect.SetActive(false);
            tapEffect_Pool.Add(tapEffect);
        }

        // 在设置中开启才添加事件
        if (GameSettingsMannger.save_Settings.showTapEffects == true)
            LeanTouch.OnFingerDown += OnFingerTap;  // 这里不调用方法是因为 方法为了防止多次添加事件 会有个判断
    }
    private void OnDestroy()
    {
        LeanTouch.OnFingerDown -= OnFingerTap;
    }

    [Header("动画预设体")]
    public GameObject tapEffect_OBJ;
    [Header("拖入父对象")]
    public GameObject parentOBJ;


    private List<GameObject> tapEffect_Pool = new List<GameObject>();    // 池子
    private GameObject lastOBJ;                 // 最后一个物体
    private Coroutine lastOBJ_Coroutine;        // 最后物体的回收协程

    private GameObject TapEffectOBJ_Get()
    {
        if (tapEffect_Pool.Count != 0)
        {
            foreach (GameObject tapEffect in tapEffect_Pool)
            {
                lastOBJ = tapEffect;
                lastOBJ_Coroutine = StartCoroutine(TapEffectOBJ_Return(tapEffect));

                tapEffect_Pool.Remove(tapEffect);
                tapEffect.SetActive(true);
                return tapEffect;
            }
        }
        else
        {
            StopCoroutine(lastOBJ_Coroutine);
            lastOBJ.SetActive(false);

            lastOBJ_Coroutine = StartCoroutine(TapEffectOBJ_Return(lastOBJ));
            tapEffect_Pool.Remove(lastOBJ);
            lastOBJ.SetActive(true);
            return lastOBJ;
        }

        return null;    // 这句代码不加会报错
    }
    private IEnumerator TapEffectOBJ_Return(GameObject tapEffectOBJ)
    {
        // 等待动画播放完毕
        yield return new WaitForSeconds(0.25f);

        // 将对象放回对象池
        tapEffectOBJ.SetActive(false);
        tapEffect_Pool.Add(tapEffectOBJ);
    }
    private void OnFingerTap(LeanFinger finger)
    {
        Vector2 tapPosition = finger.StartScreenPosition;
        GameObject tapEffectOBJ = TapEffectOBJ_Get();

        if (tapEffectOBJ != null)
        {
            tapEffectOBJ.GetComponent<RectTransform>().anchoredPosition = tapPosition;
            tapEffectOBJ.SetActive(true);
        }
    }

    // 设置面板控制是否开启的方法
    public void Show_TapEffect()
    {
        if (GameSettingsMannger.save_Settings.showTapEffects == false)
        {
            LeanTouch.OnFingerDown += OnFingerTap;
            GameSettingsMannger.save_Settings.showTapEffects = true;
        }
    }
    public void Hide_TapEffect()
    {
        if (GameSettingsMannger.save_Settings.showTapEffects == true)
        {
            LeanTouch.OnFingerDown -= OnFingerTap;
            GameSettingsMannger.save_Settings.showTapEffects = false;
        }
    }
}
