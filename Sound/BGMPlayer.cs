using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BGMPlayer : MonoBehaviour
{
    [Header("填写需要播放的音乐路径")]
    public EventReference songPath;
    [Header("是否在脚本创建时播放")]
    public bool isPlayAtStart = true;

    public void PlayBGM()
    {
        SoundManager.Instance.Play_Song(songPath);
    }

    private void Start()
    {
        if (isPlayAtStart)
            PlayBGM();
    }
}
