using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditChart_Back : MonoBehaviour
{
    public void _Back()
    {
        PanelMannger.Instance.Create_PopUP("确认", "确认退出编辑界面？\n未保存的更改将全部丢失。",
            new Set_PopUP_Button("取消", () => { }),
            new Set_PopUP_Button("确认", ReturnOperation));
    }
    private void ReturnOperation()
    {
        PanelMannger.Instance.GoBackTo_MainInterface();
        PanelMannger.Instance.Button_Show_All();

        SoundManager.Instance.Stop_PlayerUpload_Music();
    }
}
