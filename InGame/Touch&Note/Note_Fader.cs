using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note_Fader : Note
{
    // Fader的滑条
    public Transform fader_Slide;
    private void Start()
    {
        // 设置缩放
        Vector3 childScale = fader_Slide.localScale;    // 首先得到Slide的局部缩放值，因为其不应被缩放
        this.transform.localScale *= GameSettingsMannger.save_Settings.noteSize;
        fader_Slide.localScale = new Vector3(childScale.x / GameSettingsMannger.save_Settings.noteSize,
            childScale.y / GameSettingsMannger.save_Settings.noteSize,
            0);

        // 设置Slide长度
        SpriteRenderer spriteRenderer = fader_Slide.GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2(Mathf.Abs(spriteRenderer.size.x * this.Event.direction),
            spriteRenderer.size.y);
        if (this.Event.direction == 0)
        {
            // 不是滑动类型
            Destroy(fader_Slide.gameObject);
        }
        else if (this.Event.direction < 0)
        {
            // 左滑需要将y旋转设置为180度，右滑则无需修改
            fader_Slide.localEulerAngles = new Vector3(fader_Slide.localEulerAngles.x,
                180,
                fader_Slide.localEulerAngles.z);
        }

        NoteState = new Fader_WaitingForJudgment();
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

    // 各判定区间时间，判定较宽
    public override float justPerfectTime { get { return 0.050f; } set { } }      // JP完美区间
    public override float justPerfectDelayTime { get { return 0.100f; } set { } } // JP快慢区间
    public override float PerfectTime { get { return 0.150f; } set { } }          // P快慢区间
    public override float GreatTime { get { return 0.200f; } set { } }            // G快慢区间
    public override float GoodTime { get { return 0.250f; } set { } }
    public override float BadTime { get { return 0.300f; } set { } }

    // 音符个性代码
    public override Event Event_Next { get; set; }  // 下一个音符的谱面变量

    public Transform thisTransform;     // 自身坐标，用于连接线定位点，在检查器窗口中拖入
    public Transform nextTransform;     // 下一音符的坐标，用于连接线定位点，生成谱面时动态给予
    public LineRenderer lineRenderer;   // 连接线组件，在检查器窗口中拖入
    // 设置箭头宽度......


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

    // 注意这里是LateUpdate，因为获取NoteManager中的Fader排序是在Update中进行的
    private void LateUpdate()
    {
        // 更新连接线位置。若处在隐藏音符的位置上也将其实坐标设置为自身
        if (Event.nextId != 0 && Event_Next != null && nextTransform != null)
        {
            lineRenderer.SetPosition(0, thisTransform.position);
            lineRenderer.SetPosition(1, nextTransform.position);

            // 若下一节点处在隐藏的位置上，则将原本的连接线定位点的y设置为隐藏音符的坐标
            //if (nextTransform.position.y < GameStateManager.Instance.noteHiddenValue)
            //    lineRenderer.SetPosition(1, nextTransform.position);
            //else
            //    //lineRenderer.SetPosition(1, new Vector3(nextTransform.position.x,
            //    //    GameStateManager.Instance.noteHiddenValue,
            //    //    nextTransform.position.z));
            //    lineRenderer.SetPosition(1, thisTransform.position);
        }
        else
        {
            // 没有连接线的话 连接线起始坐标始终设置为自身，表现出没有连接线
            lineRenderer.SetPosition(0, thisTransform.position);
            lineRenderer.SetPosition(1, thisTransform.position);
        }

        // 根据当前状态执行相应的操作
        NoteState.UpdateState(this);
    }

    private void Update()
    {
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
public class Fader_WaitingForJudgment : INoteState
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
        if (note.Event.direction == 0)
        {
            // 无滑动长度 => 节点类型
            if (note.Event.time - GameStateManager.Instance.GameTime <= 0)
            {
                if (note.IsAUTO)
                {
                    note.DisplayEffects(2);
                    note.UICommunication(JudgmentText.AUTO, 0);
                    InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap2_Perfect);
                    NoteManager.Instance.isFaderIn = true;

                    // 若此节点后还有节点，则需要保持连接直到遇到下一个节点
                    if (note.Event.nextId != 0)
                        note.ChangeState(new Fader_JudgmentEnded_Move());
                    else
                        note.ChangeState(new Fader_JudgmentEnded());
                }
                else
                    note.ChangeState(new Fader_JudgmentInProgress_Node());

            }
        }
        else
        {
            // 有滑动长度 => 滑动类型
            if (note.IsAUTO)
            {
                if (note.Event.time - GameStateManager.Instance.GameTime <= 0)
                {
                    note.DisplayEffects(3);
                    note.UICommunication(JudgmentText.AUTO, 0);
                    InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);

                    note.ChangeState(new Fader_JudgmentEnded());
                }
            }
            else
            {
                if (note.Event.time - note.BadTime < GameStateManager.Instance.GameTime)
                    note.ChangeState(new Fader_JudgmentInProgress_Slide());
            }
        }
    }
}

// 判定中：节点类型
public class Fader_JudgmentInProgress_Node : INoteState
{
    public void EnterState(Note note)
    {
        NoteManager.Instance.judgments.Add(note);
        NoteManager.Instance.isFaderIn = true;
    }

    public void ExitState(Note note)
    {
        // 节点类型音符没有提早判定
        if (note.DelayTime < note.justPerfectTime)
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
        // 必须是判定区间第一个元素，否则跳过本次判定
        if (NoteManager.Instance.allFaders[0] != note) return;

        note.DelayTime = -(note.Event.time - GameStateManager.Instance.GameTime) + GameSettingsMannger.save_Settings.judgementLatencyAdjustment;


        if (note.Event.laneId - 0.5f < NoteManager.Instance.faderTrackPosition && NoteManager.Instance.faderTrackPosition < note.Event.laneId + 1.5f)
        {
            // 只要Fader在判定位置内就立即判定
            // 若此节点后还有节点，则需要保持连接直到遇到下一个节点
            if (note.Event.nextId != 0)
                note.ChangeState(new Fader_JudgmentEnded_Move());
            else
                note.ChangeState(new Fader_JudgmentEnded());
        }
        else if (note.BadTime <= note.DelayTime)
        {
            // 超出判定时间，进入后也是执行Miss的代码
            note.ChangeState(new Fader_JudgmentEnded());
        }
    }
}

// 判定中：滑动类型
public class Fader_JudgmentInProgress_Slide : INoteState
{
    // 滑动Fader的具体轨道判定位置
    float leftJudgmentIntervalTrackPosition;
    float rightJudgmentIntervalTrackPosition;

    public void EnterState(Note note)
    {
        NoteManager.Instance.judgments.Add(note);
        // 滑动类型Fader不设置isFaderIn

        // 根据滑动长度方向最终计算得到具体轨道判定位置
        // 可以画张图，laneId为0、1...6 7根轨道。TrackPosition指的是>0 - 7<的轨道线范围，需要区分
        if (note.Event.direction < 0)
        {
            rightJudgmentIntervalTrackPosition = note.Event.laneId + 1;
            leftJudgmentIntervalTrackPosition = note.Event.laneId + note.Event.direction;
        }
        else
        {
            leftJudgmentIntervalTrackPosition = note.Event.laneId;
            rightJudgmentIntervalTrackPosition = note.Event.laneId + note.Event.direction + 1;
        }
    }

    public void ExitState(Note note)
    {
        if (note.DelayTime < -note.GoodTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_Bad, -1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Bad);
        }
        else if (note.DelayTime < -note.GreatTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_Good, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);
        }
        else if (note.DelayTime < -note.PerfectTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_Great, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);
        }
        else if (note.DelayTime < -note.justPerfectDelayTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_Perfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);
        }
        else if (note.DelayTime < -note.justPerfectTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Fast_JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);
        }
        else if (note.DelayTime < note.justPerfectTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);
        }
        else if (note.DelayTime < note.justPerfectDelayTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Slow_JustPerfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);
        }
        else if (note.DelayTime < note.PerfectTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Slow_Perfect, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);
        }
        else if (note.DelayTime < note.GreatTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Slow_Great, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);
        }
        else if (note.DelayTime < note.GoodTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Slow_Good, 1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Perfect);
        }
        else if (note.DelayTime < note.BadTime)
        {
            note.DisplayEffects(0);
            note.UICommunication(JudgmentText.Slow_Bad, -1);
            InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.SliderFlick_Bad);
        }
        else
        {
            note.UICommunication(JudgmentText.Slow_Miss, -1);
        }
    }

    public void UpdateState(Note note)
    {
        // 必须是判定区间第一个元素，否则跳过本次判定
        if (NoteManager.Instance.allFaders[0] != note) return;

        note.DelayTime = -(note.Event.time - GameStateManager.Instance.GameTime) + GameSettingsMannger.save_Settings.judgementLatencyAdjustment;

        if (NoteManager.Instance.faderFinger != null)
        {
            // 只要一有正确方向的移动就立即判定
            if (note.Event.direction < 0)
            {
                // 左滑。满足1.手指在滑动 2.向左滑 3.Fader在长度范围内
                if (NoteManager.Instance.faderFinger.TouchStage == 3 &&
                NoteManager.Instance.faderFinger.ScreenDelta.x < 0 &&
                leftJudgmentIntervalTrackPosition <= NoteManager.Instance.faderTrackPosition &&
                NoteManager.Instance.faderTrackPosition <= rightJudgmentIntervalTrackPosition)
                {
                    note.ChangeState(new Fader_JudgmentEnded());
                }
            }
            else
            {
                // 右滑
                if (NoteManager.Instance.faderFinger.TouchStage == 3 &&
                0 < NoteManager.Instance.faderFinger.ScreenDelta.x &&
                leftJudgmentIntervalTrackPosition <= NoteManager.Instance.faderTrackPosition &&
                NoteManager.Instance.faderTrackPosition <= rightJudgmentIntervalTrackPosition)
                {
                    note.ChangeState(new Fader_JudgmentEnded());
                }
            }
        }
        else if (note.BadTime <= note.DelayTime)
        {
            // 超出判定时间，进入后也是执行Miss的代码
            note.ChangeState(new Fader_JudgmentEnded());
        }
    }
}

// 判定结束：节点类型，保留物体直到Fader移动到下一个节点
public class Fader_JudgmentEnded_Move : INoteState
{
    public void EnterState(Note note)
    {
        // 设置为Fader上方的定位点
        note.transform.parent = NoteManager.Instance.faderAnchorPoint;
        note.transform.localPosition = Vector3.zero;

        // 计算Fader移动时间
        moveTime = note.Event_Next.time - note.Event.time;

        startCoordinate = NoteManager.Instance.CalculateNotePosition(note.Event.laneId);
        targetCoordinate = NoteManager.Instance.CalculateNotePosition(note.Event_Next.laneId);
    }

    public void ExitState(Note note)
    {
        // 这里只是类似于播放动画，已判定过，接下来就可以直接删除了
    }


    public void UpdateState(Note note)
    {
        // ————————————————————————————————————修改Fader位置————————————————————————————————————
        // 若为Auto模式。如果当前时间小于移动时间，执行移动逻辑
        if (note.IsAUTO && (currentTime < moveTime))
        {
            MoveNote();
            currentTime += Time.deltaTime;
        }

        // ————————————————————————————————————设置混音————————————————————————————————————
        float value = Mathf.Abs(NoteManager.Instance.faderTrackPosition - (note.Event_Next.laneId + 0.5f));

        if (value <= 0.5f)
        {
            // 轨道误差小于0.5时，混音为最大值
            InGameSoundManager.Instance.Mixing_PlayerUpload_Music(note.Event.effectType, note.Event.effectParameter);
        }
        else if (3 <= value)
        {
            // 轨道误差大于3时，混音为最小值，最小值为原始值的0.2倍
            InGameSoundManager.Instance.Mixing_PlayerUpload_Music(note.Event.effectType,
                note.Event.effectParameter * 0.2f);
        }
        else
        {
            // 轨道误差在0.5-3时，混音强度将根据距离动态减小
            // 将距离映射到[0, 1]范围内
            float t = (value - 0.5f) / 2.5f;
            // 线性插值。线性插值会根据一个介于0和1之间的插值参数t来计算出中间值c
            float mixedParameter = Mathf.Lerp(note.Event.effectParameter, note.Event.effectParameter * 0.2f, t);

            InGameSoundManager.Instance.Mixing_PlayerUpload_Music(note.Event.effectType,
                mixedParameter);
        }

        // —————————————————————————————————到下一个节点后移除—————————————————————————————————
        if (note.Event_Next.time - GameStateManager.Instance.GameTime <= 0)
        {
            InGameSoundManager.Instance.ResetAll_MixingEffects();
            note.ChangeState(new Fader_JudgmentEnded());
        }
    }

    float startCoordinate;  // 起始x坐标
    float targetCoordinate; // 目标x坐标
    float moveTime;         // 移动时间（秒）

    float currentTime = 0;  // 当前时间（秒）
    void MoveNote()
    {
        // 计算当前位置的百分比
        float t = currentTime / moveTime;

        // 使用Lerp函数计算新的x坐标
        float newX = Mathf.Lerp(startCoordinate, targetCoordinate, t);

        // 更新物体的位置
        NoteManager.Instance.fader.localPosition = new Vector3(newX,
            NoteManager.Instance.fader.localPosition.y,
            NoteManager.Instance.fader.localPosition.z);
    }
}

// 判定彻底结束
public class Fader_JudgmentEnded : INoteState
{
    public void EnterState(Note note)
    {
        if (note.Event.nextId == 0 && note.Event_Next == null)
        {
            // 这个if只有是 1.这是Fader最后一个节点 2.这是滑动的Fader 才会进入到这里
            NoteManager.Instance.isFaderIn = false;
        }

        NoteManager.Instance.judgments.Remove(note);
        NoteManager.Instance.allFaders.Remove(note);
        GameObject.Destroy(note.gameObject);
    }

    public void ExitState(Note note)
    {
    }

    public void UpdateState(Note note)
    {

    }
}
