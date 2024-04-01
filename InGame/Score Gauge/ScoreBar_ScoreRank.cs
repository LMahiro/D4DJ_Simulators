using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar_ScoreRank : MonoBehaviour
{
    private static ScoreBar_ScoreRank instance;
    public static ScoreBar_ScoreRank Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        scoreBar_Length = position_Right.x - position_Left.x;
        scoreBar_Hight = position_Right.y - position_Left.y;

        SetScoreRankPosition();
    }

    [Header("设置各等级所对应的位置百分比")]
    public float rank_C_Percentage = 0.0714f;
    public float rank_B_Percentage = 0.2857f;
    public float rank_A_Percentage = 0.5f;
    public float rank_S_Percentage = 0.7143f;
    public float rank_SS_Percentage = 0.9286f;
    public float rank_Clear_Percentage = 0.7143f;

    [Header("拖入对应等级的图片Obj")]
    public Image rank_C_Image;
    public Image rank_B_Image;
    public Image rank_A_Image;
    public Image rank_S_Image;
    public Image rank_SS_Image;
    public Image rank_Clear_Image;

    [Header("选择是否显示Clear")]
    public bool isClearDisplayed = false;

    [Header("输入整个框在左下角的定位点")]
    public Vector2 position_Left = new Vector2(89.33f, -101.4f);
    [Header("输入整个框在右下角的定位点")]
    public Vector2 position_Right = new Vector2(586.33f, -114);

    [HideInInspector]
    public float scoreBar_Length;   // 整个框的长度
    [HideInInspector]
    public float scoreBar_Hight;    // 整个框的下移变化量

    public void SetScoreRankPosition()
    {
        //487.3是整个框的长度，-12.1由两个y值相减得来
        rank_C_Image.rectTransform.anchoredPosition = new Vector2(position_Left.x + scoreBar_Length * rank_C_Percentage, position_Left.y + scoreBar_Hight * rank_C_Percentage);
        rank_B_Image.rectTransform.anchoredPosition = new Vector2(position_Left.x + scoreBar_Length * rank_B_Percentage, position_Left.y + scoreBar_Hight * rank_B_Percentage);
        rank_A_Image.rectTransform.anchoredPosition = new Vector2(position_Left.x + scoreBar_Length * rank_A_Percentage, position_Left.y + scoreBar_Hight * rank_A_Percentage);
        rank_S_Image.rectTransform.anchoredPosition = new Vector2(position_Left.x + scoreBar_Length * rank_S_Percentage, position_Left.y + scoreBar_Hight * rank_S_Percentage);
        rank_SS_Image.rectTransform.anchoredPosition = new Vector2(position_Left.x + scoreBar_Length * rank_SS_Percentage, position_Left.y + scoreBar_Hight * rank_SS_Percentage);
        rank_Clear_Image.rectTransform.anchoredPosition = new Vector2(position_Left.x + scoreBar_Length * rank_Clear_Percentage, position_Left.y + scoreBar_Hight * rank_Clear_Percentage);

        if (isClearDisplayed == false) rank_Clear_Image.gameObject.SetActive(false);
    }
}
