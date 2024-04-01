using UnityEngine;
using Lean.Touch;
using TMPro;
using System.Collections.Generic;

// 脚本执行顺序为-99
public class TouchManager : MonoBehaviour
{
    private static TouchManager instance;
    public static TouchManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        // 根据宽度缩放、判定线高度设置缩放值
        // **判定线高度由于摄像机视差计算只是投机取巧，测试了为1的值，必定不准确，应有一个函数但暂未发现**
        for (int i = 0; i < trackPositions.Length; i++)
        {
            trackPositions[i] *= GameSettingsMannger.save_Settings.trackWidth;

            if (0 < GameSettingsMannger.save_Settings.judgementLineHeight)
                trackPositions[i] += GameSettingsMannger.save_Settings.judgementLineHeight / 10 * 0.08f;
            else if (GameSettingsMannger.save_Settings.judgementLineHeight < 0)
                trackPositions[i] -= GameSettingsMannger.save_Settings.judgementLineHeight / 10 * (-0.1f);
        }
    }

    // 轨道线的世界坐标
    public float[] trackPositions = { -6.51f, -4.65f, -2.79f, -0.93f, 0.93f, 2.79f, 4.65f, 6.51f };

    // 手指计算轨道位置，同时判断是否满足Fader的位置要求
    float FingerCalculateTrackPosition(LeanFinger finger)
    {
        // 获取点击屏幕的坐标
        Vector2 screenPosition = finger.ScreenPosition;
        // 将屏幕坐标转换为世界坐标
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 9.35f));

        // 判断判定线是否满足Fader位置要求。
        // **判定线高度由于摄像机视差计算只是投机取巧，必定不准确，应有一个函数但暂未发现**
        if (worldPosition.y < TrackManager.Instance.anchorPoint.position.y)
            finger.IsFaderTriggered = true;
        else
            finger.IsFaderTriggered = false;
        //Debug.Log($"worldPosition.y {worldPosition.y}  anchorPoint.y {TrackManager.Instance.anchorPoint.position.y}");

        return CalculateTrackPosition(worldPosition.x);
    }
    /// <summary>
    /// 通过世界位置计算轨道位置
    /// </summary>
    /// <param name="position">世界位置(x)</param>
    /// <returns></returns>
    public float CalculateTrackPosition(float position)
    {
        // 计算轨道位置
        if (position < trackPositions[0])
        {
            // 超出最左侧线
            return 0;
        }
        else if (position > trackPositions[trackPositions.Length - 1])
        {
            // 超出最右侧线
            return 7;
        }
        else
        {
            for (int i = 0; i < trackPositions.Length - 1; i++)
            {
                if (position >= trackPositions[i] && position < trackPositions[i + 1])
                {
                    // 使用线性插值计算轨道位置
                    float t = Mathf.InverseLerp(trackPositions[i], trackPositions[i + 1], position);
                    return Mathf.Lerp(i, i + 1, t);
                }
            }
        }

        // 默认返回值
        return 0f;
    }
    /// <summary>
    /// 通过轨道位置返回世界坐标
    /// </summary>
    /// <param name="trackPosition">轨道位置</param>
    /// <returns>世界坐标x坐标轴的值</returns>
    public float ReturnToWorldCoordinates(int trackPosition)
    {
        if (trackPosition == 0)
            return (trackPositions[0] + trackPositions[1]) / 2;
        else if (trackPosition == 7)
            return (trackPositions[6] + trackPositions[7]) / 2;
        else
        {
            return (trackPositions[trackPosition] + trackPositions[trackPosition + 1]) / 2;
            // 使用线性插值计算世界坐标，传入float
            //int index = Mathf.FloorToInt(trackPosition);
            //float t = trackPosition - index;
            //return Mathf.Lerp(trackPositions[index], trackPositions[index + 1], t);
        }
    }


    public TextMeshProUGUI debugText;

    void Update()
    {
        // 手指状态设置
        FingerStateSettings();

        // 二次遍历寻找可判定音符
        SearchingForDecidableNotes();

        // Scratch的专属判定方法
        ScratchJudgment();

        // 将抬起的手指取消已判定状态
        foreach (var finger in LeanTouch.Fingers)
        {
            if (finger.TouchStage == 4)
            {
                // 移除已判定标识
                finger.ParticipatedInJudgment = false;

                // 移除Scratch专用标识
                finger.IsScratchExclusive = false;
            }
        }
    }
    void FingerStateSettings()
    {
        foreach (var finger in LeanTouch.Fingers)
        {
            finger.TrackPosition = FingerCalculateTrackPosition(finger);

            if (finger.Down)
            {
                // 按下时，设置TouchStage为1
                finger.TouchStage = 1;
            }
            else if (finger.Set)
            {
                // 在一直按住时，设置TouchStage为2
                finger.TouchStage = 2;
                finger.IsScratchTriggered = false;

                // 检查是否有位移
                if (finger.ScreenDelta.magnitude > 0)
                {
                    // 如果有位移，设置TouchStage为3
                    finger.TouchStage = 3;

                    // 若位移满足长度，则设置IsScratchTriggered满足条件，该值只会在DJ轨道上使用
                    // 注意：这里暂时没有方向判定代码，也就是说只要滑动距离够了就判定
                    if (finger.ScreenDelta.magnitude > 100 * (1.05f - GameSettingsMannger.save_Settings.djTrackSlideSensitivity))
                    {
                        finger.IsScratchTriggered = true;
                    }
                }
            }
            else if (finger.Up)
            {
                // 松开时，设置TouchStage为4
                finger.TouchStage = 4;
            }
        }
    }   // 手指状态设置
    void SearchingForDecidableNotes()
    {
        // 先排序
        NoteManager.Instance.SortNotesByGenerationTime();
        foreach (var finger in LeanTouch.Fingers)
        {
            if (finger.ParticipatedInJudgment == true) continue;

            // 通过套方法解决双Foreach无法跳出内部循环的问题
            GiveFingers(finger);
        }
    }   // 二次遍历寻找可判定音符
    void GiveFingers(LeanFinger finger)
    {
        // 是否判定成功的bool值
        bool isGiveSuccess = false;

        // 排序后最靠前的音符，使用Foreach避免0出错
        foreach (var note in NoteManager.Instance.judgments)
        {
            isGiveSuccess = true;

            // DJ轨道没有临轨判定
            if (finger.TrackPosition <= 1)
            {
                // 左侧DJ道Stop音符，只有Stop能进到这里。Scratch添加在NoteManager.Instance.scratchs
                if (note.Event.laneId == 0)
                {
                    finger.ParticipatedInJudgment = true;
                    note.Finger = finger;
                    return;
                }
            }
            else if (finger.TrackPosition <= 6)
            {
                // 中间五轨道
                // 根据设置的判定模式以不同的方式给予手指
                // 分离模式
                if (GameSettingsMannger.save_Settings.judgementMode == JudgementMode.Separate)
                {
                    // 点击的是底部，直接给予Fader的控制权
                    if (finger.IsFaderTriggered == true)
                    {
                        finger.ParticipatedInJudgment = true;
                        NoteManager.Instance.faderFinger = finger;
                        NoteManager.Instance.isFaderMove = true;
                        return;
                    }
                    else
                    {
                        // 尝试寻找到与点击位置离得最近、且时间较早的音符，【不考虑Slide音符】
                        Note bestNote = null;
                        float bestDistance = 999;
                        float bestTime = 999;

                        for (int i = 0; i < NoteManager.Instance.judgments.Count; i++)
                        {
                            if (NoteManager.Instance.judgments[i].Event.type == NoteType.Slide)
                                continue;

                            float tempDistance = NoteManager.Instance.judgments[i].Event.laneId;
                            float tempTime = NoteManager.Instance.judgments[i].Event.time;

                            // 优先比较时间 => 音符距离 => 最终确定
                            if (tempTime - GameStateManager.Instance.GameTime < bestTime)
                            {
                                bestTime = tempTime;
                                if (tempDistance - finger.TrackPosition < bestDistance)
                                {
                                    bestDistance = tempDistance;
                                    bestNote = NoteManager.Instance.judgments[i];
                                }
                            }
                        }

                        if (bestNote != null)
                        {
                            if (bestNote.Event.laneId - 0.5f < finger.TrackPosition &&
                            finger.TrackPosition < bestNote.Event.laneId + 1.5f)
                            {
                                // 尝试将点击位置临轨判定，否则判定失败
                                finger.ParticipatedInJudgment = true;
                                bestNote.Finger = finger;
                                return;
                            }
                        }
                    }

                }
                else
                {
                    // 混合模式
                    // 尝试寻找到与点击位置离得最近、且时间较早的音符
                    Note bestNote = null;
                    float bestDistance = 999;
                    float bestTime = 999;

                    for (int i = 0; i < NoteManager.Instance.judgments.Count; i++)
                    {
                        float tempDistance = NoteManager.Instance.judgments[i].Event.laneId;
                        float tempTime = NoteManager.Instance.judgments[i].Event.time;

                        // 优先比较时间 => 音符距离 => 最终确定。不考虑滑动Slide占轨道的大小
                        if (tempTime - GameStateManager.Instance.GameTime < bestTime)
                        {
                            bestTime = tempTime;
                            if (tempDistance - finger.TrackPosition < bestDistance)
                            {
                                bestDistance = tempDistance;
                                bestNote = NoteManager.Instance.judgments[i];
                            }
                        }
                    }

                    if (bestNote != null)
                    {
                        // 获取Fader的轨道位置
                        float faderTrackPosition = NoteManager.Instance.faderTrackPosition;


                        if (bestNote.Event.type == NoteType.Slide && 0 < bestNote.Event.direction)
                        {
                            // 若滑键Slide为最优的音符
                            finger.ParticipatedInJudgment = true;
                            NoteManager.Instance.faderFinger = finger;
                            NoteManager.Instance.isFaderMove = true;
                            return;
                        }
                        else if (bestNote.Event.type == NoteType.Slide &&
                            NoteManager.Instance.isFaderIn &&
                            (faderTrackPosition < NoteManager.Instance.nextFaderLane - 0.5f || NoteManager.Instance.nextFaderLane + 1.5f < faderTrackPosition))
                        {
                            // 若节点Slide为最优的音符，并且处在区间，而且Fader还【不在】正确的判定位置上时
                            finger.ParticipatedInJudgment = true;
                            NoteManager.Instance.faderFinger = finger;
                            NoteManager.Instance.isFaderMove = true;
                            return;
                        }
                        else if (bestNote.Event.type != NoteType.Slide &&
                            bestNote.Event.laneId - 0.5f < finger.TrackPosition &&
                            finger.TrackPosition < bestNote.Event.laneId + 1.5f)
                        {
                            // 若优先音符不是Slide类型，尝试将点击位置临轨判定
                            finger.ParticipatedInJudgment = true;
                            bestNote.Finger = finger;
                            return;
                        }

                        // 能到这里说明Slide已在位 或点击的位置可能没有音符
                        // 也有可能同时存在普通音符和Slide音符。最优音符给了普通音符，但是普通音符没有判定成功，接下来需要尝试给Slide音符手指
                        // 还有可能是等待判定序列只有一个音符。可能是只有一个普通音符(不再判定区间)或Slide音符(单独存在)。单纯只存在【在区间内的】Slide则会进入到判定失败代码再给予手指

                        // 再次尝试遍历查找新值

                        Note alternateNote = null;
                        bestDistance = 999;
                        bestTime = 999;

                        for (int i = 0; i < NoteManager.Instance.judgments.Count; i++)
                        {
                            // 若本次遍历依然为先前遍历的最佳音符 则跳过本次循环
                            if (NoteManager.Instance.judgments[i] == bestNote)
                                continue;

                            float tempDistance = NoteManager.Instance.judgments[i].Event.laneId;
                            float tempTime = NoteManager.Instance.judgments[i].Event.time;

                            // 优先比较时间 => 音符距离 => 最终确定。不考虑滑动Slide占轨道的大小
                            if (tempTime - GameStateManager.Instance.GameTime < bestTime)
                            {
                                bestTime = tempTime;
                                if (tempDistance - finger.TrackPosition < bestDistance)
                                {
                                    bestDistance = tempDistance;
                                    alternateNote = NoteManager.Instance.judgments[i];
                                }
                            }
                        }

                        if (alternateNote != null)
                        {
                            if (alternateNote.Event.type != NoteType.Slide &&
                                alternateNote.Event.laneId - 0.5f < finger.TrackPosition &&
                                finger.TrackPosition < alternateNote.Event.laneId + 1.5f)
                            {
                                // 尝试将点击位置临轨判定
                                finger.ParticipatedInJudgment = true;
                                alternateNote.Finger = finger;
                                return;
                            }
                            else if (alternateNote.Event.type == NoteType.Slide && 0 < alternateNote.Event.direction)
                            {
                                // 若滑键Slide为最优的音符
                                finger.ParticipatedInJudgment = true;
                                NoteManager.Instance.faderFinger = finger;
                                NoteManager.Instance.isFaderMove = true;
                                return;
                            }
                            else if (NoteManager.Instance.isFaderIn)
                            {
                                // 都不成功的情况下，若Slide在区间，则将控制权给予Slide
                                finger.ParticipatedInJudgment = true;
                                NoteManager.Instance.faderFinger = finger;
                                NoteManager.Instance.isFaderMove = true;
                                return;
                            }
                        }
                        // 二次寻找依然无音符
                    }
                    // 初次寻找就没有找到音符
                }
            }
            else
            {
                // 右侧DJ道，只有Stop能进到这里。Scratch添加在NoteManager.Instance.scratchs
                if (note.Event.laneId == 6)
                {
                    finger.ParticipatedInJudgment = true;
                    note.Finger = finger;
                    return;
                }
            }
        }

        if (isGiveSuccess == false)
        {
            // 没有判定成功
            // 手指依然设置为已判定
            finger.ParticipatedInJudgment = true;

            // 若当前未判定手指点击到了DJ轨道上，则增加标识
            if (finger.TrackPosition <= 1)
            {
                // 左侧DJ道
                finger.IsScratchExclusive = true;
                finger.LeftOrRightScratch = false;
                return;
            }
            else if (6 < finger.TrackPosition)
            {
                // 右侧DJ道
                finger.IsScratchExclusive = true;
                finger.LeftOrRightScratch = true;
                return;
            }

            if (1 < finger.TrackPosition && finger.TrackPosition <= 6)
            {
                // 中间五轨道判定失败
                // 若当前处在Fader区间，始终将控制权给予滑条（必须设置为混合判定）
                if (NoteManager.Instance.isFaderIn)
                {
                    if (GameSettingsMannger.save_Settings.judgementMode == JudgementMode.Blend)
                    {
                        // 移动Fader
                        NoteManager.Instance.faderFinger = finger;
                        NoteManager.Instance.isFaderMove = true;

                        return; // 给予后直接结束
                    }
                }

                // 此时不在Fader区间或判定方式为分离，根据位置判断触控行为
                if (finger.IsFaderTriggered)
                {
                    // 移动Fader
                    NoteManager.Instance.faderFinger = finger;
                    NoteManager.Instance.isFaderMove = true;
                }
                else
                {
                    // 播放主轨道未命中动画以及音效...
                    InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Tap_Empty);
                }
            }
        }
    }   // 给予手指。通过套方法解决双Foreach无法跳出内层循环的问题
    void ScratchJudgment()
    {
        foreach (var finger in LeanTouch.Fingers)
        {
            if (finger.IsScratchExclusive == false)
                continue;

            if (finger.IsScratchTriggered)
            {
                if (finger.LeftOrRightScratch == false)
                {
                    // 左侧
                    // 已在SearchingForDecidableNotes()中排序过，可以直接用
                    if (NoteManager.Instance.leftScratchs.Count != 0)
                    {
                        // 判定成功，设置音符脚本的值
                        NoteManager.Instance.leftScratchs[0].IsScratchSuccessful = true;
                        finger.IsScratchTriggered = false;
                    }
                    else
                    {
                        // 判定失败，播放动画以及音效...
                        InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_TapEmpty);
                    }
                }
                else
                {
                    // 右侧
                    if (NoteManager.Instance.rightScratchs.Count != 0)
                    {
                        // 判定成功，设置音符脚本的值
                        NoteManager.Instance.rightScratchs[0].IsScratchSuccessful = true;
                        finger.IsScratchTriggered = false;
                    }
                    else
                    {
                        // 判定失败，播放动画以及音效...
                        InGameSoundManager.Instance.PlaySe_Note(InGameSe_Note.Scratch_TapEmpty);
                    }
                }
            }
        }
    }   // Scratch专门的判定方法
}