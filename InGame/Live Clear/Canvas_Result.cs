using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Canvas_Result : MonoBehaviour
{
    private static Canvas_Result instance;
    public static Canvas_Result Instance => instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this.gameObject);

        this.gameObject.SetActive(false);
    }
    public Image difficultImage;
    public TextMeshProUGUI difficultText;

    public TextMeshProUGUI songName;

    public TextMeshProUGUI chartLevel;


    public TextMeshProUGUI preciseValue;
    public TextMeshProUGUI[] fastValue;
    public TextMeshProUGUI[] slowValue;

    public GameObject combo;
    public TextMeshProUGUI comboValue;

    public GameObject auto;
    public TextMeshProUGUI autoValue;

    public void Show()
    {
        this.gameObject.SetActive(true);
        InGameSoundManager.Instance.Play_Song("event:/BGM/LiveClear");

        switch (GameStateManager.Instance.chart.information.chartDifficulty)
        {
            case Chart_Difficulty.EASY:
                difficultImage.color = new Color(0, 0.60392f, 1, 1);
                difficultText.text = "EASY";
                break;
            case Chart_Difficulty.NORMAL:
                difficultImage.color = new Color(0, 1, 0, 1);
                difficultText.text = "NORMAL";
                break;
            case Chart_Difficulty.HARD:
                difficultImage.color = new Color(1, 0.6196f, 0, 1);
                difficultText.text = "HARD";
                break;
            case Chart_Difficulty.EXPERT:
                difficultImage.color = new Color(1, 0, 0, 1);
                difficultText.text = "EXPENT";
                break;
        }

        songName.text = GameStateManager.Instance.chart.information.songName;

        chartLevel.text = $"Level  {GameStateManager.Instance.chart.information.chartLevel}";

        preciseValue.text = JudgmentManager.Instance.numbers_JustPerfect.ToString();
        for (int i = 0; i < 6; i++)
        {
            fastValue[i].text = JudgmentManager.Instance.FastNumbers[i].ToString();
            slowValue[i].text = JudgmentManager.Instance.SlowNumbers[i].ToString();
        }

        if (GameStateManager.Instance.isAutoPlay)
        {
            combo.SetActive(false);
            autoValue.text = JudgmentManager.Instance.numbers_Auto.ToString();
        }
        else
        {
            auto.SetActive(false);
            comboValue.text = ComboManager.Instance.maxCombo.ToString();
        }
    }
    public void _Button_Next()
    {
        InGameSoundManager.Instance.PlaySe_Common(InGameSe_Common.Button_True);
        InGameSoundManager.Instance.Stop_Song();
        PassByValue.Instance.SwitchingScenes("Normal");
    }
}
