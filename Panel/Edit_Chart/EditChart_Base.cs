using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using LitJson;

public class EditChart_Base : MonoBehaviour
{
    #region 单例模式
    static EditChart_Base instance;
    public static EditChart_Base Instance => instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        PanelMannger.Instance.Button_Hide_All();

        Add_NoteToggle_Listener();

        if (GameSettingsMannger.save_Settings.debugMode)
            debug_BaseOBJ.SetActive(true);
        else
            debug_BaseOBJ.SetActive(false);
    }
    #endregion

    void Update()
    {
        if (valueReadCompleted)
        {
            Update_MusicBar();          // 更新音乐播放进度条
            TrackPlaybackPosition();    // (播放音乐时)设置轨道Content的位置
            PCSideShortcutOperations(); // PC端快捷操作

            // 放置音符
            if (noteTrack_Clicked && noteTime_Clicked)
                PlacingNotes();

            PlayNoteEffects();          // (播放音乐时)播放音效
            DebuggingFunctions();       // Debug信息更新
        }
    }

    #region 调试模式
    [Header("调试模式")]
    public TMP_InputField debugText;
    public GameObject debug_BaseOBJ;
    public GameObject debug_TextImage;

    string debugString;

    void DebuggingFunctions()
    {
        // Tip：激活失活再Awake中进行

        debugString = $"chart总数：{chart.noteDataList.Count}\n";
        for (int i = 0; i < chart.noteDataList.Count; i++)
        {
            debugString += $"第{i}个元素({chart.noteDataList[i].type})：nextId{chart.noteDataList[i].nextId}、lastId{chart.noteDataList[i].lastId}\n";
        }
        debugText.text = debugString;
    }
    public void _Debug_Enable()
    {
        debug_TextImage.SetActive(true);
    }
    public void _Debug_Fold()
    {
        debug_TextImage.SetActive(false);
    }
    #endregion

    #region 三大按钮、读取基本信息
    [Header("三大按钮创建所需的预制体")]
    public GameObject selectFile_OBJ;
    public void _Create_SelectFile()
    {
        Instantiate(selectFile_OBJ);
    }


    public GameObject displayCode_OBJ;
    public void _Create_DisplayCode()
    {
        Instantiate(displayCode_OBJ);
    }


    public GameObject saveChart_OBJ;
    public void _Create_SaveChart()
    {
        PassByValue.Instance.chart = this.chart;
        PassByValue.Instance.SwitchingScenes("InGame");
        //Instantiate(saveChart_OBJ);
    }


    [Header("谱面编辑不可用")] public GameObject track_NoMusic;
    [HideInInspector] public Chart chart = new Chart();   // 谱面数据
    [HideInInspector] public Sprite cover_Sprite;         // 封面图片

    bool valueReadCompleted = false;
    public void ValueReadCompleted()
    {
        valueReadCompleted = true;
        track_NoMusic.SetActive(false);
        PanelMannger.Instance.Reset_Title("制作谱面（Beta）", chart.information.songName);

        // 初始化编辑谱面
        if (chart.editInformation.Count == 0)
            chart.editInformation.Add(new EditInformation(-0.01f, 120));

        Recalculate_Track_Position();
    }
    #endregion

    #region 右侧音符选择切换
    [Header("音符切换Toggle列表")]
    public Toggle toggle_Null;
    public Toggle toggle_BPM;
    public Toggle toggle_Remove;
    public Toggle toggle_Tap1;
    public Toggle toggle_Tap2;
    public Toggle toggle_Hold;
    public Toggle toggle_Slide;
    public Toggle toggle_Scratch;
    public Toggle toggle_Stop;

    [Header("操作面板列表")]
    public GameObject noteSettings_Empty;
    public GameObject noteSettings_BPM;
    public GameObject noteSettings_Slide;

    void Add_NoteToggle_Listener()
    {
        // 监听Toggle状态变化事件
        toggle_Null.onValueChanged.AddListener((value) => OnToggleValueChanged(value, noteSettings_Empty, NoteSwitchingList.Null));
        toggle_BPM.onValueChanged.AddListener((value) => OnToggleValueChanged(value, noteSettings_BPM, NoteSwitchingList.BPM));
        toggle_Remove.onValueChanged.AddListener((value) => OnToggleValueChanged(value, noteSettings_Empty, NoteSwitchingList.Remove));
        toggle_Tap1.onValueChanged.AddListener((value) => OnToggleValueChanged(value, noteSettings_Empty, NoteSwitchingList.Tap1));
        toggle_Tap2.onValueChanged.AddListener((value) => OnToggleValueChanged(value, noteSettings_Empty, NoteSwitchingList.Tap2));
        toggle_Hold.onValueChanged.AddListener((value) => OnToggleValueChanged(value, noteSettings_Empty, NoteSwitchingList.Hold));
        toggle_Slide.onValueChanged.AddListener((value) => OnToggleValueChanged(value, noteSettings_Slide, NoteSwitchingList.Slide));
        toggle_Scratch.onValueChanged.AddListener((value) => OnToggleValueChanged(value, noteSettings_Empty, NoteSwitchingList.Scratch));
        toggle_Stop.onValueChanged.AddListener((value) => OnToggleValueChanged(value, noteSettings_Empty, NoteSwitchingList.Stop));
    }
    void OnToggleValueChanged(bool isOn, GameObject panel, NoteSwitchingList note)
    {
        panel.SetActive(isOn);
        SwitchNotes(note);

        selectedNoteType = note;

        isWaitForPlacement = false; // 放弃未放置的尾长条
        isASecondClick = false;     // 放弃Slider的第二次点击判断
    }

    // 音符切换列表
    public enum NoteSwitchingList
    {
        Null, BPM, Remove, Tap1, Tap2, Hold, Slide, Scratch, Stop
    }

    // 当前选择的音符类型
    public NoteSwitchingList selectedNoteType = NoteSwitchingList.Null;

    // 切换编辑谱面操作为选择的类型
    public void SwitchNotes(NoteSwitchingList note)
    {
        switch (note)
        {
            case NoteSwitchingList.Null:
                break;
            case NoteSwitchingList.BPM:
                break;
            case NoteSwitchingList.Remove:
                break;
            case NoteSwitchingList.Tap1:
                break;
            case NoteSwitchingList.Tap2:
                break;
            case NoteSwitchingList.Hold:
                break;
            case NoteSwitchingList.Slide:
                break;
            case NoteSwitchingList.Scratch:
                break;
            case NoteSwitchingList.Stop:
                break;
            default:
                break;
        }
    }
    #endregion

    #region 选择音符的可修改内容
    [HideInInspector] public float bpmValue = 120f;  // BPM值
    [HideInInspector] public Note_EffectType mixerEffect;  // 混音器效果
    [HideInInspector] public float mixingIntensity; // 混音强度
    [Header("混音强度值")] public TextMeshProUGUI mixingIntensity_Value;

    float waitingForSettings_BPMValue;
    public void _OnEndEdit_BpmValue(string input)
    {
        waitingForSettings_BPMValue = float.Parse(input);
        if (waitingForSettings_BPMValue <= 0) { Create_PopUPPanel(1); return; }
        if (200 < waitingForSettings_BPMValue) { Create_PopUPPanel(2); return; }

        if (waitingForSettings_BPMValue != 0)
            bpmValue = waitingForSettings_BPMValue;
    }
    // 超过200BPM的弹窗确认后依然设置
    void ConfirmSettings_BPMValue()
    {
        bpmValue = waitingForSettings_BPMValue;
    }

    public void _OnValueChanged_MixereffectIndex(int input)
    {
        switch (input)
        {
            case 0:
                mixerEffect = Note_EffectType.Null;
                break;
            case 1:
                mixerEffect = Note_EffectType.MultbandEQFreq;
                break;
            case 2:
                mixerEffect = Note_EffectType.ThreeEQLow;
                break;
            case 3:
                mixerEffect = Note_EffectType.ThreeEQMid;
                break;
            case 4:
                mixerEffect = Note_EffectType.ThreeEQHigh;
                break;
            case 5:
                mixerEffect = Note_EffectType.Chorus;
                break;
            case 6:
                mixerEffect = Note_EffectType.FlangerMix;
                break;
            case 7:
                mixerEffect = Note_EffectType.Gain;
                break;
            case 8:
                mixerEffect = Note_EffectType.TremoloFrequency;
                break;
            case 9:
                mixerEffect = Note_EffectType.Delay;
                break;
            case 10:
                mixerEffect = Note_EffectType.ReverbTime;
                break;
        }
    }
    public void _OnValueChanged_MixingIntensity(float input)
    {
        mixingIntensity_Value.text = ((int)(input * 100)).ToString() + "%";
        mixingIntensity = input;
    }
    #endregion

    #region 左下角的全局控制
    [Header("左下角的全局控制内容")]
    // 音乐进度条
    public TextMeshProUGUI playTime; public Slider playTime_Slider;
    int playTime_Now;
    int content_NowPlayTime;        // 对于滚动视图：计算当前位置，设置播放时机
    void Update_MusicBar()
    {
        SoundManager.Instance.playerUploadInstance.getTimelinePosition(out playTime_Now);

        if (!SoundManager.Instance.playerUploadInstance_isPaused)
        {
            playTime.text = FormatTime(playTime_Now) + " / " + FormatTime(SoundManager.Instance.playerUploadAudioLength);

            if (playTime_Now != 0)  // 除零会导致Slider显示异常
                playTime_Slider.value = playTime_Now / (float)SoundManager.Instance.playerUploadAudioLength;
            else playTime_Slider.value = 0;
        }

        // 超出总长度，暂停音乐
        if (SoundManager.Instance.playerUploadAudioLength < playTime_Now)
            SoundManager.Instance.Pause_PlayerUpload_Music();

    }
    string FormatTime(int time)
    {
        // 将毫秒转换为分钟、秒和毫秒
        int minutes = time / (60 * 1000);
        int seconds = (time % (60 * 1000)) / 1000;
        int remainingMilliseconds = time % 1000;

        return string.Format("{0}:{1:00}.{2:00}", minutes, seconds, remainingMilliseconds / 10);
    }

    int FinalDragTime = 0;
    public void _OnvalueChanged_PlayTime_Slider(float value)
    {
        if (SoundManager.Instance.playerUploadInstance_isPaused)
        {
            // 在Slide使用FMOD的setTimelinePosition会爆内存
            // SoundManager.Instance.playerUploadInstance.setTimelinePosition((int)(SoundManager.Instance.playerUploadAudioLength * value));
            FinalDragTime = (int)(SoundManager.Instance.playerUploadAudioLength * value);
            playTime.text = FormatTime(FinalDragTime) + " / " + FormatTime(SoundManager.Instance.playerUploadAudioLength);
        }
    }
    public void _Play_UploadAudio()
    {
        if (valueReadCompleted)
        {
            SoundManager.Instance.playerUploadInstance.setTimelinePosition(FinalDragTime);

            playTime_Now_Float = playTime_Now / 1000f;
            PlayNoteEffects_NumericalSettings();

            SoundManager.Instance.Play_PlayerUpload_Music();
        }
    }
    public void _Play_UploadAudio_WithTrackPosition()
    {
        if (valueReadCompleted)
        {
            content_NowPlayTime = (int)(-content_RectTransform.anchoredPosition.y / (content_RectTransform.sizeDelta.y - 1080) * SoundManager.Instance.playerUploadAudioLength);

            SoundManager.Instance.playerUploadInstance.setTimelinePosition(content_NowPlayTime);
            if (0 < content_NowPlayTime)
                playTime_Slider.value = content_NowPlayTime / (float)SoundManager.Instance.playerUploadAudioLength;

            playTime_Now_Float = content_NowPlayTime / 1000f;
            PlayNoteEffects_NumericalSettings();

            SoundManager.Instance.Play_PlayerUpload_Music();
        }
    }
    public void _Pause_UploadAudio()
    {
        if (valueReadCompleted)
        {
            SoundManager.Instance.Pause_PlayerUpload_Music();
            SoundManager.Instance.ResetAll_MixingEffects();
        }
    }


    // 播放速度
    public TextMeshProUGUI playSpeed;
    public void _OnvalueChanged_PlaySpeed_Slider(float value)
    {
        switch (value)
        {
            case 0:
                playSpeed.text = "0.25x"; SoundManager.Instance.Set_PlayerUpload_Pitch(0.25f);
                break;
            case 1:
                playSpeed.text = "0.5x"; SoundManager.Instance.Set_PlayerUpload_Pitch(0.5f);
                break;
            case 2:
                playSpeed.text = "0.75x"; SoundManager.Instance.Set_PlayerUpload_Pitch(0.75f);
                break;
            case 3:
                playSpeed.text = "1.0x"; SoundManager.Instance.Set_PlayerUpload_Pitch(1f);
                break;
            case 4:
                playSpeed.text = "1.25x"; SoundManager.Instance.Set_PlayerUpload_Pitch(1.25f);
                break;
            case 5:
                playSpeed.text = "1.5x"; SoundManager.Instance.Set_PlayerUpload_Pitch(1.5f);
                break;
            case 6:
                playSpeed.text = "1.75x"; SoundManager.Instance.Set_PlayerUpload_Pitch(1.75f);
                break;
            case 7:
                playSpeed.text = "2.0x"; SoundManager.Instance.Set_PlayerUpload_Pitch(2f);
                break;
        }
    }


    // 网格细分
    [HideInInspector] public int gridSubdivision = 0;
    public void _OnValueChanged_GridSubdivision(int value)
    {
        switch (value)
        {
            case 0: // 1/1     画几条线
                gridSubdivision = 0; break;
            case 1: // 1/2
                gridSubdivision = 1; break;
            case 2: // 1/3
                gridSubdivision = 2; break;
            case 3: // 1/4
                gridSubdivision = 3; break;
            case 4: // 1/6
                gridSubdivision = 5; break;
            case 5: // 1/8
                gridSubdivision = 7; break;
            case 6: // 1/12
                gridSubdivision = 11; break;
            case 7: // 1/16
                gridSubdivision = 15; break;
        }
        Recalculate_Track_Position();
    }


    // 轨道缩放
    [HideInInspector] public float trackScaling = 1f;
    [HideInInspector] public float trackScaling_Last = 1f;  // 上次的缩放值
    public TextMeshProUGUI trackScaling_Text;
    public void _Plus_TrackScaling()
    {
        if (trackScaling < 2)
        {
            trackScaling_Last = trackScaling;
            trackScaling += 0.1f;
            trackScaling_Text.text = (trackScaling * 100).ToString("F0") + "%";
            Recalculate_Track_Position();
            RedrawNotes();
        }
    }
    public void _Minus_TrackScaling()
    {
        if (0.1 < trackScaling)
        {
            trackScaling_Last = trackScaling;
            trackScaling -= 0.1f;
            trackScaling_Text.text = (trackScaling * 100).ToString("F0") + "%";
            Recalculate_Track_Position();
            RedrawNotes();
        }
    }


    // 轨道横线单位
    public enum TrackHorizontalUnit { Time, BPM, }
    public TrackHorizontalUnit selected_TrackHorizontalUnit = TrackHorizontalUnit.Time;
    public void _Switching_TrackHorizontalUnit()
    {
        if (selected_TrackHorizontalUnit == TrackHorizontalUnit.Time)
        {
            selected_TrackHorizontalUnit = TrackHorizontalUnit.BPM;
            NewNotification("修改轨道横线单位为：BPM");
        }
        else
        {
            selected_TrackHorizontalUnit = TrackHorizontalUnit.Time;
            NewNotification("修改轨道横线单位为：时间");
        }

        Recalculate_Track_Position();   // 重新计算位置
    }


    // 背景切换
    bool backGround = true;
    public void _SwitchBackground()
    {
        if (backGround)
        {
            BackGroundMannger.Instance.SwitchTo_Black();
            NewNotification("修改背景为：纯黑");
            backGround = false;
        }
        else
        {
            BackGroundMannger.Instance.SwitchTo_Edit();
            NewNotification("修改背景为：创意");
            backGround = true;
        }
    }
    #endregion

    #region 播放音乐=>实时播放音效
    // 声明一个根据time排序后的chart类
    public Chart chart_PlaySE = new Chart();

    // 深拷贝代码，否则复制的chart_PlaySE与chart都引用同一地址，修改chart_PlaySE也会影响chart
    Chart DeepCopy()
    {
        // 使用LitJson进行序列化和反序列化来实现深拷贝
        string value = JsonMapper.ToJson(chart);
        return JsonMapper.ToObject<Chart>(value);
    }
    // 播放前的一些数值设定
    void PlayNoteEffects_NumericalSettings()
    {
        chart_PlaySE = DeepCopy();
        // 将所有音符按照时间从小到大排序
        chart_PlaySE.noteDataList = chart_PlaySE.noteDataList.OrderBy(note => note.time).ToList();

        for (int i = 0; i < chart_PlaySE.noteDataList.Count; i++)
        {
            // 找到第一个比当前音乐播放时间大的音符的索引
            if (playTime_Now_Float <= chart_PlaySE.noteDataList[i].time)
            {
                currentNoteIndex = i;
                return;
            }
        }
    }

    int currentNoteIndex;   // 当前音符索引
    float playTime_Now_Float;
    void PlayNoteEffects()
    {
        if (SoundManager.Instance.playerUploadInstance_isPaused == true) return;

        playTime_Now_Float = playTime_Now / 1000f;

        // 后续没有音符了
        if (currentNoteIndex >= chart_PlaySE.noteDataList.Count) return;

        // 如果当前播放时间达到了谱面音符的时间，那么就播放音效
        // 循环播放同一时间的音符音效，有约0.07秒延迟减去
        bool simultaneousNtes = true;
        if (chart_PlaySE.noteDataList[currentNoteIndex].time - 0.07f <= playTime_Now_Float)
        {
            while (simultaneousNtes == true)
            {
                switch (chart_PlaySE.noteDataList[currentNoteIndex].type)
                {
                    case NoteType.Tap1:
                        SoundManager.Instance.Play_SE("event:/SE/InGame/KIT_Tap1_Perfect");
                        break;
                    case NoteType.Tap2:
                        SoundManager.Instance.Play_SE("event:/SE/InGame/KIT_Tap2_Perfect");
                        break;
                    case NoteType.Long_Start:
                        SoundManager.Instance.Play_SE("event:/SE/InGame/KIT_Tap2_Perfect");
                        break;
                    case NoteType.Long_End:
                        SoundManager.Instance.Play_SE("event:/SE/InGame/KIT_Tap2_Perfect");
                        break;
                    case NoteType.Slide:
                        // 首先根据滑键长度设置播放音效
                        if (chart_PlaySE.noteDataList[currentNoteIndex].direction == 0)
                            SoundManager.Instance.Play_SE("event:/SE/InGame/KIT_Tap2_Perfect");
                        else
                            SoundManager.Instance.Play_SE("event:/SE/InGame/SliderFlick_Perfect");
                        // 然后设置音乐混音
                        if (chart_PlaySE.noteDataList[currentNoteIndex].nextId != 0)
                            SoundManager.Instance.Mixing_PlayerUpload_Music(chart_PlaySE.noteDataList[currentNoteIndex].effectType, chart_PlaySE.noteDataList[currentNoteIndex].effectParameter);
                        else
                            SoundManager.Instance.ResetAll_MixingEffects();
                        break;
                    case NoteType.Scratch_Left:
                        SoundManager.Instance.Play_SE("event:/SE/InGame/Scratch_Perfect");
                        break;
                    case NoteType.Scratch_Right:
                        SoundManager.Instance.Play_SE("event:/SE/InGame/Scratch_Perfect");
                        break;
                    case NoteType.Stop_Start:
                        SoundManager.Instance.Play_SE("event:/SE/InGame/Scratch_Perfect");
                        break;
                    case NoteType.Stop_End:
                        SoundManager.Instance.Play_SE("event:/SE/InGame/Scratch_Perfect");
                        break;
                }

                currentNoteIndex++;
                simultaneousNtes = false;

                if (currentNoteIndex < chart_PlaySE.noteDataList.Count)
                {
                    if (chart_PlaySE.noteDataList[currentNoteIndex - 1].time == chart_PlaySE.noteDataList[currentNoteIndex].time)
                    {
                        simultaneousNtes = true;
                    }
                }
            }
        }

    }
    #endregion

    #region 轨道画线
    [Header("轨道画线")]
    public RectTransform content_RectTransform;
    public GameObject horizontalLine_Father_OBJ;        // 父对象
    public GameObject horizontalLine_integer_OBJ;       // 整数
    public GameObject horizontalLine_subdivision_OBJ;   // 细分
    public GameObject horizontalLine_BPM_OBJ;           // BPM

    // 实例对象，重新计算位置需要先清除上一次的所有内容
    List<GameObject> horizontalLine_integer_Instance = new List<GameObject>();
    List<GameObject> horizontalLine_subdivision_Instance = new List<GameObject>();
    List<GameObject> horizontalLine_BPM_Instance = new List<GameObject>();
    public void Recalculate_Track_Position()   // 重新计算轨道位置
    {
        // 清除上一次的所有内容
        foreach (GameObject item in horizontalLine_integer_Instance)
            Destroy(item);
        foreach (GameObject item in horizontalLine_subdivision_Instance)
            Destroy(item);
        foreach (GameObject item in horizontalLine_BPM_Instance)
            Destroy(item);

        horizontalLine_integer_Instance.Clear();
        horizontalLine_subdivision_Instance.Clear();
        horizontalLine_BPM_Instance.Clear();

        // 轨道高度。加1080是为了末尾可以滚动下去
        content_RectTransform.sizeDelta = new Vector2(content_RectTransform.sizeDelta.x,
                                                      SoundManager.Instance.playerUploadAudioLength * trackScaling + 1080);

        // 两种轨道横线单位
        if (selected_TrackHorizontalUnit == TrackHorizontalUnit.Time)
            HorizontalLines_Time();
        else
            HorizontalLines_BPM();
    }
    void HorizontalLines_Time()
    {
        for (int i = 0; i < SoundManager.Instance.playerUploadAudioLength; i += 1000)
        {
            // 整数横线
            // 创建对象并得到RectTransform组件
            RectTransform line = Instantiate(horizontalLine_integer_OBJ, horizontalLine_Father_OBJ.transform).GetComponent<RectTransform>();
            // 设置位置。标准为1000像素为1s
            line.anchoredPosition = new Vector2(line.anchoredPosition.x, i * trackScaling);
            // 找到其子对象的Text并设置显示内容
            line.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = FormatTime(i);
            // 添加进管理
            horizontalLine_integer_Instance.Add(line.gameObject);

            // 细分横线
            float startPosition = i * trackScaling;
            float endPosition = (i * trackScaling) + (1000 * trackScaling);
            float oneValue = (endPosition - startPosition) / (gridSubdivision + 1);
            float nowPosition = startPosition;

            for (int j = 0; j < gridSubdivision; j++)
            {
                RectTransform subdivision_line = Instantiate(horizontalLine_subdivision_OBJ, horizontalLine_Father_OBJ.transform).GetComponent<RectTransform>();
                subdivision_line.anchoredPosition = new Vector2(subdivision_line.anchoredPosition.x, nowPosition + oneValue);
                horizontalLine_subdivision_Instance.Add(subdivision_line.gameObject);

                nowPosition += oneValue;
            }
        }
    }
    void HorizontalLines_BPM()
    {
        // 清空小节线
        chart.barLineList.Clear();

        float nowPosition = 0;      // 当前时间
        float nextPosition;         // 下一个(结束的)时间

        float aBeatOfTime;          // 每一拍的时间
        float aBigBeatOfTime;       // 一个大拍子的时间(1/4)
        float aSmallBeatOfTime;     // 一个小拍子的时间(1/4的1/4、1/6...)

        int currentBeatCount = 0;   // 当前大节拍数
        int currentSmallBeatCount = 1;  // 当前小节拍数

        // 遍历编辑谱面界面存储的信息
        for (int i = 0; i < chart.editInformation.Count; i++)
        {
            // 创建并设置BPM线
            RectTransform bpm_Line = Instantiate(horizontalLine_BPM_OBJ, horizontalLine_Father_OBJ.transform).GetComponent<RectTransform>();
            bpm_Line.anchoredPosition = new Vector2(bpm_Line.anchoredPosition.x, chart.editInformation[i].time * 1000 * trackScaling);
            bpm_Line.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "BPM " + chart.editInformation[i].bpm;
            horizontalLine_BPM_Instance.Add(bpm_Line.gameObject);

            // 倒数第二个元素
            if (i == chart.editInformation.Count - 1)
                nextPosition = SoundManager.Instance.playerUploadAudioLength * trackScaling;
            else
                nextPosition = chart.editInformation[i + 1].time * 1000 * trackScaling;

            // 计算拍子的时间(毫秒)
            aBeatOfTime = (60000f / chart.editInformation[i].bpm) * trackScaling;
            aBigBeatOfTime = aBeatOfTime;
            aSmallBeatOfTime = aBigBeatOfTime / (gridSubdivision + 1);

            // 大的横线
            for (float j = nowPosition; j < nextPosition; j += aBigBeatOfTime)
            {
                nowPosition = j;    // 设置当前位置

                // 优先先画小的横线
                float small_NowPosition = nowPosition;
                for (float l = 0; l < gridSubdivision; l++)
                {
                    small_NowPosition += aSmallBeatOfTime;

                    RectTransform subdivision_line = Instantiate(horizontalLine_subdivision_OBJ, horizontalLine_Father_OBJ.transform).GetComponent<RectTransform>();
                    subdivision_line.anchoredPosition = new Vector2(subdivision_line.anchoredPosition.x, small_NowPosition);
                    horizontalLine_subdivision_Instance.Add(subdivision_line.gameObject);

                    // 检查下次画了后会不会超到下一个BPM的位置，超出则跳过绘制
                    if (nextPosition < (small_NowPosition + aSmallBeatOfTime))
                        break;
                }

                RectTransform line = Instantiate(horizontalLine_integer_OBJ, horizontalLine_Father_OBJ.transform).GetComponent<RectTransform>();
                line.anchoredPosition = new Vector2(line.anchoredPosition.x, j);
                line.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"beat {currentBeatCount} - {currentSmallBeatCount}";
                horizontalLine_integer_Instance.Add(line.gameObject);

                // beat数+一小拍，4小拍后+1大拍
                currentSmallBeatCount++;
                if (4 < currentSmallBeatCount)
                {
                    // 增加小节线
                    chart.barLineList.Add(j / trackScaling / 1000);
                    // 4小拍后+1
                    currentSmallBeatCount = 1;
                    currentBeatCount++;
                }

            }
            // 画线部分循环完毕，更新位置
            nowPosition = nextPosition;
        }
    }

    void TrackPlaybackPosition()    // (音乐播放时)根据播放调整Content的位置
    {
        if (SoundManager.Instance.playerUploadInstance_isPaused == false)
            content_RectTransform.anchoredPosition = new Vector2(content_RectTransform.anchoredPosition.x,
                                                                -(playTime_Now * trackScaling));
    }
    #endregion

    #region 放置音符
    [Header("放置的音符预设体")]
    public GameObject[] placingNotes_OBJ = new GameObject[6];   // 0 = Tap1 ... 5 = Stop
    public GameObject connectionLine_OBJ;           // 连接线
    public GameObject placingNotes_Father;          // 放置音符父物体
    List<RectTransform> placingNotes_Instance = new List<RectTransform>();      // 管理用的音符实例对象

    public Sprite image_Slide_Arrow;                // Slide被切换为滑键的图片

    [HideInInspector] public bool noteTrack_Clicked;// 是否被点击
    [HideInInspector] public bool noteTime_Clicked;
    [HideInInspector] public int noteTrack;         // 轨道
    [HideInInspector] public float noteTime;        // 时间

    float placeNotes_noteTime;              // 放置音符时的y坐标
    float placeNotes_noteTrack;

    bool isWaitForPlacement;                // 是否正在等待放置长条类音符的下一个音符
    Event placedNotes;                      // 已放置的音符信息
    bool isASecondClick;                    // (slide)判断在已有音符的位置上，是否第二次点击，第二次点击将设置为方向滑键
    bool isContinuous;                      // (slide)判断本次尾节点是续的还是新建的
    RectTransform continuousRectTransform;  // (slide)续节点的显示音符
    Event continuousEvent;                  // (slide)续节点的chart内容
    int continuousIndex;                    // (slide)续节点的原节点的索引值

    /// <summary>
    /// 根据选择放置音符
    /// </summary>
    void PlacingNotes()
    {
        // 重置点击状态
        noteTrack_Clicked = false; noteTime_Clicked = false;

        placeNotes_noteTime = noteTime;
        noteTime = noteTime / trackScaling / 1000;
        placeNotes_noteTrack = 57.3f + 117.9f * noteTrack;

        RectTransform newPlaceNotes = null;

        switch (selectedNoteType)
        {
            case NoteSwitchingList.Null:
                return;
            #region BPM
            case NoteSwitchingList.BPM:
                // 先看看能不能在当前点击位置找到已设置的BPM
                for (int i = 0; i < chart.editInformation.Count; i++)
                {
                    if (Mathf.Abs(chart.editInformation[i].time - noteTime) < 0.001f)
                    {
                        // 找到了就删除，画线会重新画
                        chart.editInformation.RemoveAt(i);
                        Recalculate_Track_Position();
                        return;
                    }
                }

                // 没有找到，那么就在当前位置新添加BPM值，轨道横线单位必须先设置为BPM
                if (selected_TrackHorizontalUnit != TrackHorizontalUnit.BPM)
                {
                    NewNotification("横线单位必须先设置为BPM才可增加新值");
                    return;
                }
                if (bpmValue == 0) { bpmValue = 120; }
                chart.editInformation.Add(new EditInformation(noteTime, bpmValue));
                Recalculate_Track_Position();
                return;
            #endregion
            #region Remove
            case NoteSwitchingList.Remove:
                // 遍历所有音符，寻找满足条件的音符
                for (int i = 0; i < chart.noteDataList.Count; i++)
                {
                    // 小于0.001的误差可被视为同一音符
                    if (chart.noteDataList[i].laneId == noteTrack &&
                        Mathf.Abs(chart.noteDataList[i].time - noteTime) < 0.001f)
                    {
                        // 根据删除的音符具体实现逻辑
                        switch (chart.noteDataList[i].type)
                        {
                            case NoteType.Long_Start:
                                // 先删除尾节点，再移除自身
                                chart.noteDataList.RemoveAt(chart.noteDataList[i].nextId);
                                chart.noteDataList.RemoveAt(i);
                                MoveLongBarIndex(i, 2);
                                RedrawNotes();
                                return;
                            case NoteType.Long_End:
                                // 先删除起始点，再移除自身
                                chart.noteDataList.RemoveAt(chart.noteDataList[i].lastId);
                                i--;    // 移除了一个元素，总元素会少1
                                chart.noteDataList.RemoveAt(i);
                                MoveLongBarIndex(i, 2);
                                RedrawNotes();
                                return;
                            case NoteType.Slide:
                                #region Remove_Slide
                                // 首先判断还有没有下一个音符（是否为末尾连接音符）
                                if (chart.noteDataList[i].nextId == 0)
                                {
                                    // 没有下一个音符了，查找上一个音符 是否还有 连接的音符
                                    if (CheckIf0HasANote(chart.noteDataList[i].lastId) && chart.noteDataList[chart.noteDataList[i].lastId].lastId == 0)
                                    {
                                        // 上一个音符已经是第一个了，移除上一个音符和当前音符
                                        chart.noteDataList.RemoveAt(chart.noteDataList[i].lastId);
                                        i--;    // 移除了一个元素，总元素会少1
                                        chart.noteDataList.RemoveAt(i);
                                        MoveLongBarIndex(i, 2);
                                        RedrawNotes();
                                        return;
                                    }
                                    else
                                    {
                                        // 还有上一个音符，则将上一个音符的nextId设置为0并移除自身连接线，移除当前音符
                                        chart.noteDataList[chart.noteDataList[i].lastId].nextId = 0;
                                        chart.noteDataList.RemoveAt(i);
                                        MoveLongBarIndex(i, 1);
                                        RedrawNotes();
                                        return;
                                    }
                                }
                                // 这是个起点音符
                                else if (CheckIf0HasANote(i) == true & chart.noteDataList[i].lastId == 0 & chart.noteDataList[i].nextId != 0)
                                {
                                    // 查找下一个音符 是否是末尾音符
                                    if (chart.noteDataList[chart.noteDataList[i].nextId].nextId == 0)
                                    {
                                        // 是末尾音符，一起删除
                                        chart.noteDataList.RemoveAt(chart.noteDataList[i].nextId);
                                        chart.noteDataList.RemoveAt(i);
                                        MoveLongBarIndex(i, 2);
                                        RedrawNotes();
                                        return;
                                    }
                                    else
                                    {
                                        // 下一个音符是个中间音符，设置下一个音符lastId为0，移除自身
                                        chart.noteDataList[chart.noteDataList[i].nextId].lastId = 0;
                                        chart.noteDataList.RemoveAt(i);
                                        MoveLongBarIndex(i, 1);
                                        RedrawNotes();
                                        return;
                                    }
                                }
                                // 很难受，这是个中间音符
                                else
                                {
                                    // 直接罢工
                                    NewNotification("无法从中间节点删除音符");
                                    return;
                                }
                            #endregion
                            case NoteType.Stop_Start:
                                // 先删除尾节点，再移除自身
                                chart.noteDataList.RemoveAt(chart.noteDataList[i].nextId);
                                chart.noteDataList.RemoveAt(i);
                                MoveLongBarIndex(i, 2);
                                RedrawNotes();
                                return;
                            case NoteType.Stop_End:
                                // 先删除起始点，再移除自身
                                chart.noteDataList.RemoveAt(chart.noteDataList[i].lastId);
                                i--;    // 移除了一个元素，总元素会少1
                                chart.noteDataList.RemoveAt(i);
                                MoveLongBarIndex(i, 2);
                                RedrawNotes();
                                return;
                            default:
                                // Tap1、Tap2、Scratch_Left、Scratch_Right都是直接移除自身
                                chart.noteDataList.RemoveAt(i);
                                MoveLongBarIndex(i, 1);
                                RedrawNotes();
                                return;
                        }
                    }
                }
                break;
            #endregion
            #region Tap1
            case NoteSwitchingList.Tap1:
                if (NoteOverlapDetection() == false) return;
                if (0 < noteTrack && noteTrack < 6)
                {
                    chart.noteDataList.Add(new Event(NoteType.Tap1, noteTrack, noteTime, 0, 0, 0, Note_EffectType.Null, 0));
                    newPlaceNotes = Instantiate(placingNotes_OBJ[0], placingNotes_Father.transform).GetComponent<RectTransform>();
                }
                else
                    NewNotification("Tap1音符必须放置在中间5轨道上");
                break;
            #endregion
            #region Tap2
            case NoteSwitchingList.Tap2:
                if (NoteOverlapDetection() == false) return;
                if (0 < noteTrack && noteTrack < 6)
                {
                    chart.noteDataList.Add(new Event(NoteType.Tap2, noteTrack, noteTime, 0, 0, 0, Note_EffectType.Null, 0));
                    newPlaceNotes = Instantiate(placingNotes_OBJ[1], placingNotes_Father.transform).GetComponent<RectTransform>();
                }
                else
                    NewNotification("Tap2音符必须放置在中间5轨道上");
                break;
            #endregion
            #region Hold
            case NoteSwitchingList.Hold:
                if (NoteOverlapDetection() == false) return;
                if (0 < noteTrack && noteTrack < 6)
                {
                    if (isWaitForPlacement == false)
                    {
                        placedNotes = new Event(NoteType.Long_Start, noteTrack, noteTime, 0, 0, 0, Note_EffectType.Null, 0);
                        isWaitForPlacement = true;
                    }
                    else
                    {
                        if (LongNotePlacementDetection() == false) return;

                        placedNotes.nextId = chart.noteDataList.Count + 1;
                        chart.noteDataList.Add(placedNotes);

                        chart.noteDataList.Add(new Event(NoteType.Long_End, noteTrack, noteTime, 0, chart.noteDataList.Count - 1, 0, Note_EffectType.Null, 0));

                        newPlaceNotes = Instantiate(placingNotes_OBJ[2], placingNotes_Father.transform).GetComponent<RectTransform>();
                        // 单独创建起始点的音符显示
                        RectTransform newPlaceNotes_start = Instantiate(placingNotes_OBJ[2], placingNotes_Father.transform).GetComponent<RectTransform>();
                        newPlaceNotes_start.anchoredPosition = new Vector2(57.3f + 117.9f * placedNotes.laneId, 1000 * trackScaling * placedNotes.time);
                        placingNotes_Instance.Add(newPlaceNotes_start);
                        // 绘制连接线
                        DrawConnectionLine(new Vector2(placeNotes_noteTrack, placeNotes_noteTime), newPlaceNotes_start.transform, new Color(1, 1, 0, 0.5f));

                        isWaitForPlacement = false;
                    }
                }
                else
                {
                    NewNotification("Hold音符必须放置在中间5轨道上");
                    ResetPlacementStatusOfLongNotes();
                }
                break;
            #endregion
            #region Slide
            case NoteSwitchingList.Slide:
                if (noteTrack <= 0 || 6 <= noteTrack)
                {
                    NewNotification("Slide音符必须放置在中间5轨道上");
                    ResetPlacementStatusOfLongNotes();
                    return;
                }
                if (NoteOverlapDetection_Slide() == 1)
                {
                    // 新的条
                    if (CheckTheCuttentTimeAlreadyExists_Slide() == false) return;

                    if (isWaitForPlacement == false)
                    {
                        placedNotes = new Event(NoteType.Slide, noteTrack, noteTime, 0, 0, 0, mixerEffect, mixingIntensity);
                        isWaitForPlacement = true;
                    }
                    else
                    {
                        if (LongNotePlacementDetection() == false) return;
                        // 判断是否是续的条的连接线
                        if (isContinuous == false)
                        {
                            placedNotes.nextId = chart.noteDataList.Count + 1;
                            chart.noteDataList.Add(placedNotes);

                            chart.noteDataList.Add(new Event(NoteType.Slide, noteTrack, noteTime, 0, chart.noteDataList.Count - 1, 0, mixerEffect, mixingIntensity));
                            newPlaceNotes = Instantiate(placingNotes_OBJ[3], placingNotes_Father.transform).GetComponent<RectTransform>();

                            newPlaceNotes.GetComponent<EditChart_HoldConnectionLine>().isSlide = true;

                            // 单独创建起始点的音符显示
                            RectTransform newPlaceNotes_start = Instantiate(placingNotes_OBJ[3], placingNotes_Father.transform).GetComponent<RectTransform>();
                            newPlaceNotes_start.anchoredPosition = new Vector2(57.3f + 117.9f * placedNotes.laneId, 1000 * trackScaling * placedNotes.time);
                            placingNotes_Instance.Add(newPlaceNotes_start);

                            newPlaceNotes_start.GetComponent<EditChart_HoldConnectionLine>().isSlide = true;

                            // 绘制连接线
                            DrawConnectionLine(new Vector2(placeNotes_noteTrack, placeNotes_noteTime), newPlaceNotes_start.transform, new Color(1, 0.75f, 0.8f, 0.5f), true);
                        }
                        else
                        {
                            continuousEvent.nextId = chart.noteDataList.Count;

                            // 可能放置音符后再继续绘制连接线，LastID不能为总数-1
                            chart.noteDataList.Add(new Event(NoteType.Slide, noteTrack, noteTime, 0, continuousIndex, 0, mixerEffect, mixingIntensity));
                            newPlaceNotes = Instantiate(placingNotes_OBJ[3], placingNotes_Father.transform).GetComponent<RectTransform>();

                            newPlaceNotes.GetComponent<EditChart_HoldConnectionLine>().isSlide = true;

                            DrawConnectionLine(new Vector2(placeNotes_noteTrack, placeNotes_noteTime), continuousRectTransform.transform, new Color(1, 0.75f, 0.8f, 0.5f), true);
                        }
                        ResetPlacementStatusOfLongNotes();
                    }
                }
                else if (NoteOverlapDetection_Slide() == 2)
                {
                    // 续的条
                    if (isASecondClick == false)
                    {
                        // 首次点击，则优先选择为继续 续粉条
                        for (int i = 0; i < chart.noteDataList.Count; i++)
                        {
                            if (chart.noteDataList[i].laneId == noteTrack &&
                                Mathf.Abs(chart.noteDataList[i].time - noteTime) < 0.001f)
                            {
                                if (chart.noteDataList[i].direction != 0)
                                {
                                    NewNotification("已是滑动Slide，不允许继续延长");
                                    ResetPlacementStatusOfLongNotes();
                                    return;
                                }
                                if (chart.noteDataList[i].nextId != 0)
                                {
                                    NewNotification("非单根最后节点不允许设置为滑动Slide类型");
                                    ResetPlacementStatusOfLongNotes();
                                    return;
                                }

                                continuousEvent = chart.noteDataList[i];
                                continuousIndex = i;
                                isWaitForPlacement = true;
                                isContinuous = true;
                                isASecondClick = true;

                                // 这句代码只是附加，只是为了过LongNotePlacementDetection()检测
                                placedNotes = new Event(NoteType.Slide, noteTrack, noteTime, 0, 0, 0, mixerEffect, mixingIntensity);

                                placeNotes_noteTrack = 57.3f + 117.9f * chart.noteDataList[i].laneId;
                                placeNotes_noteTime = chart.noteDataList[i].time * 1000 * trackScaling;
                                for (int j = 0; j < placingNotes_Instance.Count; j++)
                                {
                                    if (placingNotes_Instance[j].anchoredPosition.x == placeNotes_noteTrack &&
                                        Mathf.Abs(placingNotes_Instance[j].anchoredPosition.y - placeNotes_noteTime) < 0.001f)
                                        continuousRectTransform = placingNotes_Instance[j];
                                }
                            }
                        }
                    }
                    else
                    {
                        // 二次点击，意图设置为滑键
                        // 遍历查找当前的滑键对象
                        for (int i = 0; i < chart.noteDataList.Count; i++)
                        {
                            if (chart.noteDataList[i].laneId == noteTrack &&
                                Mathf.Abs(chart.noteDataList[i].time - noteTime) < 0.001f)
                            {
                                // 寻找其后的下一个slide音符，并设置滑键长度
                                for (int j = i + 1; j < chart.noteDataList.Count; j++)
                                {
                                    // 除了类型一致，必须时间也比现在的晚。玩家有可能现在前面放置，随后在更早的时间放置，索引会更高，但是时间却更小
                                    if (chart.noteDataList[j].type == NoteType.Slide &&
                                        chart.noteDataList[i].time < chart.noteDataList[j].time)
                                    {
                                        // 下一个Slide的轨道减去当前Slide的轨道
                                        int direction = chart.noteDataList[j].laneId - chart.noteDataList[i].laneId;

                                        if (direction == 0)
                                        {
                                            NewNotification("滑动Slide的下一节点不允许在同一轨道上");
                                            ResetPlacementStatusOfLongNotes();
                                            return;
                                        }

                                        chart.noteDataList[i].direction = direction;
                                        Vector2 vector2 = new Vector2(57.3f + 117.9f * chart.noteDataList[i].laneId, 1000 * chart.noteDataList[i].time * trackScaling);

                                        // 遍历寻找显示的图片的物体
                                        for (int l = 0; l < placingNotes_Instance.Count; l++)
                                        {
                                            if (placingNotes_Instance[l].anchoredPosition == vector2)
                                            {
                                                Image image = placingNotes_Instance[l].GetComponent<Image>();
                                                image.sprite = image_Slide_Arrow;
                                                // 设置原生大小
                                                image.SetNativeSize();
                                            }

                                        }
                                        ResetPlacementStatusOfLongNotes();
                                        return;
                                    }
                                }
                                NewNotification("请先放置下一个Slide节点以计算滑动长度");
                                ResetPlacementStatusOfLongNotes();
                            }
                        }
                    }
                }
                break;
            #endregion
            #region Scratch
            case NoteSwitchingList.Scratch:
                if (NoteOverlapDetection() == false) return;
                if (noteTrack == 0)
                {
                    chart.noteDataList.Add(new Event(NoteType.Scratch_Left, 0, noteTime, 0, 0, 0, Note_EffectType.Null, 0));
                    newPlaceNotes = Instantiate(placingNotes_OBJ[4], placingNotes_Father.transform).GetComponent<RectTransform>();
                }
                else if (noteTrack == 6)
                {
                    chart.noteDataList.Add(new Event(NoteType.Scratch_Right, 6, noteTime, 0, 0, 0, Note_EffectType.Null, 0));
                    newPlaceNotes = Instantiate(placingNotes_OBJ[4], placingNotes_Father.transform).GetComponent<RectTransform>();
                }
                else
                    NewNotification("Scratch音符必须放置在DJ轨道上");
                break;
            #endregion
            #region Stop
            case NoteSwitchingList.Stop:
                if (NoteOverlapDetection() == false) return;
                if (noteTrack == 0 || noteTrack == 6)
                {
                    if (isWaitForPlacement == false)
                    {
                        placedNotes = new Event(NoteType.Stop_Start, noteTrack, noteTime, 0, 0, 0, Note_EffectType.Null, 0);
                        isWaitForPlacement = true;
                    }
                    else
                    {
                        if (LongNotePlacementDetection() == false) return;
                        if (placedNotes.laneId != noteTrack)
                        {
                            NewNotification("Stop音符的起始点不在同一条轨道上");
                            ResetPlacementStatusOfLongNotes();
                            return;
                        }

                        placedNotes.nextId = chart.noteDataList.Count + 1;
                        chart.noteDataList.Add(placedNotes);

                        chart.noteDataList.Add(new Event(NoteType.Stop_End, noteTrack, noteTime, 0, chart.noteDataList.Count - 1, 0, Note_EffectType.Null, 0));

                        newPlaceNotes = Instantiate(placingNotes_OBJ[5], placingNotes_Father.transform).GetComponent<RectTransform>();
                        // 单独创建起始点的音符显示
                        RectTransform newPlaceNotes_start = Instantiate(placingNotes_OBJ[5], placingNotes_Father.transform).GetComponent<RectTransform>();
                        newPlaceNotes_start.anchoredPosition = new Vector2(57.3f + 117.9f * placedNotes.laneId, 1000 * trackScaling * placedNotes.time);
                        placingNotes_Instance.Add(newPlaceNotes_start);
                        // 绘制连接线
                        DrawConnectionLine(new Vector2(placeNotes_noteTrack, placeNotes_noteTime), newPlaceNotes_start.transform, new Color(1, 0, 0, 0.5f));

                        isWaitForPlacement = false;
                    }
                }
                else
                {
                    NewNotification("Stop音符必须放置在DJ轨道上");
                    ResetPlacementStatusOfLongNotes();
                }
                break;
                #endregion
        }

        if (newPlaceNotes != null)
        {
            newPlaceNotes.anchoredPosition = new Vector2(placeNotes_noteTrack, placeNotes_noteTime);
            placingNotes_Instance.Add(newPlaceNotes);
        }
    }
    /// <summary>
    /// 音符重叠检测
    /// </summary>
    bool NoteOverlapDetection()
    {
        for (int i = 0; i < chart.noteDataList.Count; i++)
        {
            if (chart.noteDataList[i].laneId == noteTrack && chart.noteDataList[i].time == noteTime)
            {
                NewNotification("音符放置重叠");
                ResetPlacementStatusOfLongNotes();
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 音符重叠检测(Slide用)
    /// </summary>
    int NoteOverlapDetection_Slide()
    {
        for (int i = 0; i < chart.noteDataList.Count; i++)
        {
            if (chart.noteDataList[i].laneId == noteTrack && chart.noteDataList[i].time == noteTime)
            {
                if (chart.noteDataList[i].type == NoteType.Slide)
                    return 2;   // 重叠的是自身
                else
                {
                    NewNotification("音符放置重叠");
                    ResetPlacementStatusOfLongNotes();
                    return 0;
                }

            }
        }
        return 1;   // 新建
    }
    /// <summary>
    /// 检测当前时间是否已存在Slide音符
    /// </summary>
    /// <returns></returns>
    bool CheckTheCuttentTimeAlreadyExists_Slide()
    {
        float time;
        for (int i = 0; i < chart.noteDataList.Count; i++)
        {
            if (chart.noteDataList[i].type == NoteType.Slide)
            {
                // 判断是否是同时的音符
                if (Mathf.Abs(chart.noteDataList[i].time - noteTime) < 0.001f)
                {
                    NewNotification("不允许在同一时间放置多个Slide音符");
                    ResetPlacementStatusOfLongNotes();
                    return false;
                }

                // 判断是否在Slide区间继续放置音符
                if (chart.noteDataList[i].nextId != 0)
                {
                    // 首先判断判断的音符时间是否超出了当前放置位置的时间，超出则不执行
                    if (noteTime < chart.noteDataList[i].time) continue;

                    time = chart.noteDataList[chart.noteDataList[i].nextId].time;
                    if ((time - noteTime) > 0.001f)
                    {
                        NewNotification("Slide音符之间不允许继续放置新音符");
                        ResetPlacementStatusOfLongNotes();
                        return false;
                    }
                }
            }
        }
        return true;
    }
    /// <summary>
    /// 长条音符放置位置检测（尾节点）
    /// </summary>
    /// <returns></returns>
    bool LongNotePlacementDetection()
    {
        if (noteTime < placedNotes.time)
        {
            NewNotification("尾节点不能放在比起始节点还早的位置上");
            ResetPlacementStatusOfLongNotes();
            return false;
        }
        else if (noteTime == placedNotes.time)
        {
            NewNotification("尾节点不能和起始节点放在同一时间");
            ResetPlacementStatusOfLongNotes();
            return false;
        }
        return true;
    }
    /// <summary>
    /// 判断0是索引还是真的就没有内容(Slide) 返回true代表真的没有内容，返回false代表有内容但索引是0
    /// </summary>
    bool CheckIf0HasANote(int index)
    {
        try
        {
            // 先判断索引0是不是有指定元素
            if (chart.noteDataList[0].type == NoteType.Slide)
            {
                // 再判断是否与当前的索引相匹配
                if (chart.noteDataList[0].nextId == index)
                    return false;
            }
        }
        catch { return true; }
        return true;
    }
    /// <summary>
    /// 重置长条类音符放置状态
    /// </summary>
    void ResetPlacementStatusOfLongNotes()
    {
        isWaitForPlacement = false;
        isASecondClick = false;
        isContinuous = false;
    }
    /// <summary>
    ///  移动后续长条索引
    /// </summary>
    /// <param name="startIndex"></param>
    void MoveLongBarIndex(int startIndex, int value)
    {
        for (int i = startIndex; i < chart.noteDataList.Count; i++)
        {
            if (chart.noteDataList[i].nextId != 0)
                chart.noteDataList[i].nextId -= value;
            if (chart.noteDataList[i].lastId != 0)
                chart.noteDataList[i].lastId -= value;
        }
    }

    Vector2 offsetPosition = new Vector2(89, 39.5f);   // 线起始点偏移位置
    /// <summary>
    /// 绘制连接线
    /// </summary>
    void DrawConnectionLine(Vector2 endPosition, Transform father_OBJ, Color color, bool isSlide = false)
    {
        // 主要是判断是不是Slide
        if (isSlide == false) { offsetPosition.x = 89; offsetPosition.y = 39.5f; }
        else { offsetPosition.x = 17.67f; offsetPosition.y = 104.22f; }

        // 创建连接线实例
        RectTransform lineRect = Instantiate(connectionLine_OBJ, transform).GetComponent<RectTransform>();
        Image lineRect_Image = lineRect.GetComponent<Image>();
        lineRect_Image.color = color;

        // 设置为子对象，删除起始点时可以一并删除，并且可以有依附脚本方便管理
        EditChart_HoldConnectionLine holdConnectionLine = father_OBJ.GetComponent<EditChart_HoldConnectionLine>();
        holdConnectionLine.connectionLine = lineRect_Image;
        holdConnectionLine.endPosition = endPosition;

        lineRect.transform.SetParent(father_OBJ);
        Vector2 lineRect_fatherPosition = father_OBJ.GetComponent<RectTransform>().anchoredPosition;

        // 设置连接线的起始位置
        lineRect.anchoredPosition = offsetPosition;

        // 计算连接线的方向（旋转角度）
        Vector2 direction = endPosition - lineRect_fatherPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90f;

        // 设置连接线的旋转角度
        lineRect.rotation = Quaternion.Euler(0f, 0f, angle);

        // 计算连接线的高度
        float distance = Vector2.Distance(lineRect_fatherPosition, endPosition);
        lineRect.sizeDelta = new Vector2(20f, distance);
    }
    /// <summary>
    /// 重新绘制音符
    /// </summary>
    public void RedrawNotes()
    {
        foreach (var item in placingNotes_Instance)
        {
            Destroy(item.gameObject);
        }
        placingNotes_Instance.Clear();

        RectTransform newPlaceNotes = null;
        for (int i = 0; i < chart.noteDataList.Count; i++)
        {
            switch (chart.noteDataList[i].type)
            {
                case NoteType.Tap1:
                    newPlaceNotes = Instantiate(placingNotes_OBJ[0], placingNotes_Father.transform).GetComponent<RectTransform>();
                    break;
                case NoteType.Tap2:
                    newPlaceNotes = Instantiate(placingNotes_OBJ[1], placingNotes_Father.transform).GetComponent<RectTransform>();
                    break;
                case NoteType.Long_Start:
                    newPlaceNotes = Instantiate(placingNotes_OBJ[2], placingNotes_Father.transform).GetComponent<RectTransform>();
                    break;
                case NoteType.Long_End:
                    newPlaceNotes = Instantiate(placingNotes_OBJ[2], placingNotes_Father.transform).GetComponent<RectTransform>();
                    break;
                case NoteType.Slide:
                    newPlaceNotes = Instantiate(placingNotes_OBJ[3], placingNotes_Father.transform).GetComponent<RectTransform>();
                    break;
                case NoteType.Scratch_Left:
                    newPlaceNotes = Instantiate(placingNotes_OBJ[4], placingNotes_Father.transform).GetComponent<RectTransform>();
                    break;
                case NoteType.Scratch_Right:
                    newPlaceNotes = Instantiate(placingNotes_OBJ[4], placingNotes_Father.transform).GetComponent<RectTransform>();
                    break;
                case NoteType.Stop_Start:
                    newPlaceNotes = Instantiate(placingNotes_OBJ[5], placingNotes_Father.transform).GetComponent<RectTransform>();
                    break;
                case NoteType.Stop_End:
                    newPlaceNotes = Instantiate(placingNotes_OBJ[5], placingNotes_Father.transform).GetComponent<RectTransform>();
                    break;
            }
            if (newPlaceNotes != null)
            {
                placeNotes_noteTime = chart.noteDataList[i].time * 1000 * trackScaling;
                placeNotes_noteTrack = 57.3f + 117.9f * chart.noteDataList[i].laneId;

                newPlaceNotes.anchoredPosition = new Vector2(placeNotes_noteTrack, placeNotes_noteTime);
                placingNotes_Instance.Add(newPlaceNotes);
            }
            // 傻方法，先根据不同音符创建，然后统一设置位置，最终需要绘制连接线的再绘制
            switch (chart.noteDataList[i].type)
            {
                case NoteType.Long_Start:
                    RedrawNotes_DrawConnectionLine(chart.noteDataList[i].nextId, new Color(1, 1, 0, 0.5f), newPlaceNotes);
                    break;
                case NoteType.Slide:
                    if (chart.noteDataList[i].nextId != 0)
                        RedrawNotes_DrawConnectionLine(chart.noteDataList[i].nextId, new Color(1, 0.75f, 0.8f, 0.5f), newPlaceNotes, true);
                    else if (chart.noteDataList[i].direction != 0)
                    {
                        Image image = newPlaceNotes.GetComponent<Image>();
                        image.sprite = image_Slide_Arrow;
                        image.SetNativeSize();
                    }
                    break;
                case NoteType.Stop_Start:
                    RedrawNotes_DrawConnectionLine(chart.noteDataList[i].nextId, new Color(1, 0, 0, 0.5f), newPlaceNotes);
                    break;
            }
        }
    }
    /// <summary>
    /// 重新绘制音符 连接线
    /// </summary>
    /// <param name="nextId">尾节点Event Index</param>
    /// <param name="color">颜色</param>
    /// <param name="fatherOBJ">父物体</param>
    /// <param name="isSlide">是否为Slide（可选）</param>
    void RedrawNotes_DrawConnectionLine(int nextId, Color color, Transform fatherOBJ, bool isSlide = false)
    {
        try
        {
            // 设置结束点
            Event endNote = chart.noteDataList[nextId];
            Vector2 endNotePosition = new Vector2();
            endNotePosition.x = 57.3f + 117.9f * endNote.laneId;
            endNotePosition.y = endNote.time * 1000 * trackScaling;

            DrawConnectionLine(endNotePosition, fatherOBJ, color, isSlide);
        }
        catch { }
    }
    #endregion

    #region 弹窗说明
    void Create_PopUPPanel(int messageIndex)
    {
        switch (messageIndex)
        {
            // 选择音符的可修改内容
            case 1:
                PanelMannger.Instance.Create_PopUP("确认", "BPM不能为负数或0" +
                                            "\n\n本次输入将废弃，请重新输入值",
                                            new Set_PopUP_Button("确认", () => { }));
                break;
            case 2:
                PanelMannger.Instance.Create_PopUP("确认？", "确认将BPM的值设置为200以上？\n" +
                    "过高的BPM值会使得网格细分更密集，带来的性能消耗也越高\n" +
                    "如确实有需求，请先缩小网格细分的值，以减少性能消耗。",
                    new Set_PopUP_Button("设置", () => { ConfirmSettings_BPMValue(); }),
                    new Set_PopUP_Button("取消", () => { }));
                break;
        }
    }
    #endregion

    #region 悬浮文字提示
    [Header("悬浮文字对象")]
    public Transform notificationFather;
    public GameObject notificationObj;
    public int maxNotificationPoolCapacity = 3; // 池子最大的容量
    List<TextMeshProUGUI> notificationPool = new List<TextMeshProUGUI>();   // Queue无法通过下标单独访问元素，故使用List
    public void NewNotification(string content)
    {
        SoundManager.Instance.Play_SE("event:/SE/GameStart/RadarChart_Change");

        // 清理列表中的空对象
        for (int i = notificationPool.Count - 1; i >= 0; i--)
        {
            if (notificationPool[i] == null)
                notificationPool.RemoveAt(i);
        }

        // 修改剩余的位置
        for (int i = 0; i < notificationPool.Count; i++)
        {
            notificationPool[i].rectTransform.anchoredPosition = new Vector2(notificationPool[i].rectTransform.anchoredPosition.x,
                notificationPool[i].rectTransform.anchoredPosition.y + 50);
        }


        TextMeshProUGUI notification = Instantiate(notificationObj, notificationFather).GetComponent<TextMeshProUGUI>();
        notification.text = content;
        notificationPool.Add(notification);

        if (maxNotificationPoolCapacity < notificationPool.Count)
        {
            // 最早添加的元素的索引永远是0
            Destroy(notificationPool[0].gameObject);
            notificationPool.RemoveAt(0);
        }
    }
    #endregion

    #region PC端快捷操作
    public void PCSideShortcutOperations()
    {
        // 右侧音符切换
        if (Input.GetKeyDown(KeyCode.Q))
            toggle_Null.isOn = true;
        if (Input.GetKeyDown(KeyCode.W))
            toggle_BPM.isOn = true;
        if (Input.GetKeyDown(KeyCode.E))
            toggle_Remove.isOn = true;
        if (Input.GetKeyDown(KeyCode.A))
            toggle_Tap1.isOn = true;
        if (Input.GetKeyDown(KeyCode.S))
            toggle_Tap2.isOn = true;
        if (Input.GetKeyDown(KeyCode.D))
            toggle_Hold.isOn = true;
        if (Input.GetKeyDown(KeyCode.Z))
            toggle_Slide.isOn = true;
        if (Input.GetKeyDown(KeyCode.X))
            toggle_Scratch.isOn = true;
        if (Input.GetKeyDown(KeyCode.C))
            toggle_Stop.isOn = true;

        // 音频播放
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SoundManager.Instance.playerUploadInstance_isPaused)
                _Play_UploadAudio_WithTrackPosition();
            else
                _Pause_UploadAudio();
        }

        // 轨道缩放
        if (Input.GetKey(KeyCode.LeftControl))
        {
            // 获取滚轮滚动值 -1为向后滚动 +1为向前滚动
            if (0 < Input.GetAxis("Mouse ScrollWheel"))
                _Plus_TrackScaling();
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                _Minus_TrackScaling();
        }
    }
    #endregion
}