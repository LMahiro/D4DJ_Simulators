using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{
    // ——————————————————————————————Live设置————————————————————————————————
    // ————————————特殊
    public bool displayTheContentBeingTested = false; // 显示正在测试的内容
    // ————————————演出设置
    // 基本设置
    public float scrollSpeed = 5.0f; // 谱面流速
    public float noteSize = 1.20f; // 音符大小
    public float musicLatencyAdjustment = 0.0f; // 音乐延迟调整
    public float trackLength = 1.00f; // 轨道长度
    public float trackOpacity = 1.00f; // 轨道不透明度
    // 轨道个性化
    public float trackWidth = 1.00f; // 轨道宽度
    public int judgementLineHeight = 0; // 判定线高度
    public float trackDividerOpacity = 0.80f; // 轨道分割线不透明度
    // 视觉辅助
    public float holdNoteConnectorOpacity = 1.00f; // 长条连接线不透明度
    public float reboundNoteOpacity = 0.70f; // 待反弹音符不透明度
    public float autoNoteOpacity = 0.50f; // AUTO音符不透明度
    // 判定设置
    public float judgementLatencyAdjustment = 0.0f; // 判定延迟调整
    public float djTrackSlideSensitivity = 0.50f; // DJ轨道滑动判定灵敏度
    public float judgementTextHeight = 0.0f; // 判定文字高度
    public bool centerDisplayOfJudgmentText = false; // 居中显示判定文字
    public JudgementMode judgementMode = JudgementMode.Blend; // 判定模式
    // MV设置
    public float mvOpacity = 0.80f; // MV不透明度
    // 显示设置
    public bool showBarLines = true; // 小节线
    public bool showSimultaneousLines = true; // 同时点击线
    public bool showSkillActivationLines = true; // 技能发动线
    public bool showSkillWindow = true; // 技能窗口
    // 演出设置
    public bool screenShake = true; // 画面摇晃
    public bool groovyPerformance = true; // GROOVY演出

    // ————————————音量设置
    // 音乐音量
    public float musicVolume = 1.00f; // 音乐音量
    public bool musicVolumeSliderEffect = true; // 音乐音量滑条效果
    // 音效音量
    public float soundEffectVolume = 1.00f; // 音效音量
    public SoundEffectSelection soundEffectSelection = SoundEffectSelection.Normal; //音效选择
    // 语音音量
    public float voiceVolume = 1.00f; // 语音音量

    // ————————————皮肤设置
    // 皮肤设置
    public SkinSelection skinSelection = SkinSelection.Normal; //皮肤选择

    // ————————————高级设置
    // 帧率设置
    public Framerate gameInterfaceFramerate = Framerate._120; // 游戏界面帧率设置
    // 画质设置
    public QualityLevel gameInterfaceQuality = QualityLevel.High; // 游戏界面画质设置
    // 特殊Combo数字
    public SpecialComboThreshold specialComboThreshold = SpecialComboThreshold.PerfectAndAbove; // 特殊Combo数字
    // FAST/SLOW显示
    public FastSlowDisplayThreshold fastSlowDisplayThreshold = FastSlowDisplayThreshold.PerfectBelow; // FAST/SLOW显示

    // ——————————————————————————————常规设置————————————————————————————————
    // ————————————常规设置
    // 常规界面帧率设置
    public Framerate generalInterfaceFramerate = Framerate._60; // 常规界面帧率设置
    // 常规界面画质设置
    public QualityLevel generalInterfaceQuality = QualityLevel.High; // 常规界面画质设置

    // ————————————音量设置
    // 常规界面音乐音量
    public float generalMusicVolume = 0.80f; // 常规音乐音量
    // 常规界面音效音量
    public float generalSoundEffectVolume = 0.50f; // 常规音效音量
    // 常规界面语音音量
    public float generalVoiceVolume = 0.70f; // 常规语音音量

    // ————————————显示设置
    // 点击特效
    public bool showTapEffects = true; // 点击特效

    // ————————————特殊
    // DebugMode模式
    public bool debugMode = false;
}


// 判定模式
public enum JudgementMode
{
    Blend,     // 混合
    Separate   // 分离
}

// 音效选择
public enum SoundEffectSelection
{
    Normal,     //常规
}

// 皮肤选择
public enum SkinSelection
{
    Normal,     //常规
}

// 帧率选择
public enum Framerate
{
    _30,
    _60,
    _120,
}

// 质量等级
public enum QualityLevel
{
    High,  // 高
    Medium,// 中
    Low    // 低
}

// 特殊Combo数字阈值
public enum SpecialComboThreshold
{
    JustPerfect,      // Just Perfect
    PerfectAndAbove,  // Perfect以上
    GreatAndAbove,    // Great以上
    GoodAndAbove,     // Good以上
    Disable           // 不开启此功能
}

// FAST/SLOW显示阈值
public enum FastSlowDisplayThreshold
{
    Above10ms,    // 10ms以上
    PerfectBelow, // Perfect以下
    GreatBelow,   // Great以下
    GoodBelow,    // Good以下
    Disable       // 不开启此功能
}