using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 适合单个Toggle对应单个设置项
public class Settings_PerformanceSettings : MonoBehaviour
{
    public Toggle toggleOption1;
    public Toggle toggleOption2;
    public Toggle toggleOption3;
    public Toggle toggleOption4;
    public Toggle toggleOption5;

    private void Start()
    {
        // 设置每个Toggle的初始状态
        toggleOption1.isOn = GameSettingsMannger.save_Settings.screenShake;
        toggleOption2.isOn = GameSettingsMannger.save_Settings.groovyPerformance;
        //toggleOption3.isOn = Game_Settings_Mannger.save_Settings.设置项;
        //toggleOption4.isOn = Game_Settings_Mannger.save_Settings.设置项;
        //toggleOption5.isOn = Game_Settings_Mannger.save_Settings.设置项;

        // 添加监听器
        toggleOption1.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption1); });
        toggleOption2.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption2); });
        //toggleOption3.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption3); });
        //toggleOption4.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption4); });
        //toggleOption5.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption5); });
    }

    // Toggle值变化时的回调函数
    private void OnToggleValueChanged(Toggle toggle)
    {
        if (toggle == toggleOption1)
            GameSettingsMannger.save_Settings.screenShake = toggle.isOn;
        else if (toggle == toggleOption2)
            GameSettingsMannger.save_Settings.groovyPerformance = toggle.isOn;
        else if (toggle == toggleOption3)
            Debug.LogWarning("Settings_DisplaySettings：没有toggleOption3的对应操作。");
        else if (toggle == toggleOption4)
            Debug.LogWarning("Settings_DisplaySettings：没有toggleOption4的对应操作。");
        else if (toggle == toggleOption5)
            Debug.LogWarning("Settings_DisplaySettings：没有toggleOption5的对应操作。");

        GameSettingsMannger.Instance.SaveSettings();
    }
}
