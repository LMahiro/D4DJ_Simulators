using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 适合用2个Toggle做开关
public class Settings_ShowTapEffects : MonoBehaviour
{
    public Toggle toggleON;
    public Toggle toggleOFF;

    private void Start()
    {
        if (GameSettingsMannger.save_Settings.showTapEffects)
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
            TapEffectManager.Instance.Show_TapEffect();
        else
            TapEffectManager.Instance.Hide_TapEffect();

        GameSettingsMannger.Instance.SaveSettings();
    }
}
