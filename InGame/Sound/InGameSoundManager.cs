using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

public class InGameSoundManager : MonoBehaviour
{
    private static InGameSoundManager instance;
    public static InGameSoundManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        dialogueCallback = new EVENT_CALLBACK(DialogueEventCallback);
    }


    // ————————————————————————————————音效——————————————————————————————————————
    void Play_SE(string path)
    {
        EventInstance instance = RuntimeManager.CreateInstance(path);
        instance.setVolume(GameSettingsMannger.save_Settings.soundEffectVolume);
        instance.start();
        instance.release(); // 自动释放资源
    }   // 播放一次指定的音效

    /// <summary>
    /// 播放一次常规SE
    /// </summary>
    /// <param name="se">要播放的SE</param>
    public void PlaySe_Common(InGameSe_Common se)
    {
        switch (se)
        {
            case InGameSe_Common.Numbers_In:
                Play_SE("event:/SE/InGame/InGame_Numbers_In");
                break;
            case InGameSe_Common.Numbers_Out:
                Play_SE("event:/SE/InGame/InGame_Numbers_Out");
                break;
            case InGameSe_Common.LiveSuccess:
                Play_SE("event:/SE/InGame/LiveSuccess");
                break;
            case InGameSe_Common.Button_True:
                Play_SE("event:/SE/Normal/True");
                break;
        }
    }

    /// <summary>
    /// 播放音符按键音效
    /// </summary>
    /// <param name="se">要播放的SE</param>
    public void PlaySe_Note(InGameSe_Note se)
    {
        switch (GameSettingsMannger.save_Settings.soundEffectSelection)
        {
            case SoundEffectSelection.Normal:
                NoteSe_Normal(se);
                break;
        }
    }
    void NoteSe_Normal(InGameSe_Note se)
    {
        switch (se)
        {
            case InGameSe_Note.Tap_Empty:
                Play_SE("event:/SE/InGame/KIT_TapEmpty");
                break;
            case InGameSe_Note.Tap1_Bad:
                Play_SE("event:/SE/InGame/KIT_Tap1_Bad");
                break;
            case InGameSe_Note.Tap1_Good:
                Play_SE("event:/SE/InGame/KIT_Tap1_Good");
                break;
            case InGameSe_Note.Tap1_Great:
                Play_SE("event:/SE/InGame/KIT_Tap1_Great");
                break;
            case InGameSe_Note.Tap1_Perfect:
                Play_SE("event:/SE/InGame/KIT_Tap1_Perfect");
                break;
            case InGameSe_Note.Tap2_Bad:
                Play_SE("event:/SE/InGame/KIT_Tap2_Bad");
                break;
            case InGameSe_Note.Tap2_Good:
                Play_SE("event:/SE/InGame/KIT_Tap2_Good");
                break;
            case InGameSe_Note.Tap2_Great:
                Play_SE("event:/SE/InGame/KIT_Tap2_Great");
                break;
            case InGameSe_Note.Tap2_Perfect:
                Play_SE("event:/SE/InGame/KIT_Tap2_Perfect");
                break;
            case InGameSe_Note.Scratch_TapEmpty:
                Play_SE("event:/SE/InGame/Scratch_TapEmpty");
                break;
            case InGameSe_Note.Scratch_Bad:
                Play_SE("event:/SE/InGame/Scratch_Bad");
                break;
            case InGameSe_Note.Scratch_Perfect:
                Play_SE("event:/SE/InGame/Scratch_Perfect");
                break;
            case InGameSe_Note.SliderFlick_Bad:
                Play_SE("event:/SE/InGame/SliderFlick_Bad");
                break;
            case InGameSe_Note.SliderFlick_Perfect:
                Play_SE("event:/SE/InGame/SliderFlick_Perfect");
                break;
        }
    }


    // ————————————————————————————————语音——————————————————————————————————————
    public EventInstance currentVoiceInstance;  // 单一音效播放实例
    /// <summary>
    /// 播放一次指定的语音
    /// </summary>
    /// <param name="path">语音的路径</param>
    public void Play_Vo(string path)
    {
        Play_Vo_Setting(RuntimeManager.CreateInstance(path));
    }
    // 通用的语音音量设置
    private void Play_Vo_Setting(EventInstance instance)
    {
        // 管理音效实例
        Stop_Vo();

        currentVoiceInstance = instance;
        currentVoiceInstance.setVolume(GameSettingsMannger.save_Settings.voiceVolume);
        currentVoiceInstance.start();
    }
    /// <summary>
    /// 停止播放语音
    /// </summary>
    public void Stop_Vo()
    {
        currentVoiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentVoiceInstance.release();
    }


    // ————————————————————————————————音乐——————————————————————————————————————
    // 单一音乐播放实例
    public EventInstance currentSongInstance;
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="path">音乐的路径</param>
    public void Play_Song(string path)
    {
        Stop_Song();

        // 创建新的音乐实例
        currentSongInstance = RuntimeManager.CreateInstance(path);
        currentSongInstance.setVolume(GameSettingsMannger.save_Settings.generalMusicVolume);
        currentSongInstance.start();
    }
    /// <summary>
    /// 停止播放背景音乐
    /// </summary>
    public void Stop_Song()
    {
        // 允许淡出
        currentSongInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentSongInstance.release();
    }


    [Header("选择程序员乐器的事件路径")]
    public EventReference ProgrammerEventReference;
    // 玩家提供的音乐 的播放实例
    public EventInstance playerUploadInstance;
    // 私有的监听事件
    EVENT_CALLBACK dialogueCallback;
    // 音乐长度（单位：毫秒）
    public int playerUploadAudioLength;

    /// <summary>
    /// 获取玩家提供的音乐
    /// </summary>
    /// <param name="path">音乐路径</param>
    public void Get_PlayerUpload_Music(string path)
    {
        playerUploadInstance = RuntimeManager.CreateInstance(ProgrammerEventReference);

        GCHandle stringHandle = GCHandle.Alloc(path);
        playerUploadInstance.setUserData(GCHandle.ToIntPtr(stringHandle));

        playerUploadInstance.setCallback(dialogueCallback);
    }
    /// <summary>
    /// 播放玩家提供的音乐
    /// </summary>
    public bool playerUploadInstance_isPaused;
    public void Play_PlayerUpload_Music()
    {
        playerUploadInstance.getPaused(out playerUploadInstance_isPaused);

        if (playerUploadInstance_isPaused)
        {
            playerUploadInstance.setPaused(false);
            playerUploadInstance_isPaused = false;
        }
        else
        {
            playerUploadInstance.setVolume(GameSettingsMannger.save_Settings.musicVolume);
            playerUploadInstance.start();
            SoundManager.Instance.playerUploadInstance.setTimelinePosition((int)(GameStateManager.Instance.GameTime * 1000));
        }
    }
    /// <summary>
    /// 暂停播放玩家提供的音乐
    /// </summary>
    public void Pause_PlayerUpload_Music()
    {
        playerUploadInstance.setPaused(true);
        playerUploadInstance_isPaused = true;
    }
    /// <summary>
    /// 停止播放玩家提供的音乐
    /// </summary>
    public void Stop_PlayerUpload_Music()
    {
        playerUploadInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        playerUploadInstance.release();
    }
    /// <summary>
    /// 设置玩家提供音乐的倍速
    /// </summary>
    /// <param name="value">倍速</param>
    public void Set_PlayerUpload_Pitch(float value)
    {
        playerUploadInstance.setPitch(value);
    }
    /// <summary>
    /// 实时设置玩家提供的音乐的混音
    /// </summary>
    /// <param name="effectType">混音类型</param>
    /// <param name="mixingIntensity">混音强度</param>
    public void Mixing_PlayerUpload_Music(Note_EffectType effectType, float mixingIntensity)
    {
        switch (effectType)
        {
            case Note_EffectType.MultbandEQFreq:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Multband EQ Freq.", mixingIntensity);
                break;
            case Note_EffectType.ReverbTime:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Reverb Time", mixingIntensity);
                break;
            case Note_EffectType.ThreeEQLow:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("3-EQ Low", mixingIntensity);
                break;
            case Note_EffectType.ThreeEQMid:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("3-EQ Mid", mixingIntensity);
                break;
            case Note_EffectType.ThreeEQHigh:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("3-EQ High", mixingIntensity);
                break;
            case Note_EffectType.Chorus:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Chorus", mixingIntensity);
                break;
            case Note_EffectType.Delay:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Delay", mixingIntensity);
                break;
            case Note_EffectType.FlangerMix:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Flanger Mix", mixingIntensity);
                break;
            case Note_EffectType.Gain:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Gain", mixingIntensity);
                break;
            case Note_EffectType.TremoloFrequency:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Gain", mixingIntensity);
                break;
            case Note_EffectType.Null:
                ResetAll_MixingEffects();
                break;
        }
    }
    /// <summary>
    /// 重置所有混音效果
    /// </summary>
    public void ResetAll_MixingEffects()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Multband EQ Freq.", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Reverb Time", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("3-EQ Low", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("3-EQ Mid", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("3-EQ High", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Chorus", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Delay", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Flanger Mix", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Gain", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Gain", 0);
    }

    // 玩家传入的播放事件 的监听事件函数
    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static FMOD.RESULT DialogueEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);

        IntPtr stringPtr;
        instance.getUserData(out stringPtr);

        GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
        string key = stringHandle.Target as string;


        switch (type)
        {
            case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
                    var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));

                    if (key.Contains("."))
                    {
                        FMOD.Sound dialogueSound;
                        var soundResult = RuntimeManager.CoreSystem.createSound(key, soundMode, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = -1;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    break;
                }
            case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new FMOD.Sound(parameter.sound);
                    sound.release();

                    break;
                }
            case EVENT_CALLBACK_TYPE.SOUND_PLAYED:
                {
                    // 获取玩家传入的音乐的长度
                    uint len;

                    FMOD.Sound sound = new FMOD.Sound(parameterPtr);
                    sound.getLength(out len, FMOD.TIMEUNIT.MS);

                    InGameSoundManager.instance.playerUploadAudioLength = (int)len;
                    break;
                }
            case EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    stringHandle.Free();
                    break;
                }
        }
        return FMOD.RESULT.OK;
    }

}

public enum InGameSe_Note
{
    Tap_Empty,
    Tap1_Bad, Tap1_Good, Tap1_Great, Tap1_Perfect,
    Tap2_Bad, Tap2_Good, Tap2_Great, Tap2_Perfect,
    Scratch_TapEmpty, Scratch_Bad, Scratch_Perfect,
    SliderFlick_Bad, SliderFlick_Perfect,
}
public enum InGameSe_Common
{
    Numbers_In, Numbers_Out,
    LiveSuccess,
    Button_True,
}
