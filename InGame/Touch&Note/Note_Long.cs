using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note_Long : Note
{
    private void Start()
    {
        // 设置缩放
        //this.transform.localScale *= GameSettingsMannger.save_Settings.noteSize;

        NoteState = new Long_WaitingForJudgment();
        NoteState.EnterState(this);

        // 计算长条移动时间
        moveTime = Event_Next.time - Event.time;
    }

    // 按键点
    public Transform up;
    public Transform down;
    public float moveTime;

    // 当前状态
    public override INoteState NoteState { get; set; }

    // 对应的谱面变量
    public override Event Event { get; set; }

    // 长条的下一个音符的谱面变量
    public override Event Event_Next { get; set; }

    // 绑定的手指
    public override LeanFinger Finger { get; set; }

    // 延迟时间（单独增加私有字段为了防止重复使用相同的自动属性字段）
    private float _delayTime;
    public override float DelayTime { get { return _delayTime; } set { _delayTime = value; } }

    // 是否为自动判定
    private bool _isAuto;
    public override bool IsAUTO { get { return _isAuto; } set { _isAuto = value; } }

    // 各判定区间时间
    public override float justPerfectTime { get { return 0.010f; } set { } }      // JP完美区间
    public override float justPerfectDelayTime { get { return 0.025f; } set { } } // JP快慢区间
    public override float PerfectTime { get { return 0.050f; } set { } }          // P快慢区间
    public override float GreatTime { get { return 0.100f; } set { } }            // G快慢区间
    public override float GoodTime { get { return 0.150f; } set { } }
    public override float BadTime { get { return 0.200f; } set { } }

    // 在子类中实现的方法
    public override void DisplayEffects(int effectNumber, bool isNextEffect = false)
    {
        // 显示特效（特效编号）
        if (isNextEffect)
            EffectsManager.Instance.CreateEffect(effectNumber, Event_Next.laneId);
        else
            EffectsManager.Instance.CreateEffect(effectNumber, Event.laneId);
    }

    public override void UICommunication(JudgmentText judgment, int increaseComboCount, bool isNextEvent = false)
    {
        // UI通信（本次判定）
        if (isNextEvent)
            JudgmentManager.Instance.Judgment(judgment, DelayTime * 1000, Event_Next.laneId, IsAUTO);
        else
            JudgmentManager.Instance.Judgment(judgment, DelayTime * 1000, Event.laneId, IsAUTO);
        // 增加Combo数
        ComboManager.Instance.addCombo(increaseComboCount);
    }

    // 切换状态的方法
    public override void ChangeState(INoteState State)
    {
        NoteState.ExitState(this);
        NoteState = State;
        NoteState.EnterState(this);
    }

    private void FixedUpdate()
    {
        // 尾节点下落
        if (this.Event.time - GameStateManager.Instance.GameTime <= 0)
        {
            up.localPosition = new Vector3(up.localPosition.x,
                up.localPosition.y - -(NoteLaneManager.Instance.distanceMoverdPerFixedFrame / 100),
                up.localPosition.z);
        }
    }
    private void Update()
    {
        // 根据当前状态执行相应的操作
        NoteState.UpdateState(this);

        // 是否显示
        IsDisplayed();
    }
    bool hideExecuted;
    bool showExecuted;  // 显示代码是否已被执行过
    void IsDisplayed()
    {
        // 是否显示
        if (this.transform.position.y < GameStateManager.Instance.noteHiddenValue)
        {
            if (showExecuted == false)
            {
                // 7为显示的layer标签
                ChangeLayer(this.transform, 7);
                showExecuted = true;
            }
        }
        else
        {
            if (hideExecuted == false)
            {
                // 16为不显示的layer标签
                ChangeLayer(this.transform, 16);
                hideExecuted = true;
            }
        }
    }
    void ChangeLayer(Transform obj, int layerIndex)
    {
        obj.gameObject.layer = layerIndex;

        // 遍历一个物体的transform遍历其子物体
        foreach (Transform child in obj.transform)
        {
            ChangeLayer(child, layerIndex);
        }
    }
}

// 等待判定
public class Long_WaitingForJudgment : INoteState
{
    public void EnterState(Note note)
    {
        NoteManager.Instance.allNotes.Add(note);
    }

    public void ExitState(Note note)
    {
    }

    public void UpdateState(Note note)
    {
        if (note.IsAUTO)
        {
            if (note.Event.time - GameStateManager.Instance.GameTime <= 0)
            {
                note.DisplayEffects(1);
                note.UICommunication(JudgmentText.AUTO, 0);
                InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
                note.ChangeState(new Long_EndpointDetermination());
            }
        }
        else
        {
            if (note.Event.time - note.BadTime < GameStateManager.Instance.GameTime)
                note.ChangeState(new Long_StartingPointDetermination());
        }
    }
}

// 判定中：起点
public class Long_StartingPointDetermination : INoteState
{
    public void EnterState(Note note)
    {
        NoteManager.Instance.judgments.Add(note);
    }

    public void ExitState(Note note)
    {
        NoteManager.Instance.judgments.Remove(note);

        if (note.DelayTime < -note.GoodTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Fast_Bad, -1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Bad);
        }
        else if (note.DelayTime < -note.GreatTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Fast_Good, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Good);
        }
        else if (note.DelayTime < -note.PerfectTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Fast_Great, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Great);
        }
        else if (note.DelayTime < -note.justPerfectDelayTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Fast_Perfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < -note.justPerfectTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Fast_JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.justPerfectTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.justPerfectDelayTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Slow_JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.PerfectTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Slow_Perfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.GreatTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Slow_Great, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Great);
        }
        else if (note.DelayTime < note.GoodTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Slow_Good, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Good);
        }
        else if (note.DelayTime < note.BadTime)
        {
            note.DisplayEffects(1);
            note.UICommunication(JudgmentText.Slow_Bad, -1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Bad);
        }
        else
        {
            note.UICommunication(JudgmentText.Slow_Miss, -1);
        }
    }

    public void UpdateState(Note note)
    {
        note.DelayTime = -(note.Event.time - GameStateManager.Instance.GameTime) + GameSettingsMannger.save_Settings.judgementLatencyAdjustment;

        if (note.Finger != null)
        {
            // 已被分配手指（判定成功）
            note.ChangeState(new Long_EndpointDetermination());
        }
        else if (note.BadTime <= note.DelayTime)
        {
            // 超出判定时间，整个长条直接结束判定
            note.ChangeState(new Long_JudgmentEnded());
        }
    }
}

// 判定中：终点
public class Long_EndpointDetermination : INoteState
{
    public void EnterState(Note note)
    {
        // 设置为不移动的轨道
        note.transform.parent = NoteLaneManager.Instance.notMovingLine;
        note.transform.localPosition = Vector3.zero;
        // 强制转换类型
        Note_Long noteLong = note as Note_Long;

        startCoordinate = noteLong.down.localPosition.x;
        targetCoordinate = noteLong.up.localPosition.x;
        moveTime = noteLong.moveTime;
        down = noteLong.down;
    }

    public void ExitState(Note note)
    {
        // 自动播放则直接跳出
        if (note.IsAUTO) return;

        if (note.DelayTime < -note.BadTime)
        {
            note.UICommunication(JudgmentText.Fast_Miss, -1, true);
        }
        else if (note.DelayTime < -note.GoodTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Fast_Bad, -1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Bad);
        }
        else if (note.DelayTime < -note.GreatTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Fast_Good, 1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Good);
        }
        else if (note.DelayTime < -note.PerfectTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Fast_Great, 1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Great);
        }
        else if (note.DelayTime < -note.justPerfectDelayTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Fast_Perfect, 1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < -note.justPerfectTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Fast_JustPerfect, 1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.justPerfectTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.JustPerfect, 1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.justPerfectDelayTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Slow_JustPerfect, 1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.PerfectTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Slow_Perfect, 1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.GreatTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Slow_Great, 1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Great);
        }
        else if (note.DelayTime < note.GoodTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Slow_Good, 1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Good);
        }
        else if (note.DelayTime < note.BadTime)
        {
            note.DisplayEffects(1, true);
            note.UICommunication(JudgmentText.Slow_Bad, -1, true);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Bad);
        }
        else
        {
            note.UICommunication(JudgmentText.Slow_Miss, -1, true);
        }
    }

    public void UpdateState(Note note)
    {
        note.DelayTime = -(note.Event.time - GameStateManager.Instance.GameTime) + GameSettingsMannger.save_Settings.judgementLatencyAdjustment;

        if (note.IsAUTO)
        {
            if (note.Event_Next.time - GameStateManager.Instance.GameTime <= 0)
            {
                note.DisplayEffects(1, true);
                note.UICommunication(JudgmentText.AUTO, 0, true);
                InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
                note.ChangeState(new Long_JudgmentEnded());
            }
        }
        else
        {
            if (note.Event_Next.laneId - 0.5f < note.Finger.TrackPosition && note.Finger.TrackPosition < note.Event_Next.laneId + 1.5f)
            {
                // 持续判定是否在范围内

                if (note.Finger.TouchStage == 4)
                {
                    // 抬起手指（立即判定）
                    note.ChangeState(new Long_JudgmentEnded());
                }
                else if (note.Event_Next.time - GameStateManager.Instance.GameTime <= 0)
                {
                    // 一直按住且达到了尾判时间
                    note.DisplayEffects(1, true);
                    note.UICommunication(JudgmentText.JustPerfect, 1, true);
                    InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
                    note.IsAUTO = true; // Auto设置为True使得ExitState直接跳出
                    note.ChangeState(new Long_JudgmentEnded());
                }
            }
            else if (note.Finger.TouchStage == 4)
            {
                // 不在范围内抬起手指（立即判定）
                note.UICommunication(JudgmentText.Slow_Miss, -1, true);
                note.IsAUTO = true; // Auto设置为True使得ExitState直接跳出
                note.ChangeState(new Long_JudgmentEnded());
            }
            else if (note.BadTime <= note.DelayTime)
            {
                // 超出判定时间（长条虽然一直按住但不在指定位置内），切换后也是Miss
                note.ChangeState(new Long_JudgmentEnded());
            }
        }


        // 如果当前时间小于移动时间，执行移动逻辑
        if (currentTime < moveTime)
        {
            MoveNote();
            currentTime += Time.deltaTime;
        }
    }

    float startCoordinate;  // 起始x坐标
    float targetCoordinate; // 目标x坐标
    float moveTime;         // 移动时间（秒）
    Transform down;        // 长条底部音符对象

    float currentTime = 0;  // 当前时间（秒）
    void MoveNote()
    {
        // 计算当前位置的百分比
        float t = currentTime / moveTime;

        // 使用Lerp函数计算新的x坐标
        float newX = Mathf.Lerp(startCoordinate, targetCoordinate, t);

        // 更新物体的位置
        down.localPosition = new Vector3(newX, down.localPosition.y, down.localPosition.z);
    }
}

// 判定结束
public class Long_JudgmentEnded : INoteState
{
    public void EnterState(Note note)
    {
        NoteManager.Instance.allNotes.Remove(note);
        GameObject.Destroy(note.gameObject);
    }

    public void ExitState(Note note)
    {

    }

    public void UpdateState(Note note)
    {

    }
}
