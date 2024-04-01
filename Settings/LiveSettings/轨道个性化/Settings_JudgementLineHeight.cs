using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings_JudgementLineHeight : MonoBehaviour
{
    public int changeValue;
    public int changeValue_More;

    public int Value_Max;
    public int Value_Min;

    public TextMeshProUGUI Value;

    private void Start()
    {
        Value.text = GameSettingsMannger.save_Settings.judgementLineHeight.ToString();
    }

    public void Remove()
    {
        Remove_Functions_Common(false);
    }
    public void Remove_More()
    {
        Remove_Functions_Common(true);
    }
    public void Add()
    {
        Add_Functions_Common(false);
    }
    public void Add_More()
    {
        Add_Functions_Common(true);
    }

    private void Remove_Functions_Common(bool isMore)
    {
        if(isMore)
            GameSettingsMannger.save_Settings.judgementLineHeight -= changeValue_More;
        else
            GameSettingsMannger.save_Settings.judgementLineHeight -= changeValue;

        if (GameSettingsMannger.save_Settings.judgementLineHeight < Value_Min)
            GameSettingsMannger.save_Settings.judgementLineHeight = Value_Min;

        Value.text = GameSettingsMannger.save_Settings.judgementLineHeight.ToString();
        GameSettingsMannger.Instance.SaveSettings();
    }
    private void Add_Functions_Common(bool isMore)
    {
        if (isMore)
            GameSettingsMannger.save_Settings.judgementLineHeight += changeValue_More;
        else
            GameSettingsMannger.save_Settings.judgementLineHeight += changeValue;

        if (GameSettingsMannger.save_Settings.judgementLineHeight > Value_Max)
            GameSettingsMannger.save_Settings.judgementLineHeight = Value_Max;

        Value.text = GameSettingsMannger.save_Settings.judgementLineHeight.ToString();
        GameSettingsMannger.Instance.SaveSettings();
    }
}
