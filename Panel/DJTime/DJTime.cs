using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using UnityEngine.Events;
using System.IO;

public class DJTime : MonoBehaviour
{
    private void Awake()
    {
        SoundManager.Instance.Stop_Song();
        PanelMannger.Instance.Button_Hide_All();
        SoundManager.Instance.Stop_PlayerUpload_Music();
    }
    public void _Back()
    {
        PanelMannger.Instance.GoBackTo_PreviousLeve();
        PanelMannger.Instance.Button_Show_All();

        SoundManager.Instance.Stop_PlayerUpload_Music();
        SoundManager.Instance.ResetAll_MixingEffects();
        SoundManager.Instance.Play_Song();
    }

    #region 音乐进度条
    void Update()
    {
        if (valueReadCompleted)
        {
            Update_MusicBar();
        }
    }

    bool valueReadCompleted;

    public TextMeshProUGUI playTime; public Slider playTime_Slider;
    private int playTime_Now; int FinalDragTime = 0;
    private void Update_MusicBar()
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
    private string FormatTime(int time)
    {
        // 将毫秒转换为分钟、秒和毫秒
        int minutes = time / (60 * 1000);
        int seconds = (time % (60 * 1000)) / 1000;
        int remainingMilliseconds = time % 1000;

        return string.Format("{0}:{1:00}.{2:0}", minutes, seconds, remainingMilliseconds / 100);
    }
    public void _OnvalueChanged_PlayTime_Slider(float value)
    {
        if (SoundManager.Instance.playerUploadInstance_isPaused)
        {
            FinalDragTime = (int)(SoundManager.Instance.playerUploadAudioLength * value);
            playTime.text = FormatTime(FinalDragTime) + " / " + FormatTime(SoundManager.Instance.playerUploadAudioLength);
        }
    }
    public void _Play_UploadAudio()
    {
        if (valueReadCompleted)
        {
            SoundManager.Instance.playerUploadInstance.setTimelinePosition(FinalDragTime);
            SoundManager.Instance.Play_PlayerUpload_Music();
        }
    }
    public void _Pause_UploadAudio()
    {
        if (valueReadCompleted)
        {
            SoundManager.Instance.Pause_PlayerUpload_Music();
        }
    }
    #endregion

    #region 导入音乐
    public TextMeshProUGUI musicName;

    public void _Specify_Audio()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("所指定的音频文件", ".wav", ".mp3"));
        FileBrowser.SetDefaultFilter(".wav");

        PanelMannger.Instance.Show_NowLoading_Panel();
        StartCoroutine(LoadingForPlayers("选择.wav或.mp3音频文件", ".wav", ".mp3"));
    }

    private string audio_ReadPath;
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

        valueReadCompleted = true;
        _Play_UploadAudio();
    }

    // 选择路径
    IEnumerator LoadingForPlayers(string title, params string[] fileExtension)
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
                    Specify_Audio_Action();
                    musicName.text = FileBrowserHelpers.GetFilename(audio_ReadPath);
                    break;                  // 发现匹配值，跳出循环
                }
                else
                {
                    i++;
                    if (i == fileExtension.Length)
                        PanelMannger.Instance.Create_PopUP("读取失败", "不是所指定后缀的文件" +
                                                "\n\n※不要点击右下角的\"所有文件\"来修改文件选择范围",
                                                new Set_PopUP_Button("确认", () => { }));
                }
            }
        }
        else
            PanelMannger.Instance.Create_PopUP("确认", "你取消了本次文件读取的操作。",
                                                new Set_PopUP_Button("确认", () => { }));

        PanelMannger.Instance.Hide_NowLoading_Panel();
    }
    #endregion
}
