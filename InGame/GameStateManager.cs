using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    public static GameStateManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        gamePaused = true;

        // 计算隐藏音符的y轴值
        noteHiddenValue = minValueOfHiddenNotes + GameSettingsMannger.save_Settings.trackLength * (maxValueOfHiddenNotes - minValueOfHiddenNotes);
    }
    void Start()
    {
        GivenChart(PassByValue.Instance.chart);
        gamePaused = false;
    }

    // 游戏时间
    public float GameTime { get; private set; }
    void Update()
    {
        if (gameOver)
            return;

        if (!gamePaused)
            GameTime += Time.deltaTime;

        // 超出总长度，进行结算
        if (InGameSoundManager.Instance.playerUploadAudioLength != 0)
            if (InGameSoundManager.Instance.playerUploadAudioLength < GameStateManager.Instance.GameTime * 1000)
                GameOver();
    }

    // 游戏暂停、恢复
    public bool gamePaused { get; private set; }
    public event ToDo gamePausedToDo;
    public event ToDo gameRecoveryToDo;
    public void SetGamePause(bool isPause)
    {
        if (isPause)
        {
            if (gamePausedToDo != null)
                gamePausedToDo();
            gamePaused = true;
        }
        else
        {
            if (gameRecoveryToDo != null)
                gameRecoveryToDo();
            gamePaused = false;
        }
    }

    // 游戏已结束
    public bool gameOver { get; private set; }
    public GameObject[] gameOverDestoryObjects; // 游戏结束需要销毁的物体
    void GameOver()
    {
        InGameSoundManager.Instance.Pause_PlayerUpload_Music();
        gamePaused = true;
        gameOver = true;
        foreach (var obj in gameOverDestoryObjects)
        {
            Destroy(obj);
        }
        Canvas_Result.Instance.Show();
    }


    // 音符隐藏的世界y轴坐标
    public float maxValueOfHiddenNotes = 86.40296f;
    public float minValueOfHiddenNotes = -2.833013f;
    [HideInInspector]
    public float noteHiddenValue;


    // 得分基准（1 Perfect的得分）
    public float scoringBenchmark;


    // 是否为自动播放
    public bool isAutoPlay;

    // 音符预制体
    // [0]Tap1  [1]Tap2  [2]Hold  [3]Slide  [4]Scratch  [5]Stop
    public GameObject[] prefabricatedNotes = new GameObject[6];
    // 小节线
    public GameObject barLine;
    // 游戏谱面
    public Chart chart;
    /// <summary>
    /// 在外部给予游戏谱面，并进行游戏前初始化
    /// </summary>
    /// <param name="chart">游戏谱面</param>
    public void GivenChart(Chart chart)
    {
        this.chart = chart;

        // 计算得分基准（1 Perfect的得分）
        scoringBenchmark = 1000000 / chart.noteDataList.Count;

        // 把值给予过来，要不然太长了
        float fallingDistancePerSecond = NoteLaneManager.Instance.fallingDistancePerSecond;

        for (int i = 0; i < chart.noteDataList.Count; i++)
        {
            if (IsUsedIndexes(i))
                continue;

            switch (chart.noteDataList[i].type)
            {
                case NoteType.Tap1:
                    GameObject tap1Obj = Instantiate(prefabricatedNotes[0], NoteLaneManager.Instance.normalLine);
                    Note_Tap1 tap1 = tap1Obj.GetComponent<Note_Tap1>();
                    tap1.Event = chart.noteDataList[i];
                    tap1.IsAUTO = isAutoPlay;

                    tap1.transform.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(chart.noteDataList[i].laneId),
                            chart.noteDataList[i].time * fallingDistancePerSecond, 0);
                    break;
                case NoteType.Tap2:
                    GameObject tap2Obj = Instantiate(prefabricatedNotes[1], NoteLaneManager.Instance.normalLine);
                    Note_Tap2 tap2 = tap2Obj.GetComponent<Note_Tap2>();
                    tap2.Event = chart.noteDataList[i];
                    tap2.IsAUTO = isAutoPlay;

                    tap2.transform.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(chart.noteDataList[i].laneId),
                            chart.noteDataList[i].time * fallingDistancePerSecond, 0);
                    break;
                case NoteType.Long_Start:
                    GameObject holdObj = Instantiate(prefabricatedNotes[2], NoteLaneManager.Instance.normalLine);
                    Note_Long hold = holdObj.GetComponent<Note_Long>();
                    hold.Event = chart.noteDataList[i];
                    hold.Event_Next = chart.noteDataList[chart.noteDataList[i].nextId];
                    hold.IsAUTO = isAutoPlay;

                    // hold只有起始和终止两节点，将尾节点加入至已使用过的索引
                    usedIndexes.Add(chart.noteDataList[i].nextId);

                    // 父对象设置为原始高度，down与父对象高度相同，up计算差值设置高度，同时因为100缩放需要除以100
                    hold.transform.localPosition = new Vector3(0,
                            hold.Event.time * fallingDistancePerSecond, 0);
                    hold.down.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(hold.Event.laneId, true),
                            0, 0);
                    hold.up.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(hold.Event_Next.laneId, true),
                            (hold.Event_Next.time * fallingDistancePerSecond - hold.Event.time * fallingDistancePerSecond) / 100, 0);
                    break;
                case NoteType.Long_End:
                    // 正常情况下不应进入这里，能进入说明谱面排序错误
                    GameObject endHoldObj = Instantiate(prefabricatedNotes[2], NoteLaneManager.Instance.normalLine);
                    Note_Long endHold = endHoldObj.GetComponent<Note_Long>();
                    endHold.Event_Next = chart.noteDataList[i];
                    endHold.Event = chart.noteDataList[chart.noteDataList[i].lastId];
                    endHold.IsAUTO = isAutoPlay;

                    // hold只有起始和终止两节点，将尾节点加入至已使用过的索引
                    usedIndexes.Add(chart.noteDataList[i].lastId);

                    // 父对象设置为原始高度，down与父对象高度相同，up计算差值设置高度，同时因为100缩放需要除以100
                    endHold.transform.localPosition = new Vector3(0,
                            endHold.Event.time * fallingDistancePerSecond, 0);
                    endHold.down.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(endHold.Event.laneId, true),
                            0, 0);
                    endHold.up.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(endHold.Event_Next.laneId, true),
                            (endHold.Event_Next.time * fallingDistancePerSecond - endHold.Event.time * fallingDistancePerSecond) / 100, 0);
                    break;
                case NoteType.Slide:
                    // 单独一节点的滑键
                    if (chart.noteDataList[i].lastId == 0 && chart.noteDataList[i].nextId == 0 &&
                        chart.noteDataList[i].direction != 0)
                    {
                        GameObject slideFaderObj = Instantiate(prefabricatedNotes[3], NoteLaneManager.Instance.normalLine);
                        Note_Fader slideFader = slideFaderObj.GetComponent<Note_Fader>();
                        slideFader.Event = chart.noteDataList[i];
                        slideFader.IsAUTO = isAutoPlay;

                        slideFader.transform.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(slideFader.Event.laneId),
                            slideFader.Event.time * fallingDistancePerSecond, 0);
                        // 设置箭头宽度...
                    }
                    else
                    {
                        bool isContinueLoop = true; // 继续循环？
                        int index = i;              // 索引
                        Note_Fader nowSlide = null;        // 当前音符的脚本
                        Transform nextTransform = null;    // 下一音符的坐标，用于连接线定位点

                        while (isContinueLoop)
                        {
                            GameObject slideObj = Instantiate(prefabricatedNotes[3], NoteLaneManager.Instance.normalLine);
                            Note_Fader slide = slideObj.GetComponent<Note_Fader>();
                            slide.Event = chart.noteDataList[index];

                            if (slide.Event.nextId != 0)    // 若没有下一个元素则不赋值
                                slide.Event_Next = chart.noteDataList[chart.noteDataList[index].nextId];
                            slide.IsAUTO = isAutoPlay;

                            slide.transform.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(slide.Event.laneId),
                                slide.Event.time * fallingDistancePerSecond, 0);

                            nextTransform = slideObj.transform;

                            if (nowSlide != null)
                                if (nowSlide.Event.nextId == index)
                                    nowSlide.nextTransform = nextTransform;

                            // 比较完后，将当前音符脚本传出去备用
                            nowSlide = slide;

                            // 判断有没有下一个元素，有则设置索引，没有则跳出循环
                            if (chart.noteDataList[index].nextId == 0)
                                isContinueLoop = false;
                            else
                            {
                                // 设置索引
                                index = chart.noteDataList[index].nextId;
                                // 加入已使用过的索引
                                usedIndexes.Add(index);
                            }
                        }
                    }
                    break;
                case NoteType.Scratch_Left:
                    GameObject scratchLeftObj = Instantiate(prefabricatedNotes[4], NoteLaneManager.Instance.djLine);
                    Note_Scratch scratchLeft = scratchLeftObj.GetComponent<Note_Scratch>();
                    scratchLeft.Event = chart.noteDataList[i];
                    scratchLeft.IsAUTO = isAutoPlay;

                    scratchLeft.transform.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(chart.noteDataList[i].laneId),
                            chart.noteDataList[i].time * fallingDistancePerSecond, 0);
                    break;
                case NoteType.Scratch_Right:
                    GameObject scratchRightObj = Instantiate(prefabricatedNotes[4], NoteLaneManager.Instance.djLine);
                    Note_Scratch scratchRight = scratchRightObj.GetComponent<Note_Scratch>();
                    scratchRight.Event = chart.noteDataList[i];
                    scratchRight.IsAUTO = isAutoPlay;

                    scratchRight.transform.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(chart.noteDataList[i].laneId),
                            chart.noteDataList[i].time * fallingDistancePerSecond, 0);
                    break;
                case NoteType.Stop_Start:
                    GameObject stopObj = Instantiate(prefabricatedNotes[5], NoteLaneManager.Instance.normalLine);
                    Note_Stop stop = stopObj.GetComponent<Note_Stop>();
                    stop.Event = chart.noteDataList[i];
                    stop.Event_Next = chart.noteDataList[chart.noteDataList[i].nextId];
                    stop.IsAUTO = isAutoPlay;

                    // hold只有起始和终止两节点，将尾节点加入至已使用过的索引
                    usedIndexes.Add(chart.noteDataList[i].nextId);

                    // 父对象设置为原始高度，down与父对象高度相同，up计算差值设置高度，同时因为100缩放需要除以100
                    stop.transform.localPosition = new Vector3(0,
                            stop.Event.time * fallingDistancePerSecond, 0);
                    stop.down.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(stop.Event.laneId, true),
                            0, 0);
                    stop.up.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(stop.Event_Next.laneId, true),
                            (stop.Event_Next.time * fallingDistancePerSecond - stop.Event.time * fallingDistancePerSecond) / 100, 0);
                    break;
                case NoteType.Stop_End:
                    // 正常情况下不应进入这里，能进入说明谱面排序错误
                    GameObject endStopObj = Instantiate(prefabricatedNotes[2], NoteLaneManager.Instance.normalLine);
                    Note_Stop endStop = endStopObj.GetComponent<Note_Stop>();
                    endStop.Event_Next = chart.noteDataList[i];
                    endStop.Event = chart.noteDataList[chart.noteDataList[i].lastId];
                    endStop.IsAUTO = isAutoPlay;

                    // hold只有起始和终止两节点，将尾节点加入至已使用过的索引
                    usedIndexes.Add(chart.noteDataList[i].lastId);

                    // 父对象设置为原始高度，down与父对象高度相同，up计算差值设置高度，同时因为100缩放需要除以100
                    endStop.transform.localPosition = new Vector3(0,
                            endStop.Event.time * fallingDistancePerSecond, 0);
                    endStop.down.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(endStop.Event.laneId, true),
                            0, 0);
                    endStop.up.localPosition = new Vector3(NoteManager.Instance.CalculateNotePosition(endStop.Event_Next.laneId, true),
                            (endStop.Event_Next.time * fallingDistancePerSecond - endStop.Event.time * fallingDistancePerSecond) / 100, 0);
                    break;
            }
        }

        for (int i = 0; i < chart.barLineList.Count; i++)
        {
            if (GameSettingsMannger.save_Settings.showBarLines == false)
                break;

            GameObject lineOBJ = Instantiate(barLine, NoteLaneManager.Instance.normalLine);
            lineOBJ.transform.localPosition = new Vector3(0,
                    chart.barLineList[i] * fallingDistancePerSecond, 0);
        }
    }
    // 已使用过的索引（对于长条类音符）
    List<int> usedIndexes = new List<int>();
    bool IsUsedIndexes(int index)
    {
        for (int i = 0; i < usedIndexes.Count; i++)
        {
            if (index == usedIndexes[i])
                return true;
        }

        return false;
    }   // 判断当前的索引值是否已被使用
}

public delegate void ToDo();
