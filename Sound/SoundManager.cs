using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class SoundManager : MonoBehaviour
{
    #region 单例模式
    private static SoundManager instance;
    public static SoundManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        // InGame场景有对应的音频管理器
        // DontDestroyOnLoad(this.gameObject);

        // 玩家提供的音乐 的监听函数
        dialogueCallback = new EVENT_CALLBACK(DialogueEventCallback);
    }
    #endregion

    [Header("请选择默认音效")]
    public EventReference defaultSoundEffects;
    /// <summary>
    /// 播放一次默认音效
    /// </summary>
    public void Play_SE()
    {
        Play_SE_Setting(RuntimeManager.CreateInstance(defaultSoundEffects));
    }
    /// <summary>
    /// 播放一次指定的音效
    /// </summary>
    /// <param name="path">音效的路径</param>
    public void Play_SE(EventReference path)
    {
        Play_SE_Setting(RuntimeManager.CreateInstance(path));
    }
    public void Play_SE(string path)
    {
        Play_SE_Setting(RuntimeManager.CreateInstance(path));
    }
    // 通用的音效音量设置
    private void Play_SE_Setting(EventInstance instance)
    {
        instance.setVolume(GameSettingsMannger.save_Settings.generalSoundEffectVolume);
        instance.start();
        instance.release(); // 自动释放资源
    }



    [Header("请选择默认语音")]
    public EventReference defaultVoice;
    // 单一音效播放实例
    public EventInstance currentVoiceInstance;
    /// <summary>
    /// 播放一次默认语音
    /// </summary>
    public void Play_Vo()
    {
        Play_Vo_Setting(RuntimeManager.CreateInstance(defaultVoice));
    }
    /// <summary>
    /// 播放一次指定的语音
    /// </summary>
    /// <param name="path">语音的路径</param>
    public void Play_Vo(EventReference path)
    {
        Play_Vo_Setting(RuntimeManager.CreateInstance(path));
    }
    public void Play_Vo(string path)
    {
        Play_Vo_Setting(RuntimeManager.CreateInstance(path));
    }
    // 通用的语音音量设置
    private void Play_Vo_Setting(EventInstance instance)
    {
        //instance.setVolume(Game_Settings_Mannger.save_Settings.generalVoiceVolume);
        //instance.start();
        //instance.release(); // 自动释放资源

        // 管理音效实例
        Stop_Vo();

        currentVoiceInstance = instance;
        currentVoiceInstance.setVolume(GameSettingsMannger.save_Settings.generalVoiceVolume);
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



    // 上一次调用的路径
    private EventReference last_Path;
    // 单一音乐播放实例
    public EventInstance currentSongInstance;
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="path">音乐的路径</param>
    public void Play_Song(EventReference path)
    {
        // 判断重新播放的音乐是否为当前播放的音乐
        if (last_Path.ToString() == path.ToString())
            return;
        else
            last_Path = path;

        Stop_Song();

        // 创建新的音乐实例
        currentSongInstance = RuntimeManager.CreateInstance(path);
        currentSongInstance.setVolume(GameSettingsMannger.save_Settings.generalMusicVolume);
        currentSongInstance.start();
    }
    /// <summary>
    ///  播放上次终止的背景音乐
    /// </summary>
    public void Play_Song()
    {
        Stop_Song();

        currentSongInstance = RuntimeManager.CreateInstance(last_Path);
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
    private EVENT_CALLBACK dialogueCallback;
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
            playerUploadInstance.setVolume(GameSettingsMannger.save_Settings.generalMusicVolume);
            playerUploadInstance.start();
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

                    SoundManager.instance.playerUploadAudioLength = (int)len;
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
