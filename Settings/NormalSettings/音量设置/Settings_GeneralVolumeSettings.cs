using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// TIP：该设置项仅修改Slide滑动条的相关内容
public class Settings_GeneralVolumeSettings : MonoBehaviour
{
    public Slider sliderOption1;
    public Slider sliderOption2;
    public Slider sliderOption3;
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;

    private void Start()
    {
        // 设置每个Slider的初始值
        SetInitialValue();
    }

    private void SetInitialValue()
    {
        sliderOption1.value = GameSettingsMannger.save_Settings.generalMusicVolume;
        sliderOption2.value = GameSettingsMannger.save_Settings.generalSoundEffectVolume;
        sliderOption3.value = GameSettingsMannger.save_Settings.generalVoiceVolume;

        text1.text = (GameSettingsMannger.save_Settings.generalMusicVolume * 100).ToString("F0") + "%";
        text2.text = (GameSettingsMannger.save_Settings.generalSoundEffectVolume * 100).ToString("F0") + "%";
        text3.text = (GameSettingsMannger.save_Settings.generalVoiceVolume * 100).ToString("F0") + "%";

        // 添加监听器
        sliderOption1.onValueChanged.AddListener(delegate { OnToggleValueChanged(sliderOption1); });
        sliderOption2.onValueChanged.AddListener(delegate { OnToggleValueChanged(sliderOption2); });
        sliderOption3.onValueChanged.AddListener(delegate { OnToggleValueChanged(sliderOption3); });
    }

    // Slider值变化时的回调函数
    private void OnToggleValueChanged(Slider slider)
    {
        if (slider == sliderOption1)
        {
            GameSettingsMannger.save_Settings.generalMusicVolume = slider.value;
            text1.text = (GameSettingsMannger.save_Settings.generalMusicVolume * 100).ToString("F0") + "%";
        }   
        else if (slider == sliderOption2)
        {
            GameSettingsMannger.save_Settings.generalSoundEffectVolume = slider.value;
            text2.text = (GameSettingsMannger.save_Settings.generalSoundEffectVolume * 100).ToString("F0") + "%";
        }
        else if (slider == sliderOption3)
        {
            GameSettingsMannger.save_Settings.generalVoiceVolume = slider.value;
            text3.text = (GameSettingsMannger.save_Settings.generalVoiceVolume * 100).ToString("F0") + "%";
        }

        GameSettingsMannger.Instance.SaveSettings();
    }
}
