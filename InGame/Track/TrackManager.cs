using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    private static TrackManager instance;
    public static TrackManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        // 根据设定值修改轨道宽度
        trackMainOBJ.localScale = new Vector3(GameSettingsMannger.save_Settings.trackWidth, 1, 1);

        // 根据设定值修改整体轨道高度，也就是修改判定线高度
        movable.localPosition = new Vector3(movable.localPosition.x,
                                                movable.localPosition.y + (GameSettingsMannger.save_Settings.judgementLineHeight / 10),
                                                movable.localPosition.z);

        laneBase.localScale = new Vector3(laneBase.localScale.x,
            laneBase.localScale.y * GameSettingsMannger.save_Settings.trackLength,
            laneBase.localScale.z);

        SpriteRenderer spriteRenderer = laneBase.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            spriteRenderer.color.a * GameSettingsMannger.save_Settings.trackOpacity);

        for (int i = 0; i < line.Length; i++)
        {
            line[i].color = new Color(spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                spriteRenderer.color.a * GameSettingsMannger.save_Settings.trackDividerOpacity);
        }
    }

    [Header("拖入整根轨道的父对象")]
    public RectTransform trackMainOBJ;

    [Header("轨道动画相关")]
    public Animation shaking;   // 轨道抖动

    [Header("轨道个性化相关")]
    public Transform movable;   // 父对象下的第一个子对象
    public Transform anchorPoint;   // 区分Fader和判定线的定位点
    public Transform laneBase; // 轨道
    public SpriteRenderer[] line;   // 轨道分割线

    public void shakingTrack()
    {
        if (GameSettingsMannger.save_Settings.screenShake == false)
            return;

        shaking.Play();
    }   // 轨道抖动动画

    /// <summary>
    /// 返回以判定线为父对象下的子对象的局部位置
    /// </summary>
    /// <param name="lane"></param>
    /// <returns></returns>
    public float ReturnToLocalCoordinates(int lane)
    {
        return -6.2997f + lane * 2.1072f;
    }
}
