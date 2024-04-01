using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class ChartAction
{
    #region 单例模式
    private ChartAction() { }
    private static ChartAction action = new();
    public static ChartAction Action => action;
    #endregion

    /// <summary>
    /// 保存谱面数据到指定路径
    /// </summary>
    /// <param name="chart">Chart类对象</param>
    /// <param name="filePath">保存的路径（不需要"Application.persistentDataPath\"前缀）</param>
    /// <param name="name">文件名（无需后缀）</param>
    public void SaveChartToJson(Chart chart, string filePath, string name)
    {
        JsonMannger.Instance.SaveJsonData_ToPath(chart, filePath, name);
    }

    /// <summary>
    /// 从指定路径读取谱面文件数据
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public Chart LoadChartFromJson_ToPath(string filePath, string name)
    {
        // 优先创建文件夹
        Directory.CreateDirectory(Application.persistentDataPath + "\\" + filePath);
        string savePath = Application.persistentDataPath + "\\" + filePath + name + ".json";

        string data = File.ReadAllText(savePath);
        data = AlternativeName(data);

        return JsonMapper.ToObject<Chart>(data);

    }

    /// <summary>
    /// 将本家谱面数据的"Type"替换为enum类型
    /// </summary>
    /// <param name="data">传入原始读取数据</param>
    /// <returns>返回修改后的数据</returns>
    private string AlternativeName(string data)
    {
        data = data.Replace("\"Tap1\"", "0");
        data = data.Replace("\"Tap2\"", "1");
        data = data.Replace("\"LongStart\"", "2");
        data = data.Replace("\"LongEnd\"", "3");
        data = data.Replace("\"Slide\"", "4");
        data = data.Replace("\"ScratchLeft\"", "5");
        data = data.Replace("\"ScratchRight\"", "6");
        data = data.Replace("\"StopStart\"", "7");
        data = data.Replace("\"StopEnd\"", "8");
        return data;
    }
}
