using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 适合多个Toggle切换Enum
public class Settings_GeneralInterfaceFramerate : MonoBehaviour
{
    public Toggle toggleOption1;
    public Toggle toggleOption2;
    public Toggle toggleOption3;
    public Toggle toggleOption4;
    public Toggle toggleOption5;

    private void Start()
    {
        // 从数据持久化类中获取保存的Enum值
        Framerate savedEnumValue = GameSettingsMannger.save_Settings.generalInterfaceFramerate;

        // 设置Toggle组件的状态，使其与数据持久化的值匹配
        SetToggleStateBasedOnEnum(savedEnumValue);

        // 添加监听器
        toggleOption1.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption1); });
        toggleOption2.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption2); });
        toggleOption3.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption3); });
        //toggleOption4.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption4); });
        //toggleOption5.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggleOption5); });
    }

    /// <summary>
    /// 根据Enum值设置Toggle组件的状态
    /// </summary>
    /// <param name="enumValue"></param>
    private void SetToggleStateBasedOnEnum(Framerate enumValue)
    {

        toggleOption1.isOn = (enumValue == Framerate._30);
        toggleOption2.isOn = (enumValue == Framerate._60);
        toggleOption3.isOn = (enumValue == Framerate._120);
        //toggleOption4.isOn = (enumValue == Framerate.Option4);
        //toggleOption5.isOn = (enumValue == Framerate.Option5);
    }

    /// <summary>
    /// Toggle值变化时的回调函数
    /// </summary>
    /// <param name="toggle">变化的Toggle</param>
    private void OnToggleValueChanged(Toggle toggle)
    {
        // 根据选中的Toggle执行对应的特定代码
        if (toggle == toggleOption1)
        {
            GameSettingsMannger.save_Settings.generalInterfaceFramerate = Framerate._30;
            Application.targetFrameRate = 30;
        }
        else if (toggle == toggleOption2)
        {
            GameSettingsMannger.save_Settings.generalInterfaceFramerate = Framerate._60;
            Application.targetFrameRate = 60;
        }
        else if (toggle == toggleOption3)
        {
            GameSettingsMannger.save_Settings.generalInterfaceFramerate = Framerate._120;
            Application.targetFrameRate = 120;
        }
        else if (toggle == toggleOption4)
            Debug.LogWarning("Settings_JudgementMode：Toggle4 没有对应的特定代码。");
        else if (toggle == toggleOption5)
            Debug.LogWarning("Settings_JudgementMode：Toggle5 没有对应的特定代码。");

        GameSettingsMannger.Instance.SaveSettings();
    }
}
