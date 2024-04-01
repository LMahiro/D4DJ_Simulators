using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 按下轨道线：竖直
public class PressingTrackLine_Vertical : MonoBehaviour, IPointerClickHandler
{
    [Header("当前脚本所在的轨道")]
    public TrackLine trackLine;
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (trackLine)
        {
            case TrackLine.zero:
                EditChart_Base.Instance.noteTrack = 0;
                break;
            case TrackLine.one:
                EditChart_Base.Instance.noteTrack = 1;
                break;
            case TrackLine.two:
                EditChart_Base.Instance.noteTrack = 2;
                break;
            case TrackLine.three:
                EditChart_Base.Instance.noteTrack = 3;
                break;
            case TrackLine.four:
                EditChart_Base.Instance.noteTrack = 4;
                break;
            case TrackLine.five:
                EditChart_Base.Instance.noteTrack = 5;
                break;
            case TrackLine.six:
                EditChart_Base.Instance.noteTrack = 6;
                break;
        }
        EditChart_Base.Instance.noteTrack_Clicked = true;
    }
}
// 轨道
public enum TrackLine
{
    zero, one, two, three, four, five, six
}
