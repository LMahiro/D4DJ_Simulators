using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

public class SelectFile : MonoBehaviour
{
    [Header("Test按钮")]
    public Button testButton;
    private void Awake()
    {
        if (GameSettingsMannger.save_Settings.debugMode)
            testButton.gameObject.SetActive(true);
        else
            testButton.gameObject.SetActive(false);
    }

    [Header("拖入自身以及子对象下的对应组件")]
    public CanvasGroup canvasGroup;
    public Animator animator;

    // 关闭面板
    private void Hide()
    {
        SoundManager.Instance.Pause_PlayerUpload_Music();

        canvasGroup.interactable = false;
        StartCoroutine(WaitingForAnimation_ToHide());
    }
    IEnumerator WaitingForAnimation_ToHide()
    {
        animator.Play("PopUP_Hide");
        yield return new WaitForSeconds(0.16667f);      // 10 - 60帧的动画
        Destroy(this.gameObject);
    }

    // 使用填写的内容
    private void SaveData()
    {
        SoundManager.Instance.Stop_Song();

        EditChart_Base.Instance.ValueReadCompleted();
        Hide();
    }

    // 底部按钮方法
    public void _Button_True()
    {
        if (audio_Path.text == "请选择文件...") { Create_PopUPPanel(-11); return; }
        if (isAudioPlayed == false) { Create_PopUPPanel(-15); return; }
        if (EditChart_Base.Instance.cover_Sprite == null) { Create_PopUPPanel(-12); return; }
        //if (isCreateNewChart)
        //{
        if (EditChart_Base.Instance.chart.information.songName == null ||
            EditChart_Base.Instance.chart.information.songWriter == null ||
            EditChart_Base.Instance.chart.information.chartWriter == null ||
            EditChart_Base.Instance.chart.information.chartLevel == 0)
        { Create_PopUPPanel(-14); return; }
        //}
        //else
        //    if (EditChart_Base.Instance.chart.noteDataList.Count == 0) { Create_PopUPPanel(-13); return; }

        Create_PopUPPanel(-19);
    }
    public void _Button_Cancel()
    {
        Create_PopUPPanel(-18);
    }
    public void _Button_Test()
    {
        SaveData();
    }


    [Header("音频文件路径相关")]
    public TextMeshProUGUI audio_Path;
    public void _Specify_Audio()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("所指定的音频文件", ".wav", ".mp3"));
        FileBrowser.SetDefaultFilter(".wav");

        PanelMannger.Instance.Show_NowLoading_Panel();
        StartCoroutine(LoadingForPlayers("选择.wav或.mp3音频文件", Specify_Audio_Action, ".wav", ".mp3"));
    }
    private string audio_ReadPath;
    private bool isAudioPlayed = false;     // 判断是否试听过（加载音频资源过）
    private void Specify_Audio_Action()
    {
        // 先删除旧内容
        audio_ReadPath = Application.persistentDataPath + "\\Edit";

        if (Directory.Exists(audio_ReadPath))
            Directory.Delete(audio_ReadPath, true);
        Directory.CreateDirectory(audio_ReadPath);

        // 再复制新内容
        audio_ReadPath = Application.persistentDataPath + "\\Edit\\" + FileBrowserHelpers.GetFilename(FileBrowser.Result[0]);

        FileBrowserHelpers.CopyFile(FileBrowser.Result[0], audio_ReadPath);

        // FMOD是进不了try-catch的
        SoundManager.Instance.Stop_PlayerUpload_Music();
        SoundManager.Instance.Get_PlayerUpload_Music(audio_ReadPath);

        audio_Path.text = FileBrowser.Result[0];
        Create_PopUPPanel(9);
    }
    public void _Play_UploadAudio()
    {
        SoundManager.Instance.Stop_Song();
        SoundManager.Instance.Play_PlayerUpload_Music();
        isAudioPlayed = true;
    }
    public void _Pause_UploadAudio()
    {
        SoundManager.Instance.Pause_PlayerUpload_Music();
        SoundManager.Instance.Play_Song();
    }


    [Header("封面文件路径相关")]
    public TextMeshProUGUI cover_Path;
    public Image cover_Image;
    public void _Specify_Cover()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("所指定的封面图片", ".png", ".jpg"));
        FileBrowser.SetDefaultFilter(".png");

        PanelMannger.Instance.Show_NowLoading_Panel();
        StartCoroutine(LoadingForPlayers("选择.png或.jpg封面图片文件", Specify_Cover_Action, ".png", ".jpg"));
    }
    private void Specify_Cover_Action()
    {
        // 尝试读取并设置
        byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
        Texture2D texture2D = new Texture2D(2, 2);
        if (!texture2D.LoadImage(bytes))    // 会修改texture2D的大小为读取后的图片的大小
        {
            Create_PopUPPanel(11); return;
        }

        // 图片不是正方形
        if (texture2D.width != texture2D.height)
            Create_PopUPPanel(12);
        else
        {
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                                                     new Vector2(0.5f, 0.5f));

            EditChart_Base.Instance.cover_Sprite = sprite;
            cover_Image.sprite = sprite;
            cover_Path.text = FileBrowser.Result[0];

            Create_PopUPPanel(19);
        }
    }


    //[Header("谱面文件路径相关")]
    //public TextMeshProUGUI chart_Path;
    //public void _Specify_Chart()
    //{
    //    FileBrowser.SetFilters(true, new FileBrowser.Filter("所指定的文本文档", ".json"));
    //    FileBrowser.SetDefaultFilter(".json");

    //    PanelMannger.Instance.Show_NowLoading_Panel();
    //    StartCoroutine(LoadingForPlayers("选择.json谱面文件", Specify_Chart_Action, ".json"));
    //}
    //private void Specify_Chart_Action()
    //{
    //    string chart_String = FileBrowserHelpers.ReadTextFromFile(FileBrowser.Result[0]);
    //    Chart chart;

    //    try { chart = JsonMannger.Instance.LoadJsonData_DefaultPath<Chart>(chart_String); }
    //    catch { Create_PopUPPanel(21); return; }

    //    EditChart_Base.Instance.chart = chart;
    //    chart_Path.text = FileBrowser.Result[0];
    //    isCreateNewChart = false;
    //    image_Disable.gameObject.SetActive(true);
    //}
    //public void _New_Chart()
    //{
    //    Create_PopUPPanel(28);
    //    chart_Path.text = "将在关闭面板后创建新谱面";
    //    image_Disable.gameObject.SetActive(false);
    //    isCreateNewChart = true;
    //}


    //[Header("谱面基本信息")]
    //public Image image_Disable;
    //private bool isCreateNewChart;
    public void _OnEndEdit_SongName(string input)
    {
        EditChart_Base.Instance.chart.information.songName = input;
    }
    public void _OnEndEdit_SongWriter(string input)
    {
        EditChart_Base.Instance.chart.information.songWriter = input;
    }
    public void _OnEndEdit_ChartWriter(string input)
    {
        EditChart_Base.Instance.chart.information.chartWriter = input;
    }
    public void _OnValueChanged_Difficulty(int input)
    {
        switch (input)
        {
            case 0:
                EditChart_Base.Instance.chart.information.chartDifficulty = Chart_Difficulty.EASY;
                break;
            case 1:
                EditChart_Base.Instance.chart.information.chartDifficulty = Chart_Difficulty.NORMAL;
                break;
            case 2:
                EditChart_Base.Instance.chart.information.chartDifficulty = Chart_Difficulty.HARD;
                break;
            case 3:
                EditChart_Base.Instance.chart.information.chartDifficulty = Chart_Difficulty.EXPERT;
                break;
            default:
                Create_PopUPPanel(31);
                break;
        }
    }
    public void _OnEndEdit_Level(string input)
    {
        int value = int.Parse(input);
        if (value <= 0) { Create_PopUPPanel(32); return; }
        if (20 < value) { Create_PopUPPanel(33); return; }

        EditChart_Base.Instance.chart.information.chartLevel = value;
    }


    // 说明
    public void _Illustrate()
    {
        PanelMannger.Instance.Create_PopUP("说明", "对于音频文件，请使用.wav或.mp3的文件格式。歌曲时长不应超过10分钟\n" +
                                            "对于封面文件，请使用.png或.jpg的文件格式。封面图片的长宽像素应一致\n\n" +
                                            "若你是使用安卓10+系统的设备，在打开\"选择文件...\"窗口后，由于没有权限导致没有路径可读取\n" +
                                            "此时请点击左上角的\"Browse...\"选项，选择一个允许模拟器访问的文件夹，并进行授权\n" +
                                            "随后就可在\"选择文件...\"窗口中查找已授权的文件夹中的内容了\n\n" +
                                            "若你使用的是Windows系统的设备，请不要将软件以及资源放在需要管理员权限的目录\n" +
                                            "若读取失败，在确定源文件没有问题后，可以尝试使用管理员身份运行模拟器",
                                            new Set_PopUP_Button("确认", () => { }));
    }


    // 选择路径，选择好后会调用传入的方法，实现不同的逻辑
    IEnumerator LoadingForPlayers(string title, UnityAction action, params string[] fileExtension)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files,
                                                    false,
                                                    null,
                                                    null,
                                                    title,
                                                    "选择");

        if (FileBrowser.Success)
        {
            // 检测后缀是否正确
            string read_RileExtension = FileBrowser.Result[0].ToString();
            for (int i = 0; i < fileExtension.Length;)
            {
                if (read_RileExtension.Contains(fileExtension[i]))
                {
                    if (action != null)     // 执行各个方法的函数
                        action();

                    break;                  // 发现匹配值，跳出循环
                }
                else
                {
                    i++;
                    if (i == fileExtension.Length)
                        Create_PopUPPanel(-1);
                }
            }
        }
        else
            Create_PopUPPanel(0);

        PanelMannger.Instance.Hide_NowLoading_Panel();
    }


    // 各种弹窗说明
    private void Create_PopUPPanel(int messageIndex)
    {
        switch (messageIndex)
        {
            // 确认和取消按钮
            case -11:
                PanelMannger.Instance.Create_PopUP("确认", "缺失音频文件数据\n请再次检查。",
                                            new Set_PopUP_Button("确认", () => { }));
                break;
            case -12:
                PanelMannger.Instance.Create_PopUP("确认", "缺失封面文件数据\n请再次检查。",
                                            new Set_PopUP_Button("确认", () => { }));
                break;
            //case -13:
            //    PanelMannger.Instance.Create_PopUP("确认", "缺失谱面文件数据\n请再次检查。",
            //                           new Set_PopUP_Button("确认", () => { }));
            //    break;
            case -14:
                PanelMannger.Instance.Create_PopUP("确认", "谱面基本信息有内容未填写\n请再次检查。",
                                       new Set_PopUP_Button("确认", () => { }));
                break;
            case -15:
                PanelMannger.Instance.Create_PopUP("确认", "请先试听传入的音频文件\n以确认音频文件没有问题",
                                       new Set_PopUP_Button("确认", () => { }));
                break;
            case -18:
                PanelMannger.Instance.Create_PopUP("确认关闭？", "点击取消关闭面板后，\n所有已设置的值将会被清空。",
                    new Set_PopUP_Button("取消", () => { }),
                    new Set_PopUP_Button("确认", () => { Hide(); }));
                break;
            case -19:
                PanelMannger.Instance.Create_PopUP("确认", "请确认所有内容填写正确，\n软件将使用所填内容设置数据。",
                                            new Set_PopUP_Button("取消", () => { }),
                                            new Set_PopUP_Button("确认", () => { SaveData(); }));

                break;

            // 路径读取时
            case 0:
                PanelMannger.Instance.Create_PopUP("确认", "你取消了本次文件读取的操作。",
                                                new Set_PopUP_Button("确认", () => { }));
                break;
            case -1:
                PanelMannger.Instance.Create_PopUP("读取失败", "不是所指定后缀的文件" +
                                                "\n\n※不要点击右下角的\"所有文件\"来修改文件选择范围",
                                                new Set_PopUP_Button("确认", () => { }));
                break;

            // 读取音频文件时
            case 9:
                PanelMannger.Instance.Create_PopUP("读取成功", "已成功读取文件\n请在右侧预览确定文件是否正确读取。" +
                                                "\n\n※若内容存在问题，请优先检查文件自身问题，随后尝试重新导入。",
                                                new Set_PopUP_Button("确认", () => { }));
                break;

            // 读取封面图片时
            case 11:
                PanelMannger.Instance.Create_PopUP("读取失败", "图片文件错误" +
                                                "\n\n※请检查文件自身是否存在问题，然后重试。",
                                                new Set_PopUP_Button("确认", () => { }));
                break;
            case 12:
                PanelMannger.Instance.Create_PopUP("读取失败", "所选择的图片尺寸不正确" +
                                                "\n\n※请检查图片尺寸，图片的长宽大小应一致。",
                                                new Set_PopUP_Button("确认", () => { }));
                break;
            case 19:
                PanelMannger.Instance.Create_PopUP("读取成功", "已成功读取文件\n请在右侧预览确定文件是否正确读取。" +
                                                "\n\n※若内容存在问题，请优先检查文件自身问题，随后尝试重新导入。",
                                                new Set_PopUP_Button("确认", () => { }));
                break;

            // 读取谱面文件时
            //case 21:
            //    PanelMannger.Instance.Create_PopUP("读取失败", "Json文件内容错误（谱面数据错误）" +
            //                                    "\n\n※请检查Json文件的内部格式是否正确，然后重试。",
            //                                    new Set_PopUP_Button("确认", () => { }));
            //    break;
            //case 28:
            //    PanelMannger.Instance.Create_PopUP("确认", "已选择创建新谱面" +
            //                                    "\n\n请完善右侧“谱面基本信息”内容",
            //                                    new Set_PopUP_Button("确认", () => { }));
            //    break;
            //case 29:
            //    PanelMannger.Instance.Create_PopUP("读取成功", "已成功读取谱面文件" +
            //                                    "\n\n将在关闭选择文件面板后加载谱面数据。",
            //                                    new Set_PopUP_Button("确认", () => { }));
            //    break;

            // 创建新谱面，填写谱面相关信息时
            case 31:
                PanelMannger.Instance.Create_PopUP("遇到了奇怪的问题", "超出难度选择范围" +
                                                "\n\n正常情况下该问题不应发生，请重试。",
                                                new Set_PopUP_Button("确认", () => { Hide(); }));
                break;
            case 32:
                PanelMannger.Instance.Create_PopUP("确认", "等级不能为负数或0" +
                                                "\n\n本次输入将作废，请重新填写。",
                                                new Set_PopUP_Button("确认", () => { }));
                break;
            case 33:
                PanelMannger.Instance.Create_PopUP("确认", "等级的上限为20级(含)" +
                                                "\n\n本次输入将作废，请重新填写。",
                                                new Set_PopUP_Button("确认", () => { }));
                break;
        }
    }
}
