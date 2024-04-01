using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteLaneManager : MonoBehaviour
{
    private static NoteLaneManager instance;
    public static NoteLaneManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        // 根据设定值修改轨道宽度
        normalLine.localScale = new Vector3(GameSettingsMannger.save_Settings.trackWidth, 1, 1);
        djLine.localScale = new Vector3(GameSettingsMannger.save_Settings.trackWidth, 1, 1);
        notMovingLine.localScale = new Vector3(GameSettingsMannger.save_Settings.trackWidth, 1, 1);



        // 下落时间(秒)
        fallingTime = 0.35f + 3.65f * Mathf.Pow((12f - GameSettingsMannger.save_Settings.scrollSpeed) / 11f, 1.31f);

        // 1秒的下落距离（平均速度）
        fallingDistancePerSecond = totalLength / fallingTime;

        // 计算每固定0.0083333秒移动的距离（每秒执行120次）
        distanceMoverdPerFixedFrame = fallingDistancePerSecond * 0.0083333f;

        // 取为相反数，y值应该为负
        distanceMoverdPerFixedFrame = -distanceMoverdPerFixedFrame;
    }

    public Transform normalLine;
    public Transform djLine;
    public Transform notMovingLine;

    public float distanceMoverdPerFixedFrame;   // 固定帧更新移动的距离
    public float normalLaneDoubleSpeed = 1;     // 帧更新移动距离的倍速
    public float djLaneDoubleSpeed = 1;

    // 轨道相关变量
    public float totalLength = 121.381f;    // 轨道总长度（顶部到判定线）
    public float fallingTime;               // 下落时间(秒)
    public float fallingDistancePerSecond;  // 1秒的下落距离（平均速度）
    void FixedUpdate()
    {
        if (GameStateManager.Instance.gamePaused) return;

        normalLine.localPosition = new Vector3(normalLine.localPosition.x,
                                               normalLine.localPosition.y + distanceMoverdPerFixedFrame * normalLaneDoubleSpeed,
                                               normalLine.localPosition.z);
        djLine.localPosition = new Vector3(djLine.localPosition.x,
                                           djLine.localPosition.y + distanceMoverdPerFixedFrame * djLaneDoubleSpeed,
                                           djLine.localPosition.z);
    }
}
