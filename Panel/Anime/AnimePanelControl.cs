using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using UnityEngine.UI;

public class AnimePanelControl : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public TextMeshProUGUI videoTime;
    public GameObject UIFather; // UI父空对象
    public AudioSource audioSource;

    void Awake()
    {
        SoundManager.Instance.Stop_Song();
        PanelMannger.Instance.Button_Back_Only();
        audioSource.volume = GameSettingsMannger.save_Settings.generalMusicVolume;
    }
    void OnDestroy()
    {
        SoundManager.Instance.Play_Song();
        PanelMannger.Instance.Button_Show_All();
    }
    void Update()
    {
        videoTime.text = $"{FormatTime(videoPlayer.time)} / {FormatTime(videoPlayer.length)}";
    }
    private string FormatTime(double time)
    {
        // 将秒转换为分钟、秒和毫秒
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        //int milliseconds = (int)((time * 1000) % 1000);

        return string.Format("{0}:{1:00}", minutes, seconds);
    }

    bool isDisplayUI = true;
    public void UIDisplaySwitching()
    {
        if(isDisplayUI)
        {
            // 隐藏UI
            PanelMannger.Instance.Button_Hide_All();
            PanelMannger.Instance.Title_Hide_All();
            SoundManager.Instance.Play_SE("event:/SE/Normal/Cancel");
            UIFather.SetActive(false);
            isDisplayUI = false;
        }
        else
        {
            // 显示UI
            PanelMannger.Instance.Button_Back_Only();
            PanelMannger.Instance.Title_Show_All();
            SoundManager.Instance.Play_SE("event:/SE/Normal/True");
            UIFather.SetActive(true);
            isDisplayUI = true;
        }
    }
}
