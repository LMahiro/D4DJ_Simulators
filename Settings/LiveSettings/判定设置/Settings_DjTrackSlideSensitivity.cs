using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings_DjTrackSlideSensitivity : MonoBehaviour
{
    public float changeValue;
    public float changeValue_More;

    public float Value_Max;
    public float Value_Min;

    public TextMeshProUGUI Value;

    private void Start()
    {
        Value.text = (GameSettingsMannger.save_Settings.djTrackSlideSensitivity * 100).ToString("F0") + "%";
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
            GameSettingsMannger.save_Settings.djTrackSlideSensitivity -= changeValue_More;
        else
            GameSettingsMannger.save_Settings.djTrackSlideSensitivity -= changeValue;

        if (GameSettingsMannger.save_Settings.djTrackSlideSensitivity < Value_Min)
            GameSettingsMannger.save_Settings.djTrackSlideSensitivity = Value_Min;

        Value.text = (GameSettingsMannger.save_Settings.djTrackSlideSensitivity * 100).ToString("F0") + "%";
        GameSettingsMannger.Instance.SaveSettings();
    }
    private void Add_Functions_Common(bool isMore)
    {
        if (isMore)
            GameSettingsMannger.save_Settings.djTrackSlideSensitivity += changeValue_More;
        else
            GameSettingsMannger.save_Settings.djTrackSlideSensitivity += changeValue;

        if (GameSettingsMannger.save_Settings.djTrackSlideSensitivity > Value_Max)
            GameSettingsMannger.save_Settings.djTrackSlideSensitivity = Value_Max;

        Value.text = (GameSettingsMannger.save_Settings.djTrackSlideSensitivity * 100).ToString("F0") + "%";
        GameSettingsMannger.Instance.SaveSettings();
    }
}
