using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Mixing : MonoBehaviour
{
    public TextMeshProUGUI threeEQLow_Value;
    public void _ThreeEQLow(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.ThreeEQLow, input);
        threeEQLow_Value.text = (input * 100).ToString("f0") + "%";
    }

    public TextMeshProUGUI threeEQMid_Value;
    public void _ThreeEQMid(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.ThreeEQMid, input);
        threeEQMid_Value.text = (input * 100).ToString("f0") + "%";
    }

    public TextMeshProUGUI threeEQHigh_Value;
    public void _ThreeEQHigh(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.ThreeEQHigh, input);
        threeEQHigh_Value.text = (input * 100).ToString("f0") + "%";
    }

    public TextMeshProUGUI multbandEQFreq_Value;
    public void _MultbandEQFreq(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.MultbandEQFreq, input);
        multbandEQFreq_Value.text = (input * 100).ToString("f0") + "%";
    }

    public TextMeshProUGUI chorus_Value;
    public void _Chorus(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.Chorus, input);
        chorus_Value.text = (input * 100).ToString("f0") + "%";
    }

    public TextMeshProUGUI delay_Value;
    public void _Delay(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.Delay, input);
        delay_Value.text = (input * 100).ToString("f0") + "%";
    }

    public TextMeshProUGUI flangerMix_Value;
    public void _FlangerMix(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.FlangerMix, input);
        flangerMix_Value.text = (input * 100).ToString("f0") + "%";
    }

    public TextMeshProUGUI gain_Value;
    public void _Gain(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.Gain, input);
        gain_Value.text = (input * 100).ToString("f0") + "%";
    }

    public TextMeshProUGUI tremoloFrequency_Value;
    public void _TremoloFrequency(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.TremoloFrequency, input);
        tremoloFrequency_Value.text = (input * 100).ToString("f0") + "%";
    }

    public TextMeshProUGUI reverbTime_Value;
    public void _ReverbTime(float input)
    {
        SoundManager.Instance.Mixing_PlayerUpload_Music(Note_EffectType.ReverbTime, input);
        reverbTime_Value.text = (input * 100).ToString("f0") + "%";
    }
}
