using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class InGameDebug : MonoBehaviour
{
    private static InGameDebug instance;
    public static InGameDebug Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this.gameObject);

        baseOBJ.SetActive(false);
        isShow = false;
    }

    public GameObject baseOBJ;
    public TextMeshProUGUI time;
    public TextMeshProUGUI input;

    void Start()
    {
        if (!GameSettingsMannger.save_Settings.debugMode)
            this.gameObject.SetActive(false);

        InGameSoundManager.Instance.Get_PlayerUpload_Music(Path.Combine(Application.persistentDataPath, "music.wav"));
        InGameSoundManager.Instance.Play_PlayerUpload_Music();

        int time = (int)(GameStateManager.Instance.GameTime + GameSettingsMannger.save_Settings.musicLatencyAdjustment / 10);
        InGameSoundManager.Instance.playerUploadInstance.setTimelinePosition(time);
    }

    void Update()
    {
        time.text = GameStateManager.Instance.GameTime.ToString();

        input.text = "";
        foreach (var finger in LeanTouch.Fingers)
        {
            input.text += $"Index：{finger.Index} TrackPosition：{finger.TrackPosition} TouchStage：{finger.TouchStage}" +
                $"\nReturnToWorldCoordinates：{TouchManager.Instance.ReturnToWorldCoordinates(Mathf.FloorToInt(finger.TrackPosition))}" +
                $"\nIsFaderTriggered：{finger.IsFaderTriggered}" +
                $"\nIsScratchTriggered：{finger.IsScratchTriggered}" +
                $"\nIsScratchExclusive：{finger.IsScratchExclusive}" +
                $"\nLeftOrRightScratch：{finger.LeftOrRightScratch}" +
                $"\n";
        }
    }


    bool isShow;
    public void _Button()
    {
        if(isShow)
        {
            baseOBJ.SetActive(false);
            isShow = false;
        }
        else
        {
            baseOBJ.SetActive(true);
            isShow = true;
        }
    }


}
