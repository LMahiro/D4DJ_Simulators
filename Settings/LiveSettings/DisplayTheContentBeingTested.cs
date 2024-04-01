using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTheContentBeingTested : MonoBehaviour
{
    public Button switchButton;

    [Header("拖入隐藏的内容")]
    public GameObject[] hiddenContents;
    [Header("拖入按钮对钩")]
    public Image hook;


    private void Awake()
    {
        if (GameSettingsMannger.save_Settings.displayTheContentBeingTested)
            ShowContent();
        else
            HideContent();
    }
    public void _OnButtonClick()
    {
        if (GameSettingsMannger.save_Settings.displayTheContentBeingTested)
        {
            SoundManager.Instance.Play_SE("event:/SE/Normal/Exchange_Unsure");
            HideContent();
            GameSettingsMannger.save_Settings.displayTheContentBeingTested = false;
            GameSettingsMannger.Instance.SaveSettings();
        }
        else
        {
            SoundManager.Instance.Play_SE("event:/SE/Normal/Exchange_Sure");
            PanelMannger.Instance.Create_PopUP("显示正在测试的内容",
                "确认显示正在测试的内容吗？\n这些内容依然在测试，可能会使得软件不稳定\n\n请注意：显示后即使再隐藏，已修改的值依然会被保存并应用",
                new Set_PopUP_Button("取消", () => { }),
                new Set_PopUP_Button("确定", () =>
                {
                    ShowContent();
                    GameSettingsMannger.save_Settings.displayTheContentBeingTested = true;
                    GameSettingsMannger.Instance.SaveSettings();
                }));
        }
    }   // 按钮操作
    public void HideContent()
    {
        hook.gameObject.SetActive(false);

        for (int i = 0; i < hiddenContents.Length; i++)
        {
            hiddenContents[i].SetActive(false);
        }
    }
    public void ShowContent()
    {
        hook.gameObject.SetActive(true);

        for (int i = 0; i < hiddenContents.Length; i++)
        {
            hiddenContents[i].SetActive(true);
        }
    }
}
