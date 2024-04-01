using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chart
{
    public BasicInformation_OfChart information;    // 谱面基本信息
    public List<float> barLineList;                 // 小节线出现时间
    public List<Event> noteDataList;                // 谱面数据
    public List<ChartOperate> soflanDataList;       // 保存谱面的时停效果
    public List<EditInformation> editInformation;   // 编辑谱面界面存储的信息

    public Chart()
    {
        barLineList = new List<float>();
        // 由于该类包含了其它类的引用，为了防止new()后为null，增加构造函数
        information = new BasicInformation_OfChart();
        noteDataList = new List<Event>();
        soflanDataList = new List<ChartOperate>();
        editInformation = new List<EditInformation>();
    }
}

#region Event
public class Event
{
    public Event() { }
    public Event(NoteType type, int laneId, float time, int nextId, int lastId, int direction, Note_EffectType effectType, float effectParameter)
    {
        this.type = type; this.laneId = laneId; this.time = time; this.nextId = nextId; this.lastId = lastId; this.direction = direction; this.effectType = effectType; this.effectParameter = effectParameter;
    }

    public NoteType type;   // 音符类型
    public int laneId;      // 所在轨道，0-6
    public float time;      // 具体时间，单位为秒。

    public int nextId;      // 音符结束后连接到的下一个音符的List索引；如果为0，则表示它结束后没有下一个音符
    public int lastId;      // 上一个音符的索引；若为0，则表示其为第一个音符

    public int direction;   // Slide的滑动方向；0为没有方向；+2表示向右滑，-2表示向左滑
    public Note_EffectType effectType;  // Slide位移时的混音器名称
    public float effectParameter;  // 最大时的混音器强度
}

public enum NoteType
{
    Tap1, Tap2,
    Long_Start, Long_End,
    Slide,
    Scratch_Left, Scratch_Right,
    Stop_Start, Stop_End
}
public enum Note_EffectType
{
    Null,
    MultbandEQFreq,
    ReverbTime,
    ThreeEQLow,
    ThreeEQMid,
    ThreeEQHigh,
    Chorus,
    Delay,
    FlangerMix,
    Gain,
    TremoloFrequency,
}
#endregion

#region ChartOperate
public class ChartOperate
{
    public float time;          // 操作时间
    public float timeScale;     // 主轨道时间倍速；1.0表示正常速度，-1.0表示倒放，0.0表示主谱面暂停
}
#endregion

#region ChartInformation
public class BasicInformation_OfChart
{
    public string songName;         // 歌曲名称
    public string songWriter;       // 歌曲作者
    public string chartWriter;      // 谱面作者

    public Chart_Difficulty chartDifficulty;
    public int chartLevel;          // 等级
}
public enum Chart_Difficulty
{
    EASY,
    NORMAL,
    HARD,
    EXPERT,
}
#endregion

#region EditInformation
public class EditInformation
{
    public EditInformation() { }
    public EditInformation(float time, float bpm)
    {
        this.time = time; this.bpm = bpm;
    }

    public float time;    // 放置时间
    public float bpm;     // 切换的BPM值
}
#endregion