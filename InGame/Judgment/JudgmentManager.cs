using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentManager : MonoBehaviour
{
    private static JudgmentManager instance;
    public static JudgmentManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        // 池子最多装10个
        for (int i = 0; i < 10; i++)
        {
            GameObject judgmentOBJ = Instantiate(judgment_OBJ, judgmentFatherOBJ);
            // 设置位置
            judgmentOBJ.transform.position = new Vector3(0, GameSettingsMannger.save_Settings.judgementTextHeight, 0);

            judgmentOBJ.SetActive(false);
            judgment_Pool.Add(judgmentOBJ);
        }
    }

    [Header("拖入判定文字预制体")]
    public GameObject judgment_OBJ;

    [Header("判定文字父对象")]
    public Transform judgmentFatherOBJ;

    [Header("拖入判定文字图片")]    // 0 JP；1 P；2 Gr；3 Go；4 B；5 M；6 Auto
    public Sprite[] judgmentSprites = new Sprite[7];

    [Header("拖入Fast/Slow图片")]   // 0 Fast；1 Slow
    public Sprite[] fastSlowSprites = new Sprite[2];

    private List<GameObject> judgment_Pool = new List<GameObject>();    // 池子
    private GameObject lastOBJ;                 // 最后一个物体
    private Coroutine lastOBJ_Coroutine;        // 最后物体的回收协程

    #region 各判定计数
    public int[] FastNumbers = new int[6]; public int[] SlowNumbers = new int[6];
    public int numbers_Miss; public int numbers_Bad; public int numbers_Good; public int numbers_Great; public int numbers_Perfect; public int numbers_JustPerfect;
    public int numbers_Auto;
    #endregion

    private GameObject JudgmentOBJ_Get()
    {
        if (judgment_Pool.Count != 0)
        {
            foreach (GameObject judgmentOBJ in judgment_Pool)
            {
                lastOBJ = judgmentOBJ;
                lastOBJ_Coroutine = StartCoroutine(JudgmentOBJ_Return(judgmentOBJ));

                judgment_Pool.Remove(judgmentOBJ);
                judgmentOBJ.SetActive(true);
                return judgmentOBJ;
            }
        }
        else
        {
            StopCoroutine(lastOBJ_Coroutine);
            lastOBJ.SetActive(false);

            lastOBJ_Coroutine = StartCoroutine(JudgmentOBJ_Return(lastOBJ));
            judgment_Pool.Remove(lastOBJ);
            lastOBJ.SetActive(true);
            return lastOBJ;
        }

        return null;    // 这句代码不加会报错
    }
    private IEnumerator JudgmentOBJ_Return(GameObject judgmentOBJ)
    {
        // 等待动画播放完毕
        yield return new WaitForSeconds(0.35f);

        // 将对象放回对象池
        judgmentOBJ.SetActive(false);
        judgment_Pool.Add(judgmentOBJ);
    }

    /// <summary>
    /// 进行判定
    /// </summary>
    /// <param name="judgment">从各个音符类中得到的判定</param>
    /// <param name="delayTime">延迟时间</param>
    /// <param name="trackPosition">所在轨道(int)</param>
    public void Judgment(JudgmentText judgment, float delayTime, int trackPosition, bool isAUTOPlay = false)
    {
        GameObject judgmentOBJ = JudgmentOBJ_Get();

        if (judgmentOBJ != null)
        {
            judgmentOBJ.SetActive(true);

            // 先设置位置
            if (GameSettingsMannger.save_Settings.centerDisplayOfJudgmentText)
                judgmentOBJ.transform.position = new Vector3(0, judgmentOBJ.transform.position.y, 0);
            else
                judgmentOBJ.transform.position = new Vector3(TouchManager.Instance.ReturnToWorldCoordinates(trackPosition), judgmentOBJ.transform.position.y, 0);


            Judgment j = judgmentOBJ.GetComponent<Judgment>();

            // 设置延迟时间的文本
            if (delayTime < 0)
                j.delayTime.text = $"<color=#00FFE6>{delayTime}ms</color>";
            else if (delayTime > 0)
                j.delayTime.text = $"<color=#FF5400>+{delayTime}ms</color>";
            else
                j.delayTime.text = $"<color=yellow>0ms</color>";


            // 设置判定文字。-1代表不显示，0代表Fast，1代表Slow
            int fastSlowIndex;
            j.result.sprite = SetJudgmentText(judgment, delayTime, isAUTOPlay, out fastSlowIndex);

            if (fastSlowIndex == 0)
            {
                j.fastSlow.gameObject.SetActive(true);
                j.fastSlow.sprite = fastSlowSprites[0];
                j.delayTime.gameObject.SetActive(true);
            }
            else if (fastSlowIndex == 1)
            {
                j.fastSlow.gameObject.SetActive(true);
                j.fastSlow.sprite = fastSlowSprites[1];
                j.delayTime.gameObject.SetActive(true);
            }
            else
            {
                // -1
                j.fastSlow.gameObject.SetActive(false);
                j.delayTime.gameObject.SetActive(false);
            }

            // 改变分数和生命值
            ChangeScoresAndHealth(judgment, isAUTOPlay);
        }
    }

    // 设置判定文字。-1代表不显示，0代表Fast，1代表Slow
    Sprite SetJudgmentText(JudgmentText judgment, float delayTime, bool isAUTOPlay, out int fastslowSpriteIndex)
    {
        // Auto模式
        if (isAUTOPlay == true)
        {
            fastslowSpriteIndex = -1;

            return judgmentSprites[6];
        }

        // 常规判断
        if (judgment == JudgmentText.JustPerfect)
        {
            fastslowSpriteIndex = -1;
            return judgmentSprites[0];
        }
        else if (judgment == JudgmentText.Fast_JustPerfect || judgment == JudgmentText.Slow_JustPerfect)
        {
            if (GameSettingsMannger.save_Settings.fastSlowDisplayThreshold == FastSlowDisplayThreshold.Above10ms)
            {
                if (delayTime < -10)
                    fastslowSpriteIndex = 0;
                else if (10 < delayTime)
                    fastslowSpriteIndex = 1;
                else fastslowSpriteIndex = -1;
            }
            else fastslowSpriteIndex = -1;

            return judgmentSprites[0];
        }
        else if (judgment == JudgmentText.Fast_Perfect || judgment == JudgmentText.Slow_Perfect)
        {
            if (GameSettingsMannger.save_Settings.fastSlowDisplayThreshold == FastSlowDisplayThreshold.PerfectBelow)
            {
                if (judgment == JudgmentText.Fast_Perfect)
                    fastslowSpriteIndex = 0;
                else if (judgment == JudgmentText.Slow_Perfect)
                    fastslowSpriteIndex = 1;
                else fastslowSpriteIndex = -1;
            }
            else fastslowSpriteIndex = -1;

            return judgmentSprites[1];
        }
        else if (judgment == JudgmentText.Fast_Great || judgment == JudgmentText.Slow_Great)
        {
            if (GameSettingsMannger.save_Settings.fastSlowDisplayThreshold == FastSlowDisplayThreshold.GreatBelow)
            {
                if (judgment == JudgmentText.Fast_Great)
                    fastslowSpriteIndex = 0;
                else if (judgment == JudgmentText.Slow_Great)
                    fastslowSpriteIndex = 1;
                else fastslowSpriteIndex = -1;
            }
            else fastslowSpriteIndex = -1;

            return judgmentSprites[2];
        }
        else if (judgment == JudgmentText.Fast_Good || judgment == JudgmentText.Slow_Good)
        {
            if (GameSettingsMannger.save_Settings.fastSlowDisplayThreshold == FastSlowDisplayThreshold.GoodBelow)
            {
                if (judgment == JudgmentText.Fast_Good)
                    fastslowSpriteIndex = 0;
                else if (judgment == JudgmentText.Slow_Good)
                    fastslowSpriteIndex = 1;
                else fastslowSpriteIndex = -1;
            }
            else fastslowSpriteIndex = -1;

            return judgmentSprites[3];
        }
        else if (judgment == JudgmentText.Fast_Bad || judgment == JudgmentText.Slow_Bad)
        {
            if (GameSettingsMannger.save_Settings.fastSlowDisplayThreshold != FastSlowDisplayThreshold.Disable)
            {
                if (judgment == JudgmentText.Fast_Bad)
                    fastslowSpriteIndex = 0;
                else if (judgment == JudgmentText.Slow_Bad)
                    fastslowSpriteIndex = 1;
                else fastslowSpriteIndex = -1;
            }
            else fastslowSpriteIndex = -1;

            return judgmentSprites[4];
        }
        else if (judgment == JudgmentText.Fast_Miss)
        {
            if (GameSettingsMannger.save_Settings.fastSlowDisplayThreshold != FastSlowDisplayThreshold.Disable)
                fastslowSpriteIndex = 0;
            else fastslowSpriteIndex = -1;

            return judgmentSprites[5];
        }
        else
        {
            if (GameSettingsMannger.save_Settings.fastSlowDisplayThreshold != FastSlowDisplayThreshold.Disable)
                fastslowSpriteIndex = 1;
            else fastslowSpriteIndex = -1;

            return judgmentSprites[5];
        }
    }

    // 改变分数和生命值
    void ChangeScoresAndHealth(JudgmentText judgment, bool isAUTOPlay)
    {
        // Auto模式
        if (isAUTOPlay == true)
        {
            ScoreBar_Score.Instance.AddScore(GameStateManager.Instance.scoringBenchmark * 0.2f);
            numbers_Auto++;
            return;
        }

        // 常规判断
        if (judgment == JudgmentText.JustPerfect || judgment == JudgmentText.Fast_JustPerfect || judgment == JudgmentText.Slow_JustPerfect)
        {
            ScoreBar_Score.Instance.AddScore(GameStateManager.Instance.scoringBenchmark * 1.05f);
            HealthBar.Instance.AddHealth(3);

            if (judgment == JudgmentText.JustPerfect)
                numbers_JustPerfect++;
            else if (judgment == JudgmentText.Fast_JustPerfect)
                FastNumbers[0]++;
            else if (judgment == JudgmentText.Slow_JustPerfect)
                SlowNumbers[0]++;
        }
        else if (judgment == JudgmentText.Fast_Perfect || judgment == JudgmentText.Slow_Perfect)
        {
            ScoreBar_Score.Instance.AddScore(GameStateManager.Instance.scoringBenchmark);
            HealthBar.Instance.AddHealth(2);

            numbers_Perfect++;
            if (judgment == JudgmentText.Fast_Perfect)
                FastNumbers[1]++;
            else if (judgment == JudgmentText.Slow_Perfect)
                SlowNumbers[1]++;
        }
        else if (judgment == JudgmentText.Fast_Great || judgment == JudgmentText.Slow_Great)
        {
            ScoreBar_Score.Instance.AddScore(GameStateManager.Instance.scoringBenchmark * 0.9f);
            HealthBar.Instance.AddHealth(1);

            numbers_Great++;
            if (judgment == JudgmentText.Fast_Great)
                FastNumbers[2]++;
            else if (judgment == JudgmentText.Slow_Great)
                SlowNumbers[2]++;
        }
        else if (judgment == JudgmentText.Fast_Good || judgment == JudgmentText.Slow_Good)
        {
            ScoreBar_Score.Instance.AddScore(GameStateManager.Instance.scoringBenchmark * 0.6f);

            numbers_Good++;
            if (judgment == JudgmentText.Fast_Good)
                FastNumbers[3]++;
            else if (judgment == JudgmentText.Slow_Good)
                SlowNumbers[3]++;
        }
        else if (judgment == JudgmentText.Fast_Bad || judgment == JudgmentText.Slow_Bad)
        {
            ScoreBar_Score.Instance.AddScore(GameStateManager.Instance.scoringBenchmark * 0.25f);
            HealthBar.Instance.RemoveHealth(60);

            numbers_Bad++;
            if (judgment == JudgmentText.Fast_Bad)
                FastNumbers[4]++;
            else if (judgment == JudgmentText.Slow_Bad)
                SlowNumbers[4]++;
        }
        else if (judgment == JudgmentText.Fast_Miss)
        {
            HealthBar.Instance.RemoveHealth(100);

            numbers_Miss++;
            FastNumbers[5]++;
        }
        else
        {
            HealthBar.Instance.RemoveHealth(100);

            numbers_Miss++;
            SlowNumbers[5]++;
        }
    }
}
public enum JudgmentText
{
    Fast_Miss, Fast_Bad, Fast_Good, Fast_Great, Fast_Perfect, Fast_JustPerfect,
    JustPerfect,
    Slow_Miss, Slow_Bad, Slow_Good, Slow_Great, Slow_Perfect, Slow_JustPerfect,
    AUTO
}