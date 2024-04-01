using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings_ScrollSpeed : MonoBehaviour
{
    public float changeValue;
    public float changeValue_More;

    public float Value_Max;
    public float Value_Min;

    public TextMeshProUGUI Value;

    private void Start()
    {
        Value.text = GameSettingsMannger.save_Settings.scrollSpeed.ToString("F1");
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
            GameSettingsMannger.save_Settings.scrollSpeed -= changeValue_More;
        else
            GameSettingsMannger.save_Settings.scrollSpeed -= changeValue;

        if (GameSettingsMannger.save_Settings.scrollSpeed < Value_Min)
            GameSettingsMannger.save_Settings.scrollSpeed = Value_Min;

        Value.text = GameSettingsMannger.save_Settings.scrollSpeed.ToString("F1");
        GameSettingsMannger.Instance.SaveSettings();
    }
    private void Add_Functions_Common(bool isMore)
    {
        if (isMore)
            GameSettingsMannger.save_Settings.scrollSpeed += changeValue_More;
        else
            GameSettingsMannger.save_Settings.scrollSpeed += changeValue;

        if (GameSettingsMannger.save_Settings.scrollSpeed > Value_Max)
            GameSettingsMannger.save_Settings.scrollSpeed = Value_Max;

        Value.text = GameSettingsMannger.save_Settings.scrollSpeed.ToString("F1");
        GameSettingsMannger.Instance.SaveSettings();
    }
}
