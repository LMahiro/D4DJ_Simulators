using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 启动时需要设置的项目，设置完成后自动删除自身对象
/// </summary>
public class GameLaunch_Set : MonoBehaviour
{
    // Debug面板
    public GameObject IngameDebugConsole;

    void Start()
    {
        switch (GameSettingsMannger.save_Settings.generalInterfaceFramerate)
        {
            case Framerate._30:
                Application.targetFrameRate = 30;
                break;
            case Framerate._60:
                Application.targetFrameRate = 60;
                break;
            case Framerate._120:
                Application.targetFrameRate = 120;
                break;
            default:
                Application.targetFrameRate = 60;
                break;
        }
        switch (GameSettingsMannger.save_Settings.generalInterfaceQuality)
        {
            case QualityLevel.High:
                QualitySettings.SetQualityLevel(5, true);
                break;
            case QualityLevel.Medium:
                QualitySettings.SetQualityLevel(2, true);
                break;
            case QualityLevel.Low:
                QualitySettings.SetQualityLevel(0, true);
                break;
            default:
                QualitySettings.SetQualityLevel(5, true);
                break;
        }

        if (GameSettingsMannger.save_Settings.debugMode == true)
            Instantiate(IngameDebugConsole);

        Debug.Log(Application.persistentDataPath);

        Destroy(this.gameObject);
    }
}
