using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(ClickCount);
    }

    int click;
    void ClickCount()
    {
        click++;
        if (10 <= click)
        {
            SwitchDebugMode();
            click = 0;
        }
    }

    void SwitchDebugMode()
    {
        if (GameSettingsMannger.save_Settings.debugMode == false)
            PanelMannger.Instance.Create_PopUP("调试模式", "确认开启调试模式吗？\n这只对于开发调试会有所帮助。\n部分调试内容需要重启游戏后才可生效。",
                new Set_PopUP_Button("取消", () => { }),
                new Set_PopUP_Button("确认", () => { EnableDebuggingMode(); }));
        else
            PanelMannger.Instance.Create_PopUP("调试模式", "确认关闭调试模式吗？\n调试模式只对于开发调试会有所帮助。\n部分调试内容需要重启游戏后才可关闭。",
                new Set_PopUP_Button("取消", () => { }),
                new Set_PopUP_Button("确认", () => { DisableDebuggingMode(); }));
    }

    // 开启
    void EnableDebuggingMode()
    {
        GameSettingsMannger.save_Settings.debugMode = true;
        GameSettingsMannger.Instance.SaveSettings();
    }

    // 禁用
    void DisableDebuggingMode()
    {
        GameSettingsMannger.save_Settings.debugMode = false;
        GameSettingsMannger.Instance.SaveSettings();
    }
}
