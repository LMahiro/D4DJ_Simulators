using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

// 基础音符类，共同特点的属性和方法
public class Note : MonoBehaviour
{
    // 当前状态
    public virtual INoteState NoteState { get; set; }

    // 对应的谱面变量
    public virtual Event Event { get; set; }

    // 下一个音符的谱面变量（长条、Fader重写）
    public virtual Event Event_Next { get; set; }

    // 绑定的手指
    public virtual LeanFinger Finger { get; set; }

    // 延迟时间
    public virtual float DelayTime { get; set; }

    // 是否为自动判定
    public virtual bool IsAUTO { get; set; }

    // Scratch是否判定成功（仅Scratch重写）
    public virtual bool IsScratchSuccessful { get; set; }

    // 各判定区间时间
    public virtual float justPerfectTime { get; set; }      // JP完美区间
    public virtual float justPerfectDelayTime { get; set; } // JP快慢区间
    public virtual float PerfectTime { get; set; }          // P快慢区间
    public virtual float GreatTime { get; set; }            // G快慢区间
    public virtual float GoodTime { get; set; }
    public virtual float BadTime { get; set; }

    // 在子类中实现的方法
    public virtual void DisplayEffects(int effectNumber,bool isNextEffect = false)
    {
        // 显示特效（特效编号）
    }

    public virtual void UICommunication(JudgmentText judgment, int increaseComboCount,bool isNextEvent = false)
    {
        // UI通信（本次判定，增加Combo数）
    }

    // 切换状态的方法
    public virtual void ChangeState(INoteState State)
    {
        NoteState.ExitState(this);
        NoteState = State;
        NoteState.EnterState(this);
    }
}

// 音符状态机
public interface INoteState
{
    public void EnterState(Note note);
    public void UpdateState(Note note);
    public void ExitState(Note note);
}
