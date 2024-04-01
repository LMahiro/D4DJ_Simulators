using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum Action
{
    调整常规音量大小,
    播放默认音效,
    播放默认语音,
}
public class FingerRelease_Preview : MonoBehaviour, IPointerUpHandler
{
    [Header("选择松开时的操作")]
    public Action chooseAction;

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (chooseAction)
        {
            case Action.调整常规音量大小:
                SoundManager.Instance.currentSongInstance.setVolume(GameSettingsMannger.save_Settings.generalMusicVolume);
                break;
            case Action.播放默认音效:
                SoundManager.Instance.Play_SE();
                break;
            case Action.播放默认语音:
                SoundManager.Instance.Play_Vo();
                break;
            default:
                Debug.LogWarning("FingerRelease_Preview：没有选定的操作。");
                break;
        }
    }
}
