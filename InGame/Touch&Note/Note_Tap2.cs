using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note_Tap2 : Note
{
    private void Start()
    {
        // 设置缩放
        this.transform.localScale *= GameSettingsMannger.save_Settings.noteSize;

        NoteState = new Tap2_WaitingForJudgment();
        NoteState.EnterState(this);
    }

    // 当前状态
    public override INoteState NoteState { get; set; }

    // 对应的谱面变量
    public override Event Event { get; set; }

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
        EffectsManager.Instance.CreateEffect(effectNumber, Event.laneId);
    }

    public override void UICommunication(JudgmentText judgment, int increaseComboCount, bool isNextEvent = false)
    {
        // UI通信（本次判定）
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
public class Tap2_WaitingForJudgment : INoteState
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
                note.DisplayEffects(0);
                note.UICommunication(JudgmentText.AUTO, 0);
                InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
                note.ChangeState(new Tap1_JudgmentEnded());
            }
        }
        else
        {
            if (note.Event.time - note.BadTime < GameStateManager.Instance.GameTime)
                note.ChangeState(new Tap1_JudgmentInProgress());
        }
    }
}

// 判定中
public class Tap2_JudgmentInProgress : INoteState
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
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_Bad, -1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Bad);
        }
        else if (note.DelayTime < -note.GreatTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_Good, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Good);
        }
        else if (note.DelayTime < -note.PerfectTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_Great, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Great);
        }
        else if (note.DelayTime < -note.justPerfectDelayTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_Perfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < -note.justPerfectTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.justPerfectTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.justPerfectDelayTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Slow_JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.PerfectTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Slow_Perfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
        }
        else if (note.DelayTime < note.GreatTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Slow_Great, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Great);
        }
        else if (note.DelayTime < note.GoodTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Slow_Good, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Good);
        }
        else if (note.DelayTime < note.BadTime)
        {
            note.DisplayEffects(0);
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
            note.ChangeState(new Tap1_JudgmentEnded());
        }
        else if (note.BadTime <= note.DelayTime)
        {
            // 超出判定时间，进入后也是执行Miss的代码
            note.ChangeState(new Tap1_JudgmentEnded());
        }
    }
}

// 判定结束
public class Tap2_JudgmentEnded : INoteState
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
