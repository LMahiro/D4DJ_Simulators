using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar_Score : MonoBehaviour
{
    private static ScoreBar_Score instance;
    public static ScoreBar_Score Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        // 获得得分条原始高度
        fixed_ScoreBarFill_Height = scoreBar_Fill_Image.rectTransform.sizeDelta.y;
        // 获得闪烁动画
        scoreBar_Flashing_Animator = scoreBar_Flashing_Image.GetComponent<Animator>();
        // 初始化对象池
        MoveNumberPool_Initialize();
    }

    // 当前分数
    public float score;
    public void AddScore(float addValue)
    {
        score += addValue;

        Update_ScoreBar();
        Update_ScoreNumbers();
        Display_MoveNumbers(addValue);
    }
    public void RemoveScore(float removeValue)
    {
        score -= removeValue;

        Update_ScoreBar();
        Update_ScoreNumbers();
        // 不显示移动数字
    }


    [Header("得分条填充图片")]
    public Image scoreBar_Fill_Image;
    [Header("得分条填充图片的各阶段颜色")]
    public Sprite[] scoreBar_Fill_Image_Color = new Sprite[3];  // 0为绿色，1为橙色，2为红色

    [Header("得分条闪烁图片")]
    public Image scoreBar_Flashing_Image;
    private Animator scoreBar_Flashing_Animator;

    [Header("填满得分条所需分数")]
    public float scoreBar_MaxPoint = 1000000;
    // 当前得分条宽度
    private float current_ScoreBarFill_Width;
    // 固定得分条高度
    private float fixed_ScoreBarFill_Height;
    private void Update_ScoreBar()
    {
        // 计算得分条宽度
        if (0 < score && score <= scoreBar_MaxPoint)    // 0 ~ 1
            current_ScoreBarFill_Width = (score / scoreBar_MaxPoint) * ScoreBar_ScoreRank.Instance.scoreBar_Length;
        else if (score > scoreBar_MaxPoint)             // 1
            current_ScoreBarFill_Width = ScoreBar_ScoreRank.Instance.scoreBar_Length;
        else current_ScoreBarFill_Width = 0;            // 0

        // 设置得分条填充图片的宽度
        image_sizeDelta.Set(current_ScoreBarFill_Width, fixed_ScoreBarFill_Height);
        scoreBar_Fill_Image.rectTransform.sizeDelta = image_sizeDelta;

        // 得分条填充图片的变色
        if (score < ScoreBar_ScoreRank.Instance.rank_B_Percentage * scoreBar_MaxPoint)        // 0 ~ B
            scoreBar_Fill_Image.sprite = scoreBar_Fill_Image_Color[0];
        else if (score >= ScoreBar_ScoreRank.Instance.rank_B_Percentage * scoreBar_MaxPoint   // B ~ A
            && score < ScoreBar_ScoreRank.Instance.rank_A_Percentage * scoreBar_MaxPoint)
            scoreBar_Fill_Image.sprite = scoreBar_Fill_Image_Color[1];                        // A ~
        else scoreBar_Fill_Image.sprite = scoreBar_Fill_Image_Color[2];

        // 得分条闪烁
        if (score >= ScoreBar_ScoreRank.Instance.rank_S_Percentage * scoreBar_MaxPoint)
            scoreBar_Flashing_Animator.Play("ScoreBar_Flashing");
        else scoreBar_Flashing_Animator.Play("ScoreBar_Hide");
    }
    private Vector2 image_sizeDelta;


    [Header("分数数字：物体")]
    public Image[] scoreNumber_Obj = new Image[8];          // 0为个位，1为十位，依次上涨
    [Header("分数数字：Sprite")]
    public Sprite[] scoreNumber_Sprite = new Sprite[11];    // 0-9同原数字，10为灰色0
    private void Update_ScoreNumbers()
    {
        score_String = ((int)score).ToString();

        // 极端情况显示处理
        if (score > 99999999)
        {
            for (int i = 0; i < scoreNumber_Obj.Length; i++)
                scoreNumber_Obj[i].sprite = scoreNumber_Sprite[9];
            return;
        }
        else if (score < 0)
        {
            for (int i = 0; i < scoreNumber_Obj.Length; i++)
                scoreNumber_Obj[i].sprite = scoreNumber_Sprite[10];
            return;
        }

        // 设置显示数字，从右向左遍历字符串
        for (int i = 0; i < score_String.Length; i++)
        {
            char digitChar = score_String[score_String.Length - 1 - i]; // 获取当前位的数字字符
            int digit = int.Parse(digitChar.ToString());                // 将字符转换为整数
            scoreNumber_Obj[i].sprite = scoreNumber_Sprite[digit];      // 设置对应位的Image的Sprite
        }

        // 如果分数字符串长度小于数组长度，将剩余的位数设置为灰色0
        for (int i = score_String.Length; i < scoreNumber_Obj.Length; i++)
            scoreNumber_Obj[i].sprite = scoreNumber_Sprite[10];
    }
    private string score_String;



    [Header("拖入预制体的父对象物体")]
    public GameObject addScore_MoveNumber_Father;
    [Header("拖入加分移动数字预制体")]
    public GameObject addScore_MoveNumber_Prefab;
    [Header("加分移动数字：Sprite")]
    public Sprite[] addScore_MoveNumber_Sprite = new Sprite[11];    // 0-9同原数字，10为透明图片
    // （可用的）移动数字对象池
    private Queue<GameObject> moveNumber_Pool = new Queue<GameObject>();

    private void Display_MoveNumbers(float addValue)
    {
        // 获取一个移动数字对象
        GameObject moveNumberObj = MoveNumberPool_GetObj();
        if (moveNumberObj == null)
            return;     // 取出失败：没东西了。本次不播放动画

        // 获取 AddScore_MoveNumber 脚本
        AddScore_MoveNumber moveNumberScript = moveNumberObj.GetComponent<AddScore_MoveNumber>();
        // 数字显示设置
        SetDisplayImage_MoveNumbers(moveNumberScript, addValue);

        // 播放动画
        moveNumberScript.animator.Play("moveNumber");

        // 在动画播放完毕后将对象放回对象池
        StartCoroutine(MoveNumberPool_Return(moveNumberObj));
    }
    public void SetDisplayImage_MoveNumbers(AddScore_MoveNumber moveNumberScript, float addValue)
    {
        // 极端情况处理
        if (addValue > 99999999) return;
        else if (addValue < 0) return;

        addScoreValue_String = ((int)addValue).ToString();  // 800
        scoreCharacterLength = addScoreValue_String.Length; // 3

        // 显示内容设置
        for (int i = scoreCharacterLength; i > 0; i--)
        {
            char digitChar = addScoreValue_String[i - 1];   // [2] => 0 ; 8[0] 0[1] 0[2]
            int digit = int.Parse(digitChar.ToString());    // 0
            moveNumberScript.addScore_MoveNumber_Obj[i - 1].sprite = addScore_MoveNumber_Sprite[digit]; // x x 0
        }

        // 空图片设置
        if (0 < scoreCharacterLength && scoreCharacterLength < 8)   // 3 => true
        {
            for (int i = scoreCharacterLength; i < 8; i++)          // for(int i = 3-1 ; i < 8) 
                moveNumberScript.addScore_MoveNumber_Obj[i].sprite = addScore_MoveNumber_Sprite[10];
        }
    }
    private int scoreCharacterLength;
    private string addScoreValue_String;

    private void MoveNumberPool_Initialize()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject moveNumberObj = Instantiate(addScore_MoveNumber_Prefab, addScore_MoveNumber_Father.transform);
            moveNumberObj.SetActive(false);
            moveNumber_Pool.Enqueue(moveNumberObj);     // 压入
        }
    }
    private GameObject MoveNumberPool_GetObj()
    {
        if (moveNumber_Pool.Count > 0)
        {
            GameObject moveNumberObj = moveNumber_Pool.Dequeue();   // 取出
            moveNumberObj.SetActive(true);
            return moveNumberObj;
        }
        else return null;
    }
    private IEnumerator MoveNumberPool_Return(GameObject moveNumberObj)
    {
        // 等待动画播放完毕
        yield return new WaitForSeconds(moveNumberObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

        // 将对象放回对象池
        moveNumberObj.SetActive(false);
        moveNumber_Pool.Enqueue(moveNumberObj);
    }
}
