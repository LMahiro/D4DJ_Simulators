using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar_SeekBar : MonoBehaviour
{
    public Image full;
    public Image arrow;

    public float full_MaxWidth;
    public Vector2 arrow_Start;
    public Vector2 arrow_End;

    void Update()
    {
        Update_SeekBar();
    }

    int playTime_Now;
    void Update_SeekBar()
    {
        InGameSoundManager.Instance.playerUploadInstance.getTimelinePosition(out playTime_Now);

        if (!InGameSoundManager.Instance.playerUploadInstance_isPaused)
        {
            //int nowGameTime = (int)(GameStateManager.Instance.GameTime * 1000);
            //if (playTime_Now < nowGameTime)
            //    InGameSoundManager.Instance.playerUploadInstance.setTimelinePosition(nowGameTime);

            float value = playTime_Now / (float)InGameSoundManager.Instance.playerUploadAudioLength;

            full.rectTransform.sizeDelta = new Vector2(full_MaxWidth * value, full.rectTransform.sizeDelta.y);
            arrow.rectTransform.anchoredPosition = Vector2.Lerp(arrow_Start, arrow_End, value);
        }
    }
}
