using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SEPlayer : MonoBehaviour
{
    [Header("填写需要播放的音频路径")]
    public EventReference sePath;

    public void PlaySE()
    {
        //RuntimeManager.PlayOneShot(sePath);
        //FMOD.Studio.EventInstance a = FMODUnity.RuntimeManager.CreateInstance(sePath);
        //a.start();
        SoundManager.Instance.Play_SE(sePath);
    }
}
