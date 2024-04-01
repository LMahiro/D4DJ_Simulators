using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    private static NoteManager instance;
    public static NoteManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }

    public List<Note> allNotes = new List<Note>();
    public List<Note> judgments = new List<Note>();

    public List<Note> leftScratchs = new List<Note>();  // DJ轨道上的Scratchs分左右轨道
    public List<Note> rightScratchs = new List<Note>();

    public List<Note> allFaders = new List<Note>();     // 为Fader专门开集合，此值每帧从judgments遍历获取

    // 排序
    public void SortNotesByGenerationTime()
    {
        // 清空allNotes的null物体
        for (int i = 0; i < allNotes.Count; i++)
        {
            if (allNotes[i] == null)
                allNotes.RemoveAt(i);
        }

        // CompareTo用于比较当前对象与另一个对象的大小关系，返回一个整数。负数表示小于传入值，0表示相等，1表示大于
        allNotes.Sort((note1, note2) => note1.Event.time.CompareTo(note2.Event.time));
        judgments.Sort((note1, note2) => note1.Event.time.CompareTo(note2.Event.time));

        leftScratchs.Sort((note1, note2) => note1.Event.time.CompareTo(note2.Event.time));
        rightScratchs.Sort((note1, note2) => note1.Event.time.CompareTo(note2.Event.time));
    }

    /// <summary>
    /// 计算Note父物体下音符的中心点位置
    /// </summary>
    /// <param name="lane">所在轨道(int)</param>
    /// <param name="isLong">是否为长条音符，长条音符会缩放100倍。长条音符父物体x坐标应为0</param>
    /// <returns>计算后的轨道音符x坐标</returns>
    public float CalculateNotePosition(int lane, bool isLong = false)
    {
        if (isLong)
            return -(-0.06301f + lane * 0.02103f);  // 除了缩放100倍，还有旋转180度
        else
            return -6.301f + lane * 2.103f;
    }



    public Transform fader;         // Fader物体
    public SpriteRenderer[] faderMovingSprite;    // Fader移动时显示的Sprite
    public Transform faderAnchorPoint;            // Fader已判定后音符跟随推子移动的定位点

    public bool isFaderIn;          // 当前是否处在Fader判定区间
    public bool isFaderMove;        // Fader是否在移动
    public float fadeSpeed = 10;    // 渐变速度
    public float faderTrackPosition;              // 计算后的Fader的轨道位置

    public LeanFinger faderFinger;  // 给Fader的手指

    float faderLeftMax;     // Fader最大可移动的范围区间
    float faderRightMax;
    float faderCenter;      // Fader轨道中心点

    public int nextFaderLane;               // 下一个Fader的轨道
    float nextFaderPosition;                // 下一个Fader的位置。单独用一个值浮点数误差会导致值匹配不上。
    void Start()
    {
        faderLeftMax = CalculateNotePosition(1) * GameSettingsMannger.save_Settings.trackWidth;
        faderRightMax = CalculateNotePosition(5) * GameSettingsMannger.save_Settings.trackWidth;
        faderCenter = CalculateNotePosition(3) * GameSettingsMannger.save_Settings.trackWidth;
    }
    void Update()
    {
        GetAllFaders();

        // 计算Fader的轨道位置
        faderTrackPosition = TouchManager.Instance.CalculateTrackPosition(fader.position.x);

        nextFaderLane = NextFaderPosition();
        if (nextFaderLane != -10)
            nextFaderPosition = CalculateNotePosition(nextFaderLane) * GameSettingsMannger.save_Settings.trackWidth;

        if (isFaderMove || isFaderIn)
        {
            FadeIn();
            // 已被给予手指。Auto模式下手指按下不参与移动
            if (faderFinger != null)
            {
                fader.localPosition = new Vector3(SetFaderPosition(faderFinger), fader.localPosition.y, fader.localPosition.z);
                isFaderMove = true;
                // 手指抬起
                if (faderFinger.TouchStage == 4)
                {
                    isFaderMove = false;
                    faderFinger = null;
                }
            }
            // 没有手指按住且不在判定区间内，取消显示
            if (isFaderIn == false && faderFinger == null)
            {
                isFaderMove = false;
            }
        }
        else
        {
            FadeOut();
            FixedFaderPosition();
        }
    }
    void FadeIn()
    {
        foreach (SpriteRenderer spriteRenderer in faderMovingSprite)
        {
            float alpha = Mathf.Lerp(spriteRenderer.color.a, 1f, fadeSpeed * Time.deltaTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        }
    }   // Fader按下特效渐入
    void FadeOut()
    {
        foreach (SpriteRenderer spriteRenderer in faderMovingSprite)
        {
            float alpha = Mathf.Lerp(spriteRenderer.color.a, 0f, fadeSpeed * Time.deltaTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        }
    }   // Fader按下特效渐出

    void GetAllFaders()
    {
        allFaders.Clear();

        foreach (var note in judgments)
        {
            if (note.Event.type == NoteType.Slide)
                allFaders.Add(note);
        }

        allFaders.Sort((note1, note2) => note1.Event.time.CompareTo(note2.Event.time));
    }   // 每帧从judgments遍历获取Fader音符

    float SetFaderPosition(LeanFinger finger)
    {
        // 获取点击屏幕的坐标
        Vector2 screenPosition = finger.ScreenPosition;
        // 将屏幕坐标转换为世界坐标
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 9.35f));
        // 计算轨道位置
        if (worldPosition.x < faderLeftMax)
        {
            // 超出最左侧限制
            return faderLeftMax;
        }
        else if (worldPosition.x > faderRightMax)
        {
            // 超出最右侧限制
            return faderRightMax;
        }
        else
        {
            return worldPosition.x;
        }
    }   // 计算Fader移动时的位置
    void FixedFaderPosition()
    {
        if (nextFaderLane != -10)
        {
            // 找到了下一个音符，强制对齐
            float currentX = Mathf.Lerp(fader.localPosition.x, nextFaderPosition, fadeSpeed * Time.deltaTime);
            fader.localPosition = new Vector3(currentX, fader.localPosition.y, fader.localPosition.z);
        }
        else
        {
            // 找不到，强制居中对齐
            float currentX = Mathf.Lerp(fader.localPosition.x, faderCenter, fadeSpeed * Time.deltaTime);
            fader.localPosition = new Vector3(currentX, fader.localPosition.y, fader.localPosition.z);
        }
    }   // 在未判定以及不在判定区间时，固定Fader位置
    int NextFaderPosition()
    {
        NoteManager.Instance.SortNotesByGenerationTime();
        foreach (var note in NoteManager.Instance.allNotes)
        {
            if (note.Event.type == NoteType.Slide && note.Event.lastId == 0)
                return note.Event.laneId;
        }

        // 找不到
        return -10;

    }   // 计算下一个断开后的Fader的位置

    // 自动模式修改Fader的位置在Note_Fader脚本中进行
}