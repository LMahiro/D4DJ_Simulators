using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 适合用2个Toggle做开关
public class Settings_MusicVolumeSliderEffect : MonoBehaviour
{
    public Toggle toggleON;
    public Toggle toggleOFF;

    private void Start()
    {
        if (GameSettingsMannger.save_Settings.musicVolumeSliderEffect)
            toggleON.isOn = true;
        else
            toggleOFF.isOn = true;

        // 添加监听器
        toggleON.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleON); });
        toggleOFF.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOFF); });
    }

    /// <summary>
    /// Toggle值变化时的回调函数
    /// </summary>
    /// <param name="toggle">变化的Toggle</param>
    private void OnToggleValueChanged(Toggle toggle)
    {
        // 根据选中的Toggle执行对应的特定代码
        if (toggle == toggleON)
            GameSettingsMannger.save_Settings.musicVolumeSliderEffect = true;
        else
            GameSettingsMannger.save_Settings.musicVolumeSliderEffect = false;

        GameSettingsMannger.Instance.SaveSettings();
    }
}
