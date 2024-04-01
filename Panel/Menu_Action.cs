using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SimpleFileBrowser;
using System.IO;

public class Menu_Action : MonoBehaviour
{
    // 版本号
    public TextMeshProUGUI version;
    private void Awake()
    {
        string debugModeText = "";

        if (GameSettingsMannger.save_Settings.debugMode)
            debugModeText = "<color=orange>/Debug Mode";

        version.text = "Version " + Application.version + debugModeText;

        if (!GameSettingsMannger.save_Settings.debugMode)
            TestOBJ.SetActive(false);
    }

    // 返回主界面
    public void _Home()
    {
        PanelMannger.Instance.GoBackTo_MainInterface();
        _CloseMenu();
    }
    // 关闭菜单
    public void _CloseMenu()
    {
        GetComponentInParent<Panel_Self>().Public_FadeOut();
    }


    // 常规设置
    public GameObject normalSetting_OBJ;
    public void _NormalSetting()
    {
        PanelMannger.Instance.SwitchTo_NewPane(normalSetting_OBJ);
        _CloseMenu();
    }

    // Live设置
    public GameObject liveSetting_OBJ;
    public void _LiveSetting()
    {
        PanelMannger.Instance.SwitchTo_NewPane(liveSetting_OBJ);
        _CloseMenu();
    }

    // 歌曲列表
    public GameObject songList_OBJ;
    public void _SongList()
    {
        PanelMannger.Instance.SwitchTo_NewPane(songList_OBJ);
        _CloseMenu();
    }

    // 关于软件
    public GameObject about_OBJ;
    public void _About()
    {
        PanelMannger.Instance.SwitchTo_NewPane(about_OBJ);
        _CloseMenu();
    }

    // 制作谱面
    public GameObject editChart_OBJ;
    public void _EditChart()
    {
        PanelMannger.Instance.SwitchTo_NewPane(editChart_OBJ);
        _CloseMenu();
    }

    // 加入聊天
    public GameObject joinChat_OBJ;
    public void _JoinChat()
    {
        PanelMannger.Instance.SwitchTo_NewPane(joinChat_OBJ);
        _CloseMenu();
    }

    // 观看动漫
    public GameObject watchAnime_OBJ;
    public void _WatchAnime()
    {
        PanelMannger.Instance.SwitchTo_NewPane(watchAnime_OBJ);
        _CloseMenu();
    }

    // DJ Time
    public GameObject djTime_OBJ;
    public void _DJTime()
    {
        PanelMannger.Instance.SwitchTo_NewPane(djTime_OBJ);
        _CloseMenu();
    }

    // 扑克计算器
    public void _PorkerCalculator()
    {
        PanelMannger.Instance.SwitchTo_WebPage("https://porker.vercel.app/");
        _CloseMenu();
    }

    // 排名计算器
    public void _ParkingCalculator()
    {
        PanelMannger.Instance.SwitchTo_WebPage("https://sigonasr2.github.io/d4djparkingcalculator/parkingcalc.html");
        _CloseMenu();
    }

    // Live2D查看器
    public void _Live2DViewer()
    {
        PanelMannger.Instance.SwitchTo_WebPage("https://d4dj.info/live2d");
        _CloseMenu();
    }

    // 卡面查看器
    public void _CardSurfaceViewer()
    {
        PanelMannger.Instance.SwitchTo_WebPage("https://d4dj.info/game/card");
        _CloseMenu();
    }

    // 谱面预览
    public void _SpectralPreview()
    {
        PanelMannger.Instance.SwitchTo_WebPage("https://d4dj.info/game/music");
        _CloseMenu();
    }

    // D4DJ Wiki
    public void _D4DJWiki()
    {
        PanelMannger.Instance.SwitchTo_WebPage("https://d4dj.fandom.com/wiki/Dig_Delight_Direct_Drive_DJ_Wiki");
        _CloseMenu();
    }

    // 官方网站
    public void _OfficialWebsite()
    {
        PanelMannger.Instance.SwitchTo_WebPage("https://d4dj-pj.com/");
        _CloseMenu();
    }

    // 退出
    public void _Exit()
    {
        PanelMannger.Instance.Create_PopUP("确认", "确认退出模拟器？",
            new Set_PopUP_Button("取消", () => { }),
            new Set_PopUP_Button("确定", () => { Application.Quit(); }));
    }

    // 测试用
    public GameObject TestOBJ;
    public void _Test()
    {

        Debug.Log(Application.persistentDataPath);
        string chart_String = FileBrowserHelpers.ReadTextFromFile(Path.Combine(Application.persistentDataPath, "chart.txt"));
        PassByValue.Instance.chart = JsonMannger.Instance.LoadJsonData<Chart>(chart_String);
        SoundManager.Instance.Stop_Song();
        PassByValue.Instance.SwitchingScenes("InGame");

    }
}
