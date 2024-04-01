using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通过Game_Settings_Mannger获取全局可访问的save_Settings对象，并提供保存、读取等数据持久化操作
/// </summary>
public class GameSettingsMannger
{
    //全局可访问的save_Settings对象
    public static GameSettings save_Settings = new GameSettings();

    //Game_Settings_Mannger管理类的单例模式
    private static GameSettingsMannger instance = new GameSettingsMannger();
    public static GameSettingsMannger Instance => instance;
    private GameSettingsMannger()
    {
        try { LoadSettings(); }
        catch { SaveSettings(); }
    }

    /// <summary>
    /// 全局可使用的保存save_Settings对象的方法
    /// </summary>
    public void SaveSettings()
    {
        JsonMannger.Instance.SaveJsonData_DefaultPath(save_Settings, "GameSettings");
    }
    /// <summary>
    /// 全局可使用的读取save_Settings对象的方法
    /// </summary>
    public void LoadSettings()
    {
        save_Settings = JsonMannger.Instance.LoadJsonData_DefaultPath<GameSettings>("GameSettings");
    }
}
