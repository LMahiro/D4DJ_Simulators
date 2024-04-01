using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    private static EffectsManager instance;
    public static EffectsManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }

    [Header("拖入所有特效预制体")]
    public GameObject[] allEffectPrefab;
    [Header("拖入特效预制体父对象")]
    public Transform effectFatherOBJ;
    [Header("碟子特效相关")]
    public Animation diskLeftOrangeOBJ;
    public GameObject diskLeftRedOBJ;
    public Animation diskRightOrangeOBJ;
    public GameObject diskRightRedOBJ;


    /// <summary>
    /// 创建特效，根据传入的值进行不同的特效播放
    /// </summary>
    /// <param name="effectNumber">特效序号：0=Tap，1=Hold，2=Fader，3=FadershakingTrack，10=Scratch，20=StopStart，21=StopEnd，22=StopEnd但不播放动画</param>
    /// <param name="trackPosition">轨道位置(int)</param>
    public void CreateEffect(int effectNumber, int trackPosition)
    {
        GameObject effectOBJ = null;

        if (0 <= effectNumber && effectNumber <= 3)
        {
            // 0-3
            effectOBJ = Instantiate(allEffectPrefab[effectNumber], effectFatherOBJ);
            if (effectNumber == 3)
                TrackManager.Instance.shakingTrack();
        }
        else
        {
            if (effectNumber != 22)
                effectOBJ = Instantiate(allEffectPrefab[3], effectFatherOBJ);

            if (trackPosition == 0)
            {
                switch (effectNumber)
                {
                    case 10:
                        diskLeftOrangeOBJ.Play();
                        break;
                    case 20:
                        diskLeftRedOBJ.SetActive(true);
                        break;
                    case 21:
                        diskLeftRedOBJ.SetActive(false);
                        break;
                    case 22:
                        diskLeftRedOBJ.SetActive(false);
                        break;
                }
            }
            else if (trackPosition == 6)
            {
                switch (effectNumber)
                {
                    case 10:
                        diskRightOrangeOBJ.Play();
                        break;
                    case 20:
                        diskRightRedOBJ.SetActive(true);
                        break;
                    case 21:
                        diskRightRedOBJ.SetActive(false);
                        break;
                    case 22:
                        diskRightRedOBJ.SetActive(false);
                        break;
                }
            }
        }

        // 设置特效位置。x根据轨道位置计算，y根据父子关系自动变化，z固定
        if (effectNumber != 22)
        {
            //effectOBJ.transform.localPosition = Vector3.zero;
            //effectOBJ.transform.position = new Vector3(TouchManager.Instance.ReturnToWorldCoordinates(trackPosition),
            //                                           effectOBJ.transform.position.y,
            //                                           effectOBJ.transform.position.z);
            effectOBJ.transform.localPosition = new Vector3(TrackManager.Instance.ReturnToLocalCoordinates(trackPosition),
                                                       0, 0);
        }


    }
}
