using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    private static ComboManager instance;
    public static ComboManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }

    // 当前连击数
    public int combo;
    public int maxCombo;

    // 常规连击数图片    [0-9]代表数字0-9，[10]代表Combo图片
    public Sprite[] normalComboSprite = new Sprite[11];

    // 全连连击数图片    [0-9]代表数字0-9，[10]代表Combo图片
    public Sprite[] fcComboSprite = new Sprite[11];

    // 连击数(Combo)父物体
    public GameObject comboFather;

    // 连击数数字预制体（附加Sprite Renderer）
    public GameObject comboNumberPreform;

    // Combo图片
    public SpriteRenderer comboSpriteRenderer;

    /// <summary>
    /// 增加连击数
    /// </summary>
    /// <param name="value">增加的值，若为-1，代表全连中断</param>
    public void addCombo(int value)
    {
        // 在各个音符的脚本中引用该方法...

        if (value < 0)
        {
            HideCombo();
        }
        else if (value == 0)
        {
            // 等于0等于不加Combo，维持现状
        }
        else
        {
            combo += value;
            ShowCombo();
            UpdateComboImage();

            if (maxCombo < combo)
                maxCombo = combo;
        }
    }

    bool isUseNormalComboNumbers = false;
    void CheckIsUseNormalComboNumbers()
    {
        // 检测是否应该使用普通数字
        if (isUseNormalComboNumbers == true) return;

        switch (GameSettingsMannger.save_Settings.specialComboThreshold)
        {
            case SpecialComboThreshold.JustPerfect:
                if (0 < JudgmentManager.Instance.numbers_Perfect + JudgmentManager.Instance.numbers_Great + JudgmentManager.Instance.numbers_Good + JudgmentManager.Instance.numbers_Bad + JudgmentManager.Instance.numbers_Miss)
                    isUseNormalComboNumbers = true;
                break;
            case SpecialComboThreshold.PerfectAndAbove:
                if (0 < JudgmentManager.Instance.numbers_Great + JudgmentManager.Instance.numbers_Good + JudgmentManager.Instance.numbers_Bad + JudgmentManager.Instance.numbers_Miss)
                    isUseNormalComboNumbers = true;
                break;
            case SpecialComboThreshold.GreatAndAbove:
                if (0 < JudgmentManager.Instance.numbers_Good + JudgmentManager.Instance.numbers_Bad + JudgmentManager.Instance.numbers_Miss)
                    isUseNormalComboNumbers = true;
                break;
            case SpecialComboThreshold.GoodAndAbove:
                if (0 < JudgmentManager.Instance.numbers_Bad + JudgmentManager.Instance.numbers_Miss)
                    isUseNormalComboNumbers = true;
                break;
            case SpecialComboThreshold.Disable:
                isUseNormalComboNumbers = true;
                break;
        }

        comboSpriteRenderer.sprite = normalComboSprite[10];
    }
    void UpdateComboImage()
    {
        // 更新连击数图片

        // 检测是否使用普通连击数图片
        CheckIsUseNormalComboNumbers();

        string comboString = combo.ToString();
        // 根据Combo长度求出整体总宽度，世界坐标中*0.8看起来间隔相对完美
        float overallWidth = comboString.Length * 0.8f;
        // 计算得到第一个连击数图片的x位置
        float firstSpritePosition = 0 - (overallWidth / 2);

        // 创建连击数数字图片
        for (int i = 0; i < comboString.Length; i++)
        {
            // 获取当前位数的数字
            int currentNumber = int.Parse(comboString[i].ToString());
            GameObject comboNumberOBJ = Instantiate(comboNumberPreform, comboFather.transform);
            // 设置位置 根据循环到的数字，每次偏移一个图片的宽度(中心点)，世界坐标中*0.8看起来间隔相对完美
            comboNumberOBJ.transform.position = new Vector3(firstSpritePosition + (i + 0.5f) * 0.8f, 0, 0);
            comboNumberOBJ.transform.localScale = Vector3.one;

            if (isUseNormalComboNumbers)
                comboNumberOBJ.GetComponent<SpriteRenderer>().sprite = normalComboSprite[currentNumber];
            else
                comboNumberOBJ.GetComponent<SpriteRenderer>().sprite = fcComboSprite[currentNumber];
        }
    }
    void HideCombo()
    {
        // 隐藏Combo数字和图片
        ClearComboObjects();
        comboSpriteRenderer.enabled = false;
        combo = 0;
    }
    void ShowCombo()
    {
        // 显示Combo数字和图片
        ClearComboObjects();
        comboSpriteRenderer.enabled = true;
    }
    void ClearComboObjects()
    {
        // 清除父物体下的子物体
        foreach (Transform child in comboFather.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
