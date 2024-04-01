using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note_Scratch : Note
{
    private void Start()
    {
        // 设置缩放
        this.transform.localScale *= GameSettingsMannger.save_Settings.noteSize;

        NoteState = new Scratch_WaitingForJudgment();
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

    // Scratch是否判定成功
    private bool _isScratchSuccessful;
    public override bool IsScratchSuccessful { get { return _isScratchSuccessful; } set { _isScratchSuccessful = value; } }

    // 是否为自动判定
    private bool _isAuto;
    public override bool IsAUTO { get { return _isAuto; } set { _isAuto = value; } }

    // 各判定区间时间
    public override float justPerfectTime { get { return 0.100f; } set { } }      // JP完美区间
    public override float justPerfectDelayTime { get { return 0.150f; } set { } } // JP快慢区间
    public override float PerfectTime { get { return 0.200f; } set { } }          // P快慢区间
    public override float GreatTime { get { return 0.250f; } set { } }            // G快慢区间
    public override float GoodTime { get { return 0.300f; } set { } }
    public override float BadTime { get { return 0.400f; } set { } }

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
public class Scratch_WaitingForJudgment : INoteState
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
                note.DisplayEffects(10);
                note.UICommunication(JudgmentText.AUTO, 0);
                InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
                note.ChangeState(new Scratch_JudgmentEnded());
            }
        }
        else
        {
            if (note.Event.time - note.BadTime < GameStateManager.Instance.GameTime)
                note.ChangeState(new Scratch_JudgmentInProgress());
        }
    }
}

// 判定中
public class Scratch_JudgmentInProgress : INoteState
{
    public void EnterState(Note note)
    {
        // 根据所在轨道位置添加至不同的等待判定序列
        if (note.Event.laneId == 0)
            NoteManager.Instance.leftScratchs.Add(note);
        else
            NoteManager.Instance.rightScratchs.Add(note);
    }

    public void ExitState(Note note)
    {
        if (note.Event.laneId == 0)
            NoteManager.Instance.leftScratchs.Remove(note);
        else
            NoteManager.Instance.rightScratchs.Remove(note);

        if (note.DelayTime < -note.GoodTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Fast_Bad, -1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Bad);
        }
        else if (note.DelayTime < -note.GreatTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Fast_Good, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
        }
        else if (note.DelayTime < -note.PerfectTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Fast_Great, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
        }
        else if (note.DelayTime < -note.justPerfectDelayTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Fast_Perfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
        }
        else if (note.DelayTime < -note.justPerfectTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Fast_JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
        }
        else if (note.DelayTime < note.justPerfectTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
        }
        else if (note.DelayTime < note.justPerfectDelayTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Slow_JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
        }
        else if (note.DelayTime < note.PerfectTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Slow_Perfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
        }
        else if (note.DelayTime < note.GreatTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Slow_Great, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
        }
        else if (note.DelayTime < note.GoodTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Slow_Good, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Perfect);
        }
        else if (note.DelayTime < note.BadTime)
        {
            note.DisplayEffects(10);
            note.UICommunication(JudgmentText.Slow_Bad, -1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_Bad);
        }
        else
        {
            note.UICommunication(JudgmentText.Slow_Miss, -1);
        }
    }

    public void UpdateState(Note note)
    {
        note.DelayTime = -(note.Event.time - GameStateManager.Instance.GameTime) + GameSettingsMannger.save_Settings.judgementLatencyAdjustment;

        if (note.IsScratchSuccessful)
        {
            // 已在TouchManager脚本中判定成功
            note.ChangeState(new Scratch_JudgmentEnded());
        }
        else if (note.BadTime <= note.DelayTime)
        {
            // 超出判定时间，进入后也是执行Miss的代码
            note.ChangeState(new Scratch_JudgmentEnded());
        }
    }
}

// 判定结束
public class Scratch_JudgmentEnded : INoteState
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
